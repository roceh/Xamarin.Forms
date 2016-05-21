using Skia.WindowsDesktop.Demo;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Skia;

[assembly: ExportRenderer(typeof(Label), typeof(LabelRenderer))]

namespace Skia.WindowsDesktop.Demo
{
    public class LabelRenderer : ViewRenderer<Label, SkiaLabel>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    SetNativeControl(new SkiaLabel());
                }

                UpdateText();
                UpdateLineBreakMode();
                UpdateAlignment();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Label.HorizontalTextAlignmentProperty.PropertyName)
                UpdateAlignment();
            else if (e.PropertyName == Label.VerticalTextAlignmentProperty.PropertyName)
                UpdateAlignment();
            else if (e.PropertyName == Label.TextColorProperty.PropertyName)
                UpdateText();
            else if (e.PropertyName == Label.FontProperty.PropertyName)
                UpdateText();
            else if (e.PropertyName == Label.TextProperty.PropertyName)
                UpdateText();
            else if (e.PropertyName == Label.FormattedTextProperty.PropertyName)
                UpdateText();
            else if (e.PropertyName == Label.LineBreakModeProperty.PropertyName)
                UpdateLineBreakMode();
        }

        void UpdateAlignment()
        {
            Control.HorizontalTextAlignment = Element.HorizontalTextAlignment;
            Control.VerticalTextAlignment = Element.VerticalTextAlignment;
        }

        void UpdateLineBreakMode()
        {
        }

        void UpdateText()
        {
            Control.Text = Element.Text;
            Control.FontAttributes = Element.FontAttributes;
            Control.FontSize = Element.FontSize;
            Control.FontFamily = Element.FontFamily;
            Control.TextColor = Element.TextColor == Color.Default ? Color.Black : Element.TextColor;
        }
    }
}