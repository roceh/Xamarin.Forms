using System;
using System.Collections.Generic;

namespace Xamarin.Forms.Platform.Skia
{
    public interface IMouseHandler
    {
        double MouseX { get; }

        double MouseY { get; }

        double MaxX { get; set; }

        double MaxY { get; set; }

        event EventHandler<TouchEventArgs> Touched;

        void ProcessTouches();
    }
}

