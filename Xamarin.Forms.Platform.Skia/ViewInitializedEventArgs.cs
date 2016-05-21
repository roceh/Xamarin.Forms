using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamarin.Forms.Platform.Skia
{
    public class ViewInitializedEventArgs : EventArgs
    {
        public SkiaView NativeView { get; internal set; }

        public VisualElement View { get; internal set; }
    }
}
