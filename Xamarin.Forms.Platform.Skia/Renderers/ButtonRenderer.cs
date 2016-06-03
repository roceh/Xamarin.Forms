using Skia.WindowsDesktop.Demo;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Skia;
using System;

[assembly: ExportRenderer(typeof(Button), typeof(ButtonRenderer))]

namespace Xamarin.Forms.Platform.Skia
{
    public class ButtonRenderer : ViewRenderer<Button, SkiaButton>
    {
        protected override void Dispose(bool disposing)
        {
            if (Control != null)
                Control.Clicked -= Control_Tapped;

            base.Dispose(disposing);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    SetNativeControl(new SkiaButton());

                    Control.Clicked += Control_Tapped;
                }

                UpdateText();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Label.TextProperty.PropertyName)
                UpdateText();
        }

        void Control_Tapped (object sender, System.EventArgs e)
        {
            if (Element != null)
            {
                ((IButtonController)Element)?.SendClicked();
            }
        }

        void UpdateText()
        {
            Control.Text = Element.Text;
        }
    }
}

