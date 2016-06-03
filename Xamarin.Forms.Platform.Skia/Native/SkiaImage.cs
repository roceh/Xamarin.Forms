using System;
using SkiaSharp;

namespace Xamarin.Forms.Platform.Skia
{
    public class SkiaImage : SkiaView
    {
        SKBitmap _image;
        Aspect _aspect;

        public SkiaImage()
        {
            IsClippedToBounds = true;
        }

        public SKBitmap Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
                Invalidate();
            }
        }

        public Aspect Aspect
        {
            get
            {
                return _aspect;
            }
            set
            {
                _aspect = value;
                Invalidate();
            }
        }

        protected override void InternalDraw(SKCanvas canvas)
        {
            base.InternalDraw(canvas);

            if (Image != null)
            {
                var renderBounds = RenderBounds;

                float cx, cy, newWidth, newHeight;

                if (Aspect == Aspect.Fill)
                {
                    cx = (float) renderBounds.X;
                    cy = (float) renderBounds.Y;
                    newWidth = (float) Width;
                    newHeight = (float) Height;
                }
                else
                {
                    double ratioX = (double)renderBounds.Width / (double)Image.Width;
                    double ratioY = (double)renderBounds.Height / (double)Image.Height;
                    double ratio;

                    switch (Aspect)
                    {
                        case Aspect.AspectFill:
                            ratio = ratioX > ratioY ? ratioX : ratioY;
                            break;
                        default:
                            ratio = ratioX < ratioY ? ratioX : ratioY;
                            break;
                    }

                    newWidth = (float)(Image.Width * ratio);
                    newHeight = (float)(Image.Height * ratio);

                    cx = (float)(renderBounds.X + (renderBounds.Width / 2.0) - (newWidth / 2.0));
                    cy = (float)(renderBounds.Y + (renderBounds.Height / 2.0) - (newHeight / 2.0));
                }

                using (var paint = new SKPaint())
                {
                    paint.XferMode = SKXferMode.SrcOver;
                    paint.FilterQuality = SKFilterQuality.Low;

                    canvas.DrawBitmap(Image, new SKRect(cx, cy, cx + newWidth, cy + newHeight), paint);
                }
            }
        }

        public override Size SizeThatFits(double widthConstraint, double heightConstraint)
        {
            if (Image != null)
            {
                return new Size(Image.Width, Image.Height);
            }

            return new Size(0, 0);
        }
    }
}

