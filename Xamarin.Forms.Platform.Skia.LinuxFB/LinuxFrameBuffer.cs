using Mono.Unix;
using Mono.Unix.Native;
using SkiaSharp;
using System;
using System.Collections.Concurrent;

namespace Xamarin.Forms.Platform.Skia
{
    public class LinuxFrameBuffer : ISKiaRenderer
    {
        const int MouseIndicatorRadius = 5;

        Application _app;
        BlockingCollection<Action> _queue = new BlockingCollection<Action>();

        public LinuxFrameBuffer()
        {
            Forms.SkiaWindowsPlatformServices.MainThreadQueue = _queue;
        }

        public SkiaView Focused { get; set; }

        public IKeyboardHandler KeyboardHandler { get; set; }

        public IMouseHandler MouseHandler { get; set; }

        public bool ShowFpsCounter { get; set; }

        public ITouchScreenHandler TouchScreenHandler { get; set; }

        public void Invalidate(SkiaView view)
        {
        }

        public void Run(Application app)
        {
            _app = app;

            var kbfd = LinuxApi.open("/dev/tty", LinuxApi.O_WRONLY);
            if (kbfd < 0)
            {
                throw new Exception("Cannot open access to tty");
            }

            try
            {
                var fbfd = LinuxApi.open("/dev/fb0", LinuxApi.O_RDWR);

                if (fbfd < 0)
                {
                    throw new Exception("Cannot open access to framebuffer");
                }

                // disable flashing cursor (app must be running on the local console)
                LinuxApi.ioctl(kbfd, LinuxApi.KDSETMODE, LinuxApi.KD_GRAPHICS);

                try
                {
                    UInt32 dummy;

                    // get the virtual screen info
                    var vinfo = new LinuxApi.fb_var_screeninfo();
                    LinuxApi.ioctl(fbfd, LinuxApi.FBIOGET_VSCREENINFO, ref vinfo);

                    // create an extra buffer below the normal
                    vinfo.yres_virtual = vinfo.xres;
                    vinfo.yres_virtual = vinfo.yres * 2;
                    LinuxApi.ioctl(fbfd, LinuxApi.FBIOPUT_VSCREENINFO, ref vinfo);

                    // get the fixed info
                    var finfo = new LinuxApi.fb_fix_screeninfo();
                    LinuxApi.ioctl(fbfd, LinuxApi.FBIOGET_FSCREENINFO, ref finfo);

                    // map a block of memory to the total fixed screen
                    var fbp = LinuxApi.mmap(IntPtr.Zero, finfo.smem_len, LinuxApi.ProtectionFlags.PROT_READ | LinuxApi.ProtectionFlags.PROT_WRITE, LinuxApi.MMapFlags.MAP_SHARED, fbfd, 0);
                    try
                    {
                        // get size of one screen
                        var page_size = finfo.line_length * vinfo.yres;

                        // current page we are drawing to
                        int bufferPage = 0;

                        // code to wait for vsync
                        uint FBIO_WAITFORVSYNC = LinuxApi.IOW('F', 0x20, 4);

                        // set initial page we are displaying
                        vinfo.yoffset = (uint)bufferPage * vinfo.yres;
                        LinuxApi.ioctl(fbfd, LinuxApi.FBIOPAN_DISPLAY, ref vinfo);
                        dummy = 0;
                        LinuxApi.ioctl(fbfd, FBIO_WAITFORVSYNC, ref dummy);

                        _app.MainPage.Platform = new Platform();

                        // setup xamarin page renderer
                        IVisualElementRenderer renderer = Platform.GetRenderer(_app.MainPage);

                        if (renderer == null)
                        {
                            renderer = Platform.CreateRenderer(_app.MainPage);
                            Platform.SetRenderer(_app.MainPage, renderer);
                        }

                        renderer.NativeView.RootRenderer = this;

                        UnixSignal signal = new UnixSignal(Signum.SIGINT);

                        DateTime lastFrame = DateTime.Now;

                        int currentFps = 0;

                        string fpsString = "";

                        Focused = renderer.NativeView;

                        if (KeyboardHandler != null)
                        {
                            KeyboardHandler.KeyboardCharacter += (sender, e) => Focused?.ProcessKeyChar(e.Character);
                            KeyboardHandler.KeyboardKey += (sender, e) => Focused?.ProcessKey(e.Key, e.Down);
                        }

                        if (TouchScreenHandler != null)
                        {
                            TouchScreenHandler.Touched += (sender, e) => renderer.NativeView.ProcessTouches(e.Touches);
                        }

                        if (MouseHandler != null)
                        {
                            MouseHandler.Touched += (sender, e) => renderer.NativeView.ProcessTouches(e.Touches);
                            MouseHandler.MaxX = vinfo.xres;
                            MouseHandler.MaxY = vinfo.yres;
                        }

                        _app.MainPage.Layout(new Rectangle(0, 0, vinfo.xres, vinfo.yres));

                        using (var paint = new SKPaint())
                        {
                            while (!signal.IsSet)
                            {
                                Action action;

                                DateTime syncStart = DateTime.Now;

                                while (_queue.Count > 0 && (DateTime.Now - syncStart).TotalMilliseconds < 16)
                                {
                                    if (_queue.TryTake(out action))
                                    {
                                        action();
                                    }
                                }

                                TouchScreenHandler?.ProcessTouches();
                                MouseHandler?.ProcessTouches();

                                KeyboardHandler?.ProcessKeyboardBuffer();

                                var now = DateTime.Now;

                                if (ShowFpsCounter && (now - lastFrame).TotalSeconds > 1.0)
                                {
                                    fpsString = currentFps.ToString();
                                    currentFps = 0;
                                    lastFrame = now;
                                }

                                bufferPage = (bufferPage + 1) % 2;

                                // render to the offsreen page
                                using (var surface = SKSurface.Create((int)vinfo.xres, (int)vinfo.yres, SKColorType.Rgb_565, SKAlphaType.Opaque, new IntPtr((long)fbp + (page_size * bufferPage)), (int)vinfo.xres * 2))
                                {
                                    var canvas = surface.Canvas;
                                    canvas.Clear(Color.White.ToSKColor());

                                    renderer.NativeView.Draw(canvas);

                                    if (ShowFpsCounter)
                                    {
                                        canvas.DrawText(fpsString, 0, 10, paint);
                                    }

                                    if (MouseHandler != null)
                                    {
                                        paint.Color = Color.Black.ToSKColor();
                                        paint.IsStroke = false;
                                        canvas.DrawOval(new SKRect((float)MouseHandler.MouseX - MouseIndicatorRadius, (float)MouseHandler.MouseY - MouseIndicatorRadius,
                                                                   (float)MouseHandler.MouseX + MouseIndicatorRadius, (float)MouseHandler.MouseY + MouseIndicatorRadius), paint);

                                        paint.Color = Color.White.ToSKColor();
                                        paint.IsStroke = true;
                                        canvas.DrawOval(new SKRect((float)MouseHandler.MouseX - MouseIndicatorRadius, (float)MouseHandler.MouseY - MouseIndicatorRadius,
                                                                   (float)MouseHandler.MouseX + MouseIndicatorRadius, (float)MouseHandler.MouseY + MouseIndicatorRadius), paint);
                                    }
                                }

                                // page flip
                                vinfo.yoffset = (uint)bufferPage * vinfo.yres;
                                LinuxApi.ioctl(fbfd, LinuxApi.FBIOPAN_DISPLAY, ref vinfo);
                                dummy = 0;
                                LinuxApi.ioctl(fbfd, FBIO_WAITFORVSYNC, ref dummy);

                                currentFps++;
                            }
                        }
                    }
                    finally
                    {
                        LinuxApi.munmap(fbp, finfo.smem_len);
                    }
                }
                finally
                {
                    LinuxApi.close(fbfd);
                }

                LinuxApi.ioctl(kbfd, LinuxApi.KDSETMODE, LinuxApi.KD_TEXT);
            }
            finally
            {
                LinuxApi.close(kbfd);
            }
        }
    }
}