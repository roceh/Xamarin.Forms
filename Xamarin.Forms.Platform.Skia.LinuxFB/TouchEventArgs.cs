using System;
using System.Collections.Generic;

namespace Xamarin.Forms.Platform.Skia
{
    public class TouchEventArgs : EventArgs
    {
        public List<TouchPoint> Touches { get; } = new List<TouchPoint>();
        
        public TouchEventArgs(List<TouchPoint> touches)
        {
            Touches.AddRange(touches);
        }
    }
}

