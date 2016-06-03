using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Xamarin.Forms.Platform.Skia
{
    public class SkiaView : IAnimatable
    {
        const int MaxTapTime = 300;
        const int PanThreshold = 2;

        Color _backgroundColor = Color.Transparent;
        ObservableList<SkiaView> _children = new ObservableList<SkiaView>();
        DateTime _downTime;
        Color _frameColor = Color.Transparent;
        double _frameThickness = 0.0;
        double _height;
        Point _initialDown;
        bool _isClippedToBounds;
        bool _isEnabled = true;
        bool _isVisible = true;
        double _opacity = 1.0;
        SkiaView _parent;
        ISKiaRenderer _rootRenderer;
        double _scale = 1.0;
        double _translationX;
        double _translationY;
        double _width;
        double _x;
        double _y;
        int _zPosition;

        public SkiaView()
        {
            _children.CollectionChanged += _children_CollectionChanged;
        }

        public Color BackgroundColor
        {
            get
            {
                return _backgroundColor;
            }

            set
            {
                _backgroundColor = value; Invalidate();
            }
        }

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(X, Y, Width, Height);
            }

            set
            {
                X = value.X;
                Y = value.Y;
                Width = value.Width;
                Height = value.Height;
            }
        }

        public IList<SkiaView> Children
        {
            get
            {
                return _children;
            }
        }

        public Color FrameColor
        {
            get
            {
                return _frameColor;
            }

            set
            {
                _frameColor = value;
                Invalidate();
            }
        }

        public double FrameThickness
        {
            get
            {
                return _frameThickness;
            }

            set
            {
                _frameThickness = value;
                Invalidate();
            }
        }

        public double Height
        {
            get
            {
                return _height;
            }

            set
            {
                _height = value;
                Invalidate();
            }
        }

        public double InheritedX
        {
            get
            {
                if (Parent != null)
                {
                    return X + Parent.InheritedX;
                }

                return 0;
            }
        }

        public double InheritedY
        {
            get
            {
                if (Parent != null)
                {
                    return Y + Parent.InheritedY;
                }

                return 0;
            }
        }

        public double InheritiedTranslationX
        {
            get
            {
                if (Parent != null)
                {
                    return TranslationX + Parent.InheritiedTranslationX;
                }

                return 0;
            }
        }

        public double InheritiedTranslationY
        {
            get
            {
                if (Parent != null)
                {
                    return TranslationY + Parent.InheritiedTranslationY;
                }

                return 0;
            }
        }

        public bool IsClippedToBounds
        {
            get
            {
                return _isClippedToBounds;
            }

            set
            {
                _isClippedToBounds = value;
                Invalidate();
            }
        }

        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }

            set
            {
                _isEnabled = value;
                Invalidate();
            }
        }

        public bool IsOpaque
        {
            get
            {
                return Math.Abs(Opacity - 1.0) < 0.0001;
            }
        }

        public bool IsScaled
        {
            get
            {
                return Math.Abs(Scale - 1.0) > 0.0001;
            }
        }

        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }

            set
            {
                _isVisible = value;
                Invalidate();
            }
        }

        public double Opacity
        {
            get
            {
                return _opacity;
            }

            set
            {
                _opacity = value;
                Invalidate();
            }
        }

        public bool IsPanning { get; private set; }


        public SkiaView Parent
        {
            get
            {
                return _parent;
            }

            internal set
            {
                _parent = value;

                if (value != null)
                {
                    _rootRenderer = _parent.RootRenderer;
                }

                UpdateRootRenderer();
            }
        }

        public Rectangle RenderBounds
        {
            get
            {
                return new Rectangle(
                    InheritedX + InheritiedTranslationX,
                    InheritedY + InheritiedTranslationY,
                    Width,
                    Height);
            }
        }

        public ISKiaRenderer RootRenderer { get; internal set; }

        public double Scale
        {
            get
            {
                return _scale;
            }

            set
            {
                _scale = value;
                Invalidate();
            }
        }

        public double TranslationX
        {
            get
            {
                return _translationX;
            }

            set
            {
                _translationX = value;
                Invalidate();
            }
        }

        public double TranslationY
        {
            get
            {
                return _translationY;
            }

            set
            {
                _translationY = value;
                Invalidate();
            }
        }

        public double Width
        {
            get
            {
                return _width;
            }

            set
            {
                _width = value;
                Invalidate();
            }
        }

        public double X
        {
            get
            {
                return _x;
            }

            set
            {
                _x = value;
                Invalidate();
            }
        }

        public double Y
        {
            get
            {
                return _y;
            }

            set
            {
                _y = value;
                Invalidate();
            }
        }

        public int ZPosition
        {
            get
            {
                return _zPosition;
            }

            set
            {
                _zPosition = value;
                Invalidate();
            }
        }

        public void BatchBegin()
        {
        }

        public void BatchCommit()
        {
            Invalidate();
        }

        public void Draw(SKCanvas canvas)
        {
            if (!IsVisible)
                return;

            using (var paint = new SKPaint())
            {
                int canvasState = int.MaxValue;

                var renderBounds = RenderBounds;

                if (!IsOpaque)
                {
                    paint.Color = new SKColor(255, 255, 255, (byte)(Opacity * 255));
                    canvasState = canvas.SaveLayer(paint);
                }
                else if (IsScaled || IsClippedToBounds)
                {
                    canvasState = canvas.Save();
                }

                if (IsScaled)
                {
                    canvas.Translate((float)renderBounds.Center.X, (float)renderBounds.Center.Y);
                    canvas.Scale((float)Scale, (float)Scale);
                    canvas.Translate((float)-renderBounds.Center.X, (float)-renderBounds.Center.Y);
                }

                if (IsClippedToBounds)
                {
                    canvas.ClipRect(renderBounds.ToSKRect());
                }

                InternalDraw(canvas);

                if (canvasState != int.MaxValue)
                {
                    canvas.RestoreToCount(canvasState);
                }
            }
        }

        public SizeRequest GetSizeRequest(double widthConstraint, double heightConstraint, double minimumWidth = -1, double minimumHeight = -1)
        {
            var s = SizeThatFits(widthConstraint, heightConstraint);
            var request = new Size(double.IsPositiveInfinity(s.Width) ? double.PositiveInfinity : s.Width, double.IsPositiveInfinity(s.Height) ? double.PositiveInfinity : s.Height);
            var minimum = new Size(minimumWidth < 0 ? request.Width : minimumWidth, minimumHeight < 0 ? request.Height : minimumHeight);
            return new SizeRequest(request, minimum);
        }

        public void Invalidate()
        {
            RootRenderer?.Invalidate(this);
        }

        public virtual void Layout()
        {
            foreach (var child in Children)
            {
                child.Layout();
            }
        }

        public void ProcessKey(Key key, bool status)
        {
        }

        public void ProcessKeyChar(char character)
        {
        }

        public void ProcessTouches(IEnumerable<TouchPoint> touches)
        {
            var filtered = touches.Where(x => (RenderBounds.Contains(x.Point) && x.HandledBy == null) || x.HandledBy == this);

            OnTouch(filtered.ToList());

            foreach (var child in _children)
            {
                child.ProcessTouches(touches);
            }
        }

        public void RemoveFromParent()
        {
            if (_parent != null)
            {
                _parent.Children.Remove(this);
            }
        }

        public virtual Size SizeThatFits(double widthConstraint, double heightConstraint)
        {
            return new Size(0, 0);
        }

        protected virtual void InternalDraw(SKCanvas canvas)
        {
            using (var paint = new SKPaint())
            {
                if (BackgroundColor.A > 0)
                {
                    paint.Color = BackgroundColor.ToSKColor();
                    canvas.DrawRect(RenderBounds.ToSKRect(), paint);
                }

                if (FrameColor.A > 0)
                {
                    paint.Color = FrameColor.ToSKColor();
                    paint.IsStroke = true;
                    paint.StrokeWidth = (float)FrameThickness;
                    canvas.DrawRect(RenderBounds.ToSKRect(), paint);
                }
            }

            foreach (var child in Children.OrderBy(x => x.ZPosition))
            {
                child.Draw(canvas);
            }
        }

        protected virtual void OnPanEnded(Point translation)
        {
        }

        protected virtual void OnPanStarted()
        {
        }

        protected virtual void OnPanUpdated(Point translation)
        {
        }

        protected virtual void OnTap()
        {
        }

        protected virtual void OnTouch(List<TouchPoint> touches)
        {
            if (touches.Count == 1)
            {
                switch (touches[0].Action)
                {
                    case TouchAction.Down:
                        _initialDown = touches[0].Point;
                        _downTime = DateTime.Now;
                        IsPanning = false;
                        break;

                    case TouchAction.Move:
                        if (IsPanning)
                        {
                            OnPanUpdated(new Point(_initialDown.X - touches[0].Point.X, _initialDown.Y - touches[0].Point.Y));
                        }

                        if (!IsPanning && (Math.Abs(_initialDown.X - touches[0].Point.X) >= PanThreshold || Math.Abs(_initialDown.Y - touches[0].Point.Y) > PanThreshold))
                        {
                            IsPanning = true;
                            OnPanStarted();
                        }
                        break;

                    case TouchAction.Up:

                        if (IsPanning)
                        {
                            IsPanning = false;
                            OnPanEnded(new Point(_initialDown.X - touches[0].Point.X, _initialDown.Y - touches[0].Point.Y));
                        }

                        if (!IsPanning && (DateTime.Now - _downTime).TotalMilliseconds < MaxTapTime)
                        {
                            OnTap();
                        }
                        break;
                }
            }
        }

        protected void UpdateRootRenderer()
        {
            if (Parent != null)
            {
                _rootRenderer = Parent.RootRenderer;
            }

            foreach (var children in Children)
            {
                children.UpdateRootRenderer();
            }
        }

        void _children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (SkiaView view in e.NewItems)
                {
                    view.Parent = this;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (SkiaView view in e.OldItems)
                {
                    view.Parent = null;
                }
            }
        }
    }
}