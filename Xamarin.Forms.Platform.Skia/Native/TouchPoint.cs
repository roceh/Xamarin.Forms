using System;

namespace Xamarin.Forms.Platform.Skia
{
    public class TouchPoint
    {
        public TouchPoint(int id)
        {
            Id = id;
        }

        public TouchAction Action { get; set; }

        public SkiaView HandledBy { get; set; }

        public int Id { get; private set; }

        public DateTime LastSeen { get; set; }

        public Point Point { get; set; }

        public Point GetViewPoint(SkiaView view)
        {
            var renderBounds = view.RenderBounds;
            return new Point(Point.X - renderBounds.X, Point.Y - renderBounds.Y);
        }
    }
}