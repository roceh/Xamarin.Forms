namespace Xamarin.Forms.Platform.Skia
{
    internal class PlatformRenderer : SkiaView
    {
        internal PlatformRenderer(Platform platform)
        {
            Platform = platform;
        }

        public Platform Platform { get; set; }
    }
}