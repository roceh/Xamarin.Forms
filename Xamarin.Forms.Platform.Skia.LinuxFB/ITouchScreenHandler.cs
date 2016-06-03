using System;
using System.Collections.Generic;

namespace Xamarin.Forms.Platform.Skia
{
    public interface ITouchScreenHandler
    {
        event EventHandler<TouchEventArgs> Touched;

        void ProcessTouches();
    }
}

