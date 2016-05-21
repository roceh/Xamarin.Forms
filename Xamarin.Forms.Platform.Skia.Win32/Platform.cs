using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xamarin.Forms.Platform.Skia
{
    public class Platform : BindableObject, IPlatform, INavigation, IDisposable
    {
        internal static readonly BindableProperty RendererProperty = BindableProperty.CreateAttached("Renderer", typeof(IVisualElementRenderer), typeof(Platform), default(IVisualElementRenderer),
            propertyChanged: (bindable, oldvalue, newvalue) =>
           {
               var view = bindable as VisualElement;
               if (view != null)
                   view.IsPlatformEnabled = newvalue != null;
           });

        readonly List<Page> _modals;
        readonly PlatformRenderer _renderer;

        bool _disposed;

        internal Platform()
        {
            _renderer = new PlatformRenderer(this);
            _modals = new List<Page>();
        }

        public IReadOnlyList<Page> ModalStack
        {
            get { return _modals; }
        }

        public IReadOnlyList<Page> NavigationStack
        {
            get { return new List<Page>(); }
        }

        public static IVisualElementRenderer CreateRenderer(VisualElement element)
        {
            var t = element.GetType();
            var renderer = Registrar.Registered.GetHandler<IVisualElementRenderer>(t) ?? new DefaultRenderer();
            renderer.SetElement(element);
            return renderer;
        }

        public static IVisualElementRenderer GetRenderer(VisualElement bindable)
        {
            return (IVisualElementRenderer)bindable.GetValue(RendererProperty);
        }

        public static void SetRenderer(VisualElement bindable, IVisualElementRenderer value)
        {
            bindable.SetValue(RendererProperty, value);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
        }

        public SizeRequest GetNativeSize(VisualElement view, double widthConstraint, double heightConstraint)
        {
            var renderView = GetRenderer(view);

            if (renderView == null || renderView.NativeView == null)
                return new SizeRequest(Size.Zero);

            return renderView.GetDesiredSize(widthConstraint, heightConstraint);
        }

        public void InsertPageBefore(Page page, Page before)
        {
            throw new NotImplementedException();
        }

        public Task<Page> PopAsync()
        {
            return ((INavigation)this).PopAsync(true);
        }

        public Task<Page> PopAsync(bool animated)
        {
            throw new NotImplementedException();
        }

        public Task<Page> PopModalAsync()
        {
            return ((INavigation)this).PopModalAsync(true);
        }

        public Task<Page> PopModalAsync(bool animated)
        {
            throw new NotImplementedException();
        }

        public Task PopToRootAsync()
        {
            return ((INavigation)this).PopToRootAsync(true);
        }

        public Task PopToRootAsync(bool animated)
        {
            throw new NotImplementedException();
        }

        public Task PushAsync(Page page)
        {
            return ((INavigation)this).PushAsync(page, true);
        }

        public Task PushAsync(Page page, bool animated)
        {
            throw new NotImplementedException();
        }

        public Task PushModalAsync(Page page)
        {
            return ((INavigation)this).PushModalAsync(page, true);
        }

        public Task PushModalAsync(Page page, bool animated)
        {
            throw new NotImplementedException();
        }

        public void RemovePage(Page page)
        {
            throw new NotImplementedException();
        }

        internal class DefaultRenderer : VisualElementRenderer<VisualElement>
        {
        }
    }
}