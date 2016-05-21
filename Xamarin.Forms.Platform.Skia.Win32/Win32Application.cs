using SkiaSharp;
using System;
using System.Runtime.InteropServices;

namespace Xamarin.Forms.Platform.Skia
{
    public class Win32Window : ISKiaRenderer
    {
        private static Win32Window _window;
        private static Win32Api.WndProc _windowProc = StaticWndProc;
        private Application _app;
        private IntPtr _hWnd;
        private bool _isClosed;
        private int _width, _height;
        private ushort _windowClass;

        public Win32Window(int width, int height)
        {
            Win32Api.SetProcessDPIAware();

            IntPtr hInstance = Win32Api.GetModuleHandle(null);

            if (_windowClass == 0)
            {
                Win32Api.WNDCLASSEX wc = new Win32Api.WNDCLASSEX();
                wc.cbSize = (uint)Marshal.SizeOf(typeof(Win32Api.WNDCLASSEX));
                wc.style = 0;
                wc.lpfnWndProc = _windowProc;
                wc.cbClsExtra = 0;
                wc.cbWndExtra = 0;
                wc.hInstance = hInstance;
                wc.hIcon = Win32Api.LoadIcon(IntPtr.Zero, Win32Api.IDI_APPLICATION);
                wc.hCursor = Win32Api.LoadCursor(IntPtr.Zero, Win32Api.IDC_ARROW);
                wc.hbrBackground = (IntPtr)(Win32Api.COLOR_WINDOW + 1);
                wc.lpszMenuName = null;
                wc.lpszClassName = typeof(Win32Window).FullName;
                wc.hIconSm = Win32Api.LoadIcon(IntPtr.Zero, Win32Api.IDI_APPLICATION);

                _windowClass = Win32Api.RegisterClassEx(ref wc);

                if (_windowClass == 0)
                    throw new InvalidOperationException(Win32Api.GetLastErrorString());
            }

            _hWnd = Win32Api.CreateWindowEx(
                Win32Api.WS_EX_CLIENTEDGE,
                _windowClass,
                string.Empty,
                Win32Api.WS_OVERLAPPEDWINDOW,
                Win32Api.CW_USEDEFAULT,
                Win32Api.CW_USEDEFAULT,
                width,
                height,
                IntPtr.Zero,
                IntPtr.Zero,
                hInstance,
                IntPtr.Zero);

            if (_hWnd == IntPtr.Zero)
                throw new InvalidOperationException(Win32Api.GetLastErrorString());

            _window = this;
        }

        public void Run(Application app)
        {
            _app = app;

            Win32Api.MSG message;

            Win32Api.ShowWindow(_hWnd, Win32Api.SW_SHOWNORMAL);
            Win32Api.UpdateWindow(_hWnd);

            while (!_isClosed)
            {
                if (Win32Api.PeekMessage(out message, IntPtr.Zero, 0, 0, Win32Api.PM_REMOVE))
                {
                    Win32Api.TranslateMessage(ref message);
                    Win32Api.DispatchMessage(ref message);
                }
            }
        }

        private static IntPtr StaticWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            if (_window != null)
                return _window.WndProc(msg, wParam, lParam);

            return Win32Api.DefWindowProc(hWnd, msg, wParam, lParam);
        }

        private void OnClose()
        {
            _isClosed = true;
            Win32Api.DestroyWindow(_hWnd);
        }

        private void OnPaint()
        {
            var ps = new Win32Api.PAINTSTRUCT();
            IntPtr hdc = Win32Api.BeginPaint(_hWnd, ref ps);

            if (hdc != IntPtr.Zero)
            {
                try
                {
                    IntPtr bits = IntPtr.Zero;

                    var memoryDC = Win32Api.CreateCompatibleDC(IntPtr.Zero);

                    var bitmapInfo = new Win32Api.BITMAPINFO();
                    bitmapInfo.bmiHeader.biSize = Marshal.SizeOf(typeof(Win32Api.BITMAPINFOHEADER));
                    bitmapInfo.bmiHeader.biWidth = _width;
                    bitmapInfo.bmiHeader.biHeight = -_height;
                    bitmapInfo.bmiHeader.biPlanes = 1;
                    bitmapInfo.bmiHeader.biBitCount = 32;
                    bitmapInfo.bmiHeader.biClrImportant = 0;
                    bitmapInfo.bmiHeader.biClrUsed = 0;
                    bitmapInfo.bmiHeader.biCompression = Win32Api.BI_RGB;

                    var bitmapHandle = Win32Api.CreateDIBSection(hdc, ref bitmapInfo, Win32Api.DIB_RGB_COLORS, ref bits, IntPtr.Zero, 0);

                    var renderer = Platform.CreateRenderer(_app.MainPage);
                    Platform.SetRenderer(_app.MainPage, renderer);

                    renderer.NativeView.RootRenderer = this;

                    _app.MainPage.Layout(new Rectangle(0, 0, _width, _height));

                    using (var surface = SKSurface.Create(_width, _height, SKColorType.N_32, SKAlphaType.Opaque, bits, _width * 4))
                    {
                        surface.Canvas.Clear(Color.White.ToSKColor());
                        renderer.NativeView.Draw(surface.Canvas);
                    }

                    Win32Api.SelectObject(memoryDC, bitmapHandle);

                    Win32Api.BitBlt(hdc, 0, 0, _width, _height, memoryDC, 0, 0, Win32Api.TernaryRasterOperations.SRCCOPY);

                    Win32Api.DeleteDC(memoryDC);
                    Win32Api.DeleteObject(bitmapHandle);
                }
                finally
                {
                    Win32Api.EndPaint(_hWnd, ref ps);
                }
            }
        }

        private void OnResize(int width, int height)
        {
            _width = width;
            _height = height;
            Win32Api.InvalidateRect(_hWnd, IntPtr.Zero, false);
        }

        private IntPtr WndProc(uint msg, IntPtr wParam, IntPtr lParam)
        {
            switch (msg)
            {
                case Win32Api.WM_SIZE:
                    OnResize(Win32Api.LowWord(lParam), Win32Api.HighWord(lParam));
                    break;

                case Win32Api.WM_CLOSE:
                    OnClose();
                    break;

                case Win32Api.WM_PAINT:
                    OnPaint();
                    break;
            }

            return Win32Api.DefWindowProc(_hWnd, msg, wParam, lParam);
        }

        public void Invalidate(SkiaView view)
        {
            Win32Api.InvalidateRect(_hWnd, IntPtr.Zero, false);
        }
    }
}