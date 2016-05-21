using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.Forms.Platform.Skia
{
    public interface ISKiaRenderer
    {
        void Invalidate(SkiaView view);
    }
}
