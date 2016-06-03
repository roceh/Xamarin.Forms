using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace Xamarin.Forms.Platform.Skia
{
    public class DefaultMouseHandler : IMouseHandler
    {
        const int BTN_LEFT = 0x110;
        const int EV_KEY = 0x01;
        const int EV_REL = 0x02;
        const int EV_SYN = 0x00;
        const int REL_X = 0x00;
        const int REL_Y = 0x01;

        string _device;
        bool _down;
        ConcurrentQueue<input_event> _events = new ConcurrentQueue<input_event>();
        Thread _mouseThread;
        TouchPoint _touch;
        double _tx = Double.PositiveInfinity;
        double _ty = Double.PositiveInfinity;

        public DefaultMouseHandler(string device)
        {
            _device = device;

            _mouseThread = new Thread(new ThreadStart(MouseRun));
            _mouseThread.Start();
        }

        public event EventHandler<TouchEventArgs> Touched;

        public double MaxX { get; set; }

        public double MaxY { get; set; }

        public double MouseX { get; private set; }

        public double MouseY { get; private set; }

        public void MouseRun()
        {
            FileStream fs = new FileStream(_device, System.IO.FileMode.Open, System.IO.FileAccess.Read);

            int structSize = Marshal.SizeOf(typeof(input_event));

            byte[] data = new byte[structSize];

            int bytes = 0;

            while (true)
            {
                bytes += fs.Read(data, bytes, structSize - bytes);

                if (bytes == structSize)
                {
                    lock (this)
                    {
                        _events.Enqueue(Utils.BytesToStruct<input_event>(data, 0));
                    }

                    bytes = 0;
                }
            }
        }

        public void ProcessTouches()
        {
            var now = DateTime.Now;

            input_event inputEvent;

            while (_events.TryDequeue(out inputEvent))
            {
                if (inputEvent.type == EV_REL)
                {
                    switch (inputEvent.code)
                    {
                        case REL_X:
                            _tx = inputEvent.value;
                            break;

                        case REL_Y:
                            _ty = inputEvent.value;
                            break;
                    }
                }
                else if (inputEvent.type == EV_KEY)
                {
                    if (inputEvent.code == BTN_LEFT)
                    {
                        _down = inputEvent.value == 1;
                    }
                }
                else if (inputEvent.type == EV_SYN)
                {
                    MouseX = Math.Min(MaxX, Math.Max(0, MouseX + (!Double.IsPositiveInfinity(_tx) ? _tx : 0)));
                    MouseY = Math.Min(MaxY, Math.Max(0, MouseY + (!Double.IsPositiveInfinity(_ty) ? _ty : 0)));

                    if (_down)
                    {
                        if (_touch == null)
                        {
                            _touch = new TouchPoint(0) { Action = TouchAction.Up };
                        }

                        _touch.Point = new Point(MouseX, MouseY);
                        _touch.Action = _touch.Action == TouchAction.Up ? TouchAction.Down : TouchAction.Move;
                        _touch.LastSeen = now;
                    }
                    else if (_touch != null)
                    {
                        _touch.Action = TouchAction.Up;
                    }

                    if (_touch != null)
                    {
                        Touched?.Invoke(this, new TouchEventArgs(new List<TouchPoint> { _touch }));

                        switch (_touch.Action)
                        {
                            case TouchAction.Up:
                                _touch = null;
                                break;
                        }
                    }

                    _tx = double.PositiveInfinity;
                    _ty = double.PositiveInfinity;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct input_event
        {
            public timeval time;
            public UInt16 type;
            public UInt16 code;
            public Int32 value;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct timeval
        {
            public Int32 tv_sec;
            public Int32 tv_usec;
        }
    }
}