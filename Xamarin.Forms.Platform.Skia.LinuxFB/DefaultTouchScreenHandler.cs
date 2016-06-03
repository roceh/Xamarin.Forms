using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace Xamarin.Forms.Platform.Skia
{
    public class DefaultTouchScreenHandler : ITouchScreenHandler
    {
        const int ABS_MT_POSITION_X = 0x35;
        const int ABS_MT_POSITION_Y = 0x36;
        const int ABS_MT_SLOT = 0x2f;
        const int ABS_X = 0x00;
        const int ABS_Y = 0x01;
        const int BTN_TOUCH = 0x14a;

        const int EV_ABS = 0x03;
        const int EV_KEY = 0x01;
        const int EV_SYN = 0x00;
        const int TouchUpTimeout = 250;

        string _device;
        ConcurrentQueue<input_event> _events = new ConcurrentQueue<input_event>();
        Rectangle _screenPoints;
        bool _touchEnded = false;
        List<TouchPoint> _touches = new List<TouchPoint>();
        int _touchId = 0;
        Rectangle _touchScreenPoints;
        Thread _touchScreenThread;
        double _tx = Double.PositiveInfinity;
        double _ty = Double.PositiveInfinity;

        public DefaultTouchScreenHandler(string device, Rectangle touchScreenPoints, Rectangle screenPoints)
        {
            _device = device;
            _touchScreenPoints = touchScreenPoints;
            _screenPoints = screenPoints;

            _touchScreenThread = new Thread(new ThreadStart(TouchScreenRun));
            _touchScreenThread.Start();
        }

        public event EventHandler<TouchEventArgs> Touched;

        public void ProcessTouches()
        {
            var now = DateTime.Now;

            input_event inputEvent;

            while (_events.TryDequeue(out inputEvent))
            {
                if (inputEvent.type == EV_ABS)
                {
                    switch (inputEvent.code)
                    {
                        case ABS_X:
                        case ABS_MT_POSITION_X:
                            _tx = TouchXToScreenX(inputEvent.value);
                            break;

                        case ABS_Y:
                        case ABS_MT_POSITION_Y:
                            _ty = TouchXToScreenX(inputEvent.value);
                            break;

                        case ABS_MT_SLOT:
                            _touchId = inputEvent.value;
                            break;
                    }
                }
                else if (inputEvent.type == EV_KEY)
                {
                    if (inputEvent.code == BTN_TOUCH)
                    {
                        _touchEnded = inputEvent.value == 0;
                    }
                }
                else if (inputEvent.type == EV_SYN)
                {
                    if (!_touchEnded)
                    {
                        var touch = _touches.Find(x => x.Id == _touchId);

                        if (touch == null)
                        {
                            touch = new TouchPoint(_touchId) { Action = TouchAction.Up };
                            _touches.Add(touch);
                        }

                        touch.Point = new Point(!Double.IsPositiveInfinity(_tx) ? _tx : touch.Point.X, !Double.IsPositiveInfinity(_ty) ? _ty : touch.Point.Y);
                        touch.Action = touch.Action == TouchAction.Up ? TouchAction.Down : TouchAction.Move;
                        touch.LastSeen = now;
                    }
                    else
                    {
                        _touches.ForEach(x => x.Action = TouchAction.Up);
                    }

                    Touched?.Invoke(this, new TouchEventArgs(_touches));

                    if (_touchEnded)
                    {
                        _touches.Clear();
                    }

                    _touchEnded = false;
                    _touchId = 0;
                    _tx = double.PositiveInfinity;
                    _ty = double.PositiveInfinity;
                }
            }
        }

        public void TouchScreenRun()
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

        double TouchXToScreenX(double x)
        {
            return (_screenPoints.X + ((x - _touchScreenPoints.X) / (_touchScreenPoints.Width)) * _screenPoints.Width);
        }

        double TouchYToScreenY(double y)
        {
            return (_screenPoints.Y + ((y - _touchScreenPoints.X) / (_touchScreenPoints.Height)) * _screenPoints.Height);
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