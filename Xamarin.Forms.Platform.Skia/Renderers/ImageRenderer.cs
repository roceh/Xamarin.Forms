using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SkiaSharp;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Skia;

[assembly: ExportRenderer(typeof(Image), typeof(ImageRenderer))]
[assembly: ExportImageSourceHandler(typeof(FileImageSource), typeof(FileImageSourceHandler))]
[assembly: ExportImageSourceHandler(typeof(StreamImageSource), typeof(StreamImagesourceHandler))]
[assembly: ExportImageSourceHandler(typeof(UriImageSource), typeof(ImageLoaderSourceHandler))]

namespace Xamarin.Forms.Platform.Skia
{
    public class ImageRenderer : ViewRenderer<Image, SkiaImage>
    {
        bool _isDisposed;

        public ImageRenderer()
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            if (disposing)
            {
                SKBitmap oldSKImage;
                if (Control != null && (oldSKImage = Control.Image) != null)
                {
                    oldSKImage.Dispose();
                    oldSKImage = null;
                }
            }

            _isDisposed = true;

            base.Dispose(disposing);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    SetNativeControl(new SkiaImage());
                }

                SetImage(e.OldElement);
                UpdateAspect();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Image.SourceProperty.PropertyName)
                SetImage();
            else if (e.PropertyName == Image.AspectProperty.PropertyName)
                UpdateAspect();
        }

        void UpdateAspect()
        {
            Control.Aspect = Element.Aspect;
        }

        async void SetImage(Image oldElement = null)
        {
            var source = Element.Source;

            if (oldElement != null)
            {
                var oldSource = oldElement.Source;
                if (Equals(oldSource, source))
                    return;

                if (oldSource is FileImageSource && source is FileImageSource && ((FileImageSource)oldSource).File == ((FileImageSource)source).File)
                    return;

                Control.Image = null;
            }

            IImageSourceHandler handler;

            ((IImageController)Element).SetIsLoading(true);

            if (source != null && (handler = Registrar.Registered.GetHandler<IImageSourceHandler>(source.GetType())) != null)
            {
                SKBitmap uiimage;
                try
                {
                    uiimage = await handler.LoadImageAsync(source);
                }
                catch (OperationCanceledException)
                {
                    uiimage = null;
                }

                var imageView = Control;
                if (imageView != null)
                    imageView.Image = uiimage;

                if (!_isDisposed)
                    ((IVisualElementController)Element).NativeSizeChanged();
            }
            else
                Control.Image = null;

            if (!_isDisposed)
                ((IImageController)Element).SetIsLoading(false);
        }
    }

    public interface IImageSourceHandler : IRegisterable
    {
        Task<SKBitmap> LoadImageAsync(ImageSource imagesource, CancellationToken cancelationToken = default(CancellationToken));
    }

    public sealed class FileImageSourceHandler : IImageSourceHandler
    {
        public Task<SKBitmap> LoadImageAsync(ImageSource imagesource, CancellationToken cancelationToken = default(CancellationToken))
        {
            SKBitmap image = null;
            var filesource = imagesource as FileImageSource;
            if (filesource != null)
            {
                var file = filesource.File;

                if (!Path.IsPathRooted(file))
                {
                    file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file);
                }

                if (!string.IsNullOrEmpty(file))
                    image = SKBitmap.Decode(file);
            }
            return Task.FromResult(image);
        }
    }

    public sealed class StreamImagesourceHandler : IImageSourceHandler
    {
        public async Task<SKBitmap> LoadImageAsync(ImageSource imagesource, CancellationToken cancelationToken = default(CancellationToken))
        {
            SKBitmap image = null;
            var streamsource = imagesource as StreamImageSource;
            if (streamsource != null && streamsource.Stream != null)
            {
                using (var streamImage = await ((IStreamImageSource)streamsource).GetStreamAsync(cancelationToken).ConfigureAwait(false))
                {
                    if (streamImage != null)
                        image = SKBitmap.Decode(streamImage.ToByteArray());
                }
            }
            return image;
        }
    }

    public sealed class ImageLoaderSourceHandler : IImageSourceHandler
    {
        public async Task<SKBitmap> LoadImageAsync(ImageSource imagesource, CancellationToken cancelationToken = default(CancellationToken))
        {
            SKBitmap image = null;
            var imageLoader = imagesource as UriImageSource;
            if (imageLoader != null && imageLoader.Uri != null)
            {
                using (var streamImage = await imageLoader.GetStreamAsync(cancelationToken).ConfigureAwait(false))
                {
                    if (streamImage != null)
                        image = SKBitmap.Decode(streamImage.ToByteArray());
                }
            }
            return image;
        }
    }
}

