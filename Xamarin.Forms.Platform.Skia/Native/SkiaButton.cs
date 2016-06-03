using SkiaSharp;
using System;
using System.Collections.Generic;

namespace Xamarin.Forms.Platform.Skia
{
    public class SkiaButton : SkiaView
    {
        SKTypeface _cachedTypeFace;
        FontAttributes _cachedTypeFaceFontAttributes;
        string _cachedTypeFaceFontFamily;
        bool _down;
        FontAttributes _fontAttributes = FontAttributes.None;
        string _fontFamily;
        double _fontSize = 17;
        TextAlignment _horizontalTextAlignment = TextAlignment.Center;
        string _text = String.Empty;
        Color _textColor = Color.Black;
        TextAlignment _verticalTextAlignment = TextAlignment.Center;
        double _touchRadius;
        double _touchAlpha;

        public SkiaButton()
        {
            IsClippedToBounds = true;
        }

        public event EventHandler Clicked;

        public FontAttributes FontAttributes
        {
            get
            {
                return _fontAttributes;
            }

            set
            {
                _fontAttributes = value; Invalidate();
            }
        }

        public string FontFamily
        {
            get
            {
                return _fontFamily;
            }

            set
            {
                _fontFamily = value; Invalidate();
            }
        }

        public double FontSize
        {
            get
            {
                return _fontSize;
            }

            set
            {
                _fontSize = value;
                Invalidate();
            }
        }

        public TextAlignment HorizontalTextAlignment
        {
            get
            {
                return _horizontalTextAlignment;
            }

            set
            {
                _horizontalTextAlignment = value;
                Invalidate();
            }
        }

        public string Text
        {
            get
            {
                return _text;
            }

            set
            {
                _text = value;
                Invalidate();
            }
        }

        public Color TextColor
        {
            get
            {
                return _textColor;
            }

            set
            {
                _textColor = value;
                Invalidate();
            }
        }

        public TextAlignment VerticalTextAlignment
        {
            get
            {
                return _verticalTextAlignment;
            }

            set
            {
                _verticalTextAlignment = value;
                Invalidate();
            }
        }

        public override Size SizeThatFits(double widthConstraint, double heightConstraint)
        {
            if (!String.IsNullOrEmpty(Text))
            {
                float labelWidth, labelHeight;

                using (var paint = new SKPaint())
                {
                    paint.Typeface = GetTypeface();
                    paint.TextSize = (float)FontSize;
                    paint.IsAntialias = true;
                    labelWidth = paint.MeasureText(Text);
                    labelHeight = Math.Abs(paint.FontMetrics.Descent) + Math.Abs(paint.FontMetrics.Ascent);
                }

                return new Size(Double.IsPositiveInfinity(widthConstraint) ? labelWidth : Math.Min(labelWidth, widthConstraint), Double.IsPositiveInfinity(heightConstraint) ? labelHeight : Math.Min(labelHeight, heightConstraint));
            }

            return base.SizeThatFits(widthConstraint, heightConstraint);
        }

        protected override void InternalDraw(SKCanvas canvas)
        {
            base.InternalDraw(canvas);

            if (!String.IsNullOrEmpty(Text))
            {
                var renderBounds = RenderBounds;

                using (var paint = new SKPaint())
                {
                    paint.Typeface = GetTypeface();
                    paint.TextSize = (float)FontSize;
                    paint.IsAntialias = true;

                    if ((int)_touchAlpha > 0)
                    {
                        paint.Color = new SKColor(255, 255, 255, (byte)_touchAlpha);

                        var ovalSize = new Rectangle(renderBounds.Center.X - _touchRadius, renderBounds.Center.Y - Height - _touchRadius,
                                                     _touchRadius * 2, Height * 2 + _touchRadius * 2);

                        canvas.DrawOval(ovalSize.ToSKRect(), paint);
                    }

                    paint.Color = TextColor.ToSKColor();

                    var labelWidth = paint.MeasureText(Text);
                    var labelHeight = Math.Abs(paint.FontMetrics.Descent) + Math.Abs(paint.FontMetrics.Ascent);

                    double x = 0;

                    switch (HorizontalTextAlignment)
                    {
                        case TextAlignment.Start:
                            x = renderBounds.X;
                            break;

                        case TextAlignment.End:
                            x = renderBounds.Right - labelWidth;
                            break;

                        case TextAlignment.Center:
                            x = renderBounds.X + (renderBounds.Width / 2.0) - (labelWidth / 2.0);
                            break;
                    }

                    double y = 0;

                    switch (VerticalTextAlignment)
                    {
                        case TextAlignment.Start:
                            y = renderBounds.Top + labelHeight;
                            break;

                        case TextAlignment.End:
                            y = renderBounds.Bottom;
                            break;

                        case TextAlignment.Center:
                            y = renderBounds.Y + (renderBounds.Height / 2.0) + (labelHeight / 2.0);
                            break;
                    }

                    canvas.DrawText(Text, (float)x, (float)y - paint.FontMetrics.Descent, paint);
                }
            }
        }

        protected override void OnTouch(List<TouchPoint> touches)
        {
            foreach (var touch in touches)
            {
                switch (touch.Action)
                {
                    case TouchAction.Down:
                        {
                            _down = true;
                            touch.HandledBy = this;
                        }
                        break;

                    case TouchAction.Up:
                        {
                            if (_down && touch.HandledBy == this)
                            {
                                Clicked(this, new EventArgs());

                                var animationRadius = new Animation((d) => _touchRadius = d, 25, Math.Max(25, Width), Easing.SinOut);
                                var animationAlpha = new Animation((d) => _touchAlpha = d, 192, 0.0, Easing.CubicInOut);
                                var combined = new Animation();
                                combined.Add(0, 1, animationRadius);
                                combined.Add(0, 1, animationAlpha);
                                combined.Commit(this, "TouchAnimation", length: 500);

                                _down = false;
                            }
                        }
                        break;
                }
            }
        }

        SKTypeface GetTypeface()
        {
            if (_cachedTypeFace == null || _cachedTypeFaceFontFamily != FontFamily || _cachedTypeFaceFontAttributes != FontAttributes)
            {
                SKTypefaceStyle style = SKTypefaceStyle.Normal;

                switch (FontAttributes)
                {
                    case FontAttributes.None:
                        style = SKTypefaceStyle.Normal;
                        break;

                    case FontAttributes.Bold:
                        style = SKTypefaceStyle.Bold;
                        break;

                    case FontAttributes.Italic:
                        style = SKTypefaceStyle.Italic;
                        break;
                }

                _cachedTypeFace = SKTypeface.FromFamilyName(FontFamily == null ? "Arial" : FontFamily, style);

                _cachedTypeFaceFontFamily = FontFamily;
                _cachedTypeFaceFontAttributes = FontAttributes;
            }

            return _cachedTypeFace;
        }
    }
}