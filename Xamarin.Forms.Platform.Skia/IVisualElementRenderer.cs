using System;

namespace Xamarin.Forms.Platform.Skia
{
    public interface IVisualElementRenderer : IDisposable, IRegisterable
    {
        event EventHandler<VisualElementChangedEventArgs> ElementChanged;

        VisualElement Element { get; }

        SkiaView NativeView { get; }

        SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint);

        void SetElement(VisualElement element);

        void SetElementSize(Size size);
    }
}