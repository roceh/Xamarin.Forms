using SkiaSharp;

namespace Xamarin.Forms.Platform.Skia
{
    public static class SkiaExtensions
    {
        public static SKColor ToSKColor(this Color color)
        {
            return new SKColor((byte)(color.R * 255), (byte)(color.G * 255), (byte)(color.B * 255), (byte)(color.A * 255));
        }

        public static SKPoint ToSKPoint(this Point point)
        {
            return new SKPoint((float)point.X, (float)point.Y);
        }

        public static SKRect ToSKRect(this Rectangle rect)
        {
            return new SKRect((float)rect.Left, (float)rect.Top, (float)rect.Right, (float)rect.Bottom);
        }
    }
}