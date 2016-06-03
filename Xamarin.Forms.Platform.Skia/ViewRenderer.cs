using System.ComponentModel;

namespace Xamarin.Forms.Platform.Skia
{
    public abstract class ViewRenderer : ViewRenderer<View, SkiaView>
    {
    }

    public abstract class ViewRenderer<TView, TNativeView> : VisualElementRenderer<TView> where TView : View where TNativeView : SkiaView
    {
        Color _defaultColor;

        public TNativeView Control { get; private set; }

        /// <summary>
        /// Determines whether the native control is disposed of when this renderer is disposed
        /// Can be overridden in deriving classes
        /// </summary>
        protected virtual bool ManageNativeControlLifetime => true;

        public override void Layout()
        {
            if (Control != null)
            {
                Control.X = 0;
                Control.Y = 0;
                Control.Width = Element.Width;
                Control.Height = Element.Height;
                Control.Layout();
            }
        }

        public override Size SizeThatFits(double widthConstraint, double heightConstraint)
        {
            return Control.SizeThatFits(widthConstraint, heightConstraint);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<TView> e)
        {
            base.OnElementChanged(e);

            //if (e.OldElement != null)
            //    e.OldElement.FocusChangeRequested -= ViewOnFocusChangeRequested;

            if (e.NewElement != null)
            {
                if (Control != null && e.OldElement != null && e.OldElement.BackgroundColor != e.NewElement.BackgroundColor || e.NewElement.BackgroundColor != Color.Default)
                    SetBackgroundColor(e.NewElement.BackgroundColor);

                //e.NewElement.FocusChangeRequested += ViewOnFocusChangeRequested;
            }

            UpdateIsEnabled();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Control != null)
            {
                if (e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
                    UpdateIsEnabled();
                else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
                    SetBackgroundColor(Element.BackgroundColor);
            }

            base.OnElementPropertyChanged(sender, e);
        }

        protected override void OnRegisterEffect(PlatformEffect effect)
        {
            base.OnRegisterEffect(effect);
            effect.Control = Control;
        }

        protected void SetNativeControl(TNativeView uiview)
        {
            _defaultColor = uiview.BackgroundColor;

            Control = uiview;

            if (Element.BackgroundColor != Color.Default)
                SetBackgroundColor(Element.BackgroundColor);

            UpdateIsEnabled();

            Children.Add(uiview);
        }

        void UpdateIsEnabled()
        {
            if (Element == null || Control == null)
                return;

            var uiControl = Control as SkiaView;

            if (uiControl == null)
                return;

            uiControl.IsEnabled = Element.IsEnabled;
        }
    }
}