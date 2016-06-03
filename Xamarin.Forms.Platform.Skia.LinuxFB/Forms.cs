﻿using System;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Skia;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Xamarin.Forms
{
    public static class Forms
    {
        //Preserve GetCallingAssembly
        static readonly bool nevertrue = false;

        static Forms()
        {
            if (nevertrue)
                Assembly.GetCallingAssembly();
        }

        public static event EventHandler<ViewInitializedEventArgs> ViewInitialized;

        public static bool IsInitialized { get; private set; }

        public static void Init()
        {
            if (IsInitialized)
                return;
            IsInitialized = true;
            Color.Accent = Color.FromRgba(50, 79, 133, 255);

            Log.Listeners.Add(new DelegateLogListener((c, m) => Trace.WriteLine(m, c)));

            Device.OS = TargetPlatform.Other;
            Device.PlatformServices = new SkiaWindowsPlatformServices();
            Device.Info = new WindowsSkiaDeviceInfo();

            Registrar.RegisterAll(new[] { typeof(ExportRendererAttribute), typeof(ExportCellAttribute), typeof(ExportImageSourceHandlerAttribute) });
        }

        internal static void SendViewInitialized(this VisualElement self, SkiaView nativeView)
        {
            var viewInitialized = ViewInitialized;
            if (viewInitialized != null)
                viewInitialized(self, new ViewInitializedEventArgs { View = self, NativeView = nativeView });
        }

        internal class WindowsSkiaDeviceInfo : DeviceInfo
        {
            readonly Size _scaledScreenSize;
            readonly double _scalingFactor;

            public WindowsSkiaDeviceInfo()
            {
                _scalingFactor = 1.0;
                var dim = new Size(800, 600); // FIXME
                _scaledScreenSize = new Size(dim.Width, dim.Height);
                PixelScreenSize = new Size(_scaledScreenSize.Width * _scalingFactor, _scaledScreenSize.Height * _scalingFactor);
            }

            public override Size PixelScreenSize { get; }

            public override Size ScaledScreenSize
            {
                get { return _scaledScreenSize; }
            }

            public override double ScalingFactor
            {
                get { return _scalingFactor; }
            }
        }

        internal class SkiaWindowsPlatformServices : IPlatformServices
        {
			static BlockingCollection<Action> _mainThreadQueue;
            static readonly MD5CryptoServiceProvider Checksum = new MD5CryptoServiceProvider();
			static int _mainThreadId;

			public static BlockingCollection<Action> MainThreadQueue
			{
				get
				{
					return _mainThreadQueue;
				}
				set
				{
					_mainThreadQueue = value;
					_mainThreadId = Thread.CurrentThread.ManagedThreadId;
				}
			}

            public bool IsInvokeRequired
            {
				get { return _mainThreadId == Thread.CurrentThread.ManagedThreadId; }
            }

            public void BeginInvokeOnMainThread(Action action)
            {
				_mainThreadQueue?.Add (action);
            }

            public Ticker CreateTicker()
            {
				return new LinuxTicker ();
            }

            public Assembly[] GetAssemblies()
            {
                return AppDomain.CurrentDomain.GetAssemblies();
            }

            public string GetMD5Hash(string input)
            {
                var bytes = Checksum.ComputeHash(Encoding.UTF8.GetBytes(input));
                var ret = new char[32];
                for (var i = 0; i < 16; i++)
                {
                    ret[i * 2] = (char)Hex(bytes[i] >> 4);
                    ret[i * 2 + 1] = (char)Hex(bytes[i] & 0xf);
                }
                return new string(ret);
            }

            public double GetNamedSize(NamedSize size, Type targetElementType, bool useOldSizes)
            {
                // We make these up anyway, so new sizes didn't really change
                // iOS docs say default button font size is 15, default label font size is 17 so we use those as the defaults.
                switch (size)
                {
                    case NamedSize.Default:
                        return typeof(Button).IsAssignableFrom(targetElementType) ? 15 : 17;

                    case NamedSize.Micro:
                        return 12;

                    case NamedSize.Small:
                        return 14;

                    case NamedSize.Medium:
                        return 17;

                    case NamedSize.Large:
                        return 22;

                    default:
                        throw new ArgumentOutOfRangeException("size");
                }
            }

            public async Task<Stream> GetStreamAsync(Uri uri, CancellationToken cancellationToken)
            {
				using (var client = new HttpClient())
                using (var response = await client.GetAsync(uri, cancellationToken))
                    return await response.Content.ReadAsStreamAsync();
            }

            public IIsolatedStorageFile GetUserStoreForApplication()
            {
                return new _IsolatedStorageFile(IsolatedStorageFile.GetUserStoreForApplication());
            }

            public void OpenUriAction(Uri uri)
            {  
				// FIXME: no web browser
            }

            public void StartTimer(TimeSpan interval, Func<bool> callback)
			{
				var t = new System.Timers.Timer (interval.TotalMilliseconds);

				t.Elapsed += (sender, e) => {
					if (!callback ()) {
						t.Dispose ();
					}
				};

				t.Start ();
			}
		
            static int Hex(int v)
            {
                if (v < 10)
                    return '0' + v;
                return 'a' + v - 10;
            }

            public class _IsolatedStorageFile : IIsolatedStorageFile
            {
                readonly IsolatedStorageFile _isolatedStorageFile;

                public _IsolatedStorageFile(IsolatedStorageFile isolatedStorageFile)
                {
                    _isolatedStorageFile = isolatedStorageFile;
                }

                public Task CreateDirectoryAsync(string path)
                {
                    _isolatedStorageFile.CreateDirectory(path);
                    return Task.FromResult(true);
                }

                public Task<bool> GetDirectoryExistsAsync(string path)
                {
                    return Task.FromResult(_isolatedStorageFile.DirectoryExists(path));
                }

                public Task<bool> GetFileExistsAsync(string path)
                {
                    return Task.FromResult(_isolatedStorageFile.FileExists(path));
                }

                public Task<DateTimeOffset> GetLastWriteTimeAsync(string path)
                {
                    return Task.FromResult(_isolatedStorageFile.GetLastWriteTime(path));
                }

                public Task<Stream> OpenFileAsync(string path, FileMode mode, FileAccess access)
                {
                    Stream stream = _isolatedStorageFile.OpenFile(path, (System.IO.FileMode)mode, (System.IO.FileAccess)access);
                    return Task.FromResult(stream);
                }

                public Task<Stream> OpenFileAsync(string path, FileMode mode, FileAccess access, FileShare share)
                {
                    Stream stream = _isolatedStorageFile.OpenFile(path, (System.IO.FileMode)mode, (System.IO.FileAccess)access, (System.IO.FileShare)share);
                    return Task.FromResult(stream);
                }
            }

            public class _Timer : ITimer
            {
                readonly Timer _timer;

                public _Timer(Timer timer)
                {
                    _timer = timer;
                }

                public void Change(int dueTime, int period)
                {
                    _timer.Change(dueTime, period);
                }

                public void Change(long dueTime, long period)
                {
                    _timer.Change(dueTime, period);
                }

                public void Change(TimeSpan dueTime, TimeSpan period)
                {
                    _timer.Change(dueTime, period);
                }

                public void Change(uint dueTime, uint period)
                {
                    _timer.Change(dueTime, period);
                }
            }
        }
    }
}