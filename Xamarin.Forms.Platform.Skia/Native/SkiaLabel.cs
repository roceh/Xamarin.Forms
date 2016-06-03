using SkiaSharp;
using System;

namespace Xamarin.Forms.Platform.Skia
{
    public class SkiaLabel : SkiaView
    {
        SKTypeface _cachedTypeFace;
        FontAttributes _cachedTypeFaceFontAttributes;
        string _cachedTypeFaceFontFamily;
        FontAttributes _fontAttributes = FontAttributes.None;
        string _fontFamily;
        double _fontSize = 10;
        TextAlignment _horizontalTextAlignment = TextAlignment.Start;
        string _text = String.Empty;
        Color _textColor = Color.Black;
        TextAlignment _verticalTextAlignment = TextAlignment.Start;

        public FontAttributes FontAttributes
        {
            get
            {
                return _fontAttributes;
            }

            set
            {
                _fontAttributes = value;
                Invalidate();
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
                _fontFamily = value;
                Invalidate();
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