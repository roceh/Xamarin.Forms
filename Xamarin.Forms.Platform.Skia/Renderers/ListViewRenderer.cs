using System;
using System.Collections;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Skia;

[assembly: ExportRenderer(typeof(ListView), typeof(ListViewRenderer))]

namespace Xamarin.Forms.Platform.Skia
{
    public class ListViewRenderer : ViewRenderer<ListView, SkiaList>
    {
        SkiaListSource _source;

        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    SetNativeControl(new SkiaList());

                    _source = new ListViewRendererSource(Element);
                    Control.Source = _source;
                    Control.RowHeight = Element.RowHeight;
                    Control.HasUnevenRows = Element.HasUnevenRows;
                    Control.Selected += HandleSelected;
                }
            }
        }

        void HandleSelected(object sender, SkiaListSelectedItemEventArgs e)
        {
            if (e.Row >= 0)
            {
                if (Element.IsGroupingEnabled)
                {
                    Element.SelectedItem = ((Element.ItemsSource as IList)[e.Group] as IList)[e.Row];
                }
                else
                {
                    Element.SelectedItem = (Element.ItemsSource as IList)[e.Row];
                }
            }
        }
   }
}