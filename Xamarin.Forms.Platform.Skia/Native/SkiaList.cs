using SkiaSharp;
using System;
using System.Collections.Generic;

namespace Xamarin.Forms.Platform.Skia
{
    public class SkiaList : SkiaView
    {
        List<SkiaView> _cellCache = new List<SkiaView>();
        Dictionary<CellInfo, double> _cellHeightCache = new Dictionary<CellInfo, double>();
        List<SkiaView> _groupHeaderCache = new List<SkiaView>();
        SkiaView _groupHeaderHeightCache;
        SkiaView _heightCache;
        DateTime _lastMeasure;
        double _panStartPosition;
        double _pointAtLastMeasure;
        double _position;
        CellInfo _selectedItem;
        DateTime _startMeasure;
        double _velocity;
        List<CellInfo> _visibleCells = new List<CellInfo>();

        public SkiaList()
        {
            IsClippedToBounds = true;
            _selectedItem.Row = -1;
            _selectedItem.Group = -1;
        }

        public event EventHandler<SkiaListSelectedItemEventArgs> Selected;

        public bool HasUnevenRows { get; set; }

        public double Position
        {
            get
            {
                return _position;
            }

            private set
            {
                _position = Math.Min(TotalContentHeight - Height, Math.Max(0, value));

                Layout();
            }
        }

        public double RowHeight { get; set; }

        public CellInfo SelectedItem
        {
            get
            {
                return _selectedItem;
            }
        }

        public SkiaListSource Source { get; set; }

        public double TotalContentHeight
        {
            get
            {
                if (Source != null)
                {
                    if (HasUnevenRows)
                    {
                        double total = 0;

                        for (int g = 0; g < Source.GroupCount; g++)
                        {
                            var rowForGroup = Source.RowsForGroup(g);

                            total += HeightForGroupHeader(g);

                            for (int r = 0; r < rowForGroup; r++)
                            {
                                total += HeightForCell(g, r);
                            }
                        }

                        return total;
                    }
                    else
                    {
                        double total = 0;

                        for (int g = 0; g < Source.GroupCount; g++)
                        {
                            total += HeightForGroupHeader(g) + (Source.RowsForGroup(g) * RowHeight);
                        }

                        return total;
                    }
                }

                return 0;
            }
        }

        public override void Layout()
        {
            if (Source != null)
            {
                var item = PixelYToInfo(Position);

                var rowsForGroup = Source.RowsForGroup(item.Group);

                var yPosition = item.YPosition;
                var row = item.Row;
                var group = item.Group;

                _visibleCells.Clear();
                Children.Clear();

                int groupHeaderReuse = 0;
                int cellReuse = 0;

                int _cellCacheCount = _cellCache.Count;
                int _groupHeaderCacheCount = _groupHeaderCache.Count;

                while (yPosition - Position < Height && group < Source.GroupCount)
                {
                    if (row == -1)
                    {
                        if (Source.ShowGroupHeaders)
                        {
                            var cell = Source.GetGroupHeader(groupHeaderReuse < _groupHeaderCacheCount ? _groupHeaderCache[groupHeaderReuse++] : null, group);

                            if (!_groupHeaderCache.Contains(cell))
                            {
                                _groupHeaderCache.Add(cell);
                            }

                            var height = HasUnevenRows ? cell.SizeThatFits(Width, Double.PositiveInfinity).Height : RowHeight;

                            cell.Bounds = new Rectangle(0, yPosition - Position, Width, height);

                            _visibleCells.Add(new CellInfo(group, row));
                            Children.Add(cell);

                            yPosition += height;
                        }

                        row++;
                    }

                    if (row < rowsForGroup)
                    {
                        var cell = Source.GetCell(cellReuse < _cellCacheCount ? _cellCache[cellReuse++] : null, group, row);

                        if (!_cellCache.Contains(cell))
                        {
                            _cellCache.Add(cell);
                        }

                        var height = HasUnevenRows ? cell.SizeThatFits(Width, Double.PositiveInfinity).Height : RowHeight;

                        cell.Bounds = new Rectangle(0, yPosition - Position, Width, height);

                        _visibleCells.Add(new CellInfo(group, row));
                        Children.Add(cell);

                        yPosition += height;
                    }

                    row++;

                    if (row > rowsForGroup)
                    {
                        group++;
                        row = -1;
                        rowsForGroup = Source.RowsForGroup(group);
                    }
                }
            }

            base.Layout();
        }

        public void SelectItem(CellInfo item)
        {
            _selectedItem = item;
            Invalidate();
        }

        protected override void InternalDraw(SkiaSharp.SKCanvas canvas)
        {
            var selectedVisible = _visibleCells.IndexOf(_selectedItem);

            if (selectedVisible != -1)
            {
                var bounds = Children[selectedVisible].RenderBounds;

                using (var paint = new SKPaint())
                {
                    paint.Color = new SKColor(225, 225, 225);
                    canvas.DrawRect(bounds.ToSKRect(), paint);
                }
            }

            base.InternalDraw(canvas);
        }

        protected override void OnPanEnded(Point translation)
        {
            base.OnPanEnded(translation);

            Position = _panStartPosition + translation.Y;

            // total time pan took
            var totalTimespan = (DateTime.Now - _startMeasure).TotalSeconds;

            // short flick - just use the total pan time and movement to work out velocity
            if (totalTimespan < 0.5 || _lastMeasure == _startMeasure)
            {
                _velocity = (Position - _panStartPosition) / totalTimespan;
            }

            // last time we did velocity calculation
            var timespan = (DateTime.Now - _lastMeasure).TotalSeconds;

            // velocity must be at least greater than this to animate flick
            if (Math.Abs(_velocity) > 25 && timespan < 0.5)
            {
                // continue with a flick animation for a couple of seconds
                var animation = new Animation((d) => Position = d, Position, Position + _velocity, Easing.CubicOut);
                animation.Commit(this, "FlickAnimation", length: 2000);
            }
        }

        protected override void OnPanStarted()
        {
            base.OnPanStarted();

            // for velocity calculation
            _startMeasure = DateTime.Now;
            _lastMeasure = DateTime.Now;
            _panStartPosition = Position;
            _pointAtLastMeasure = Position;
            _velocity = 0.0;
        }

        protected override void OnPanUpdated(Point translation)
        {
            base.OnPanUpdated(translation);

            Position = _panStartPosition + translation.Y;

            var timespan = (DateTime.Now - _lastMeasure).TotalSeconds;

            // enough time to calculate velocity ?
            if (timespan > 0.25)
            {
                var v = (Position - _pointAtLastMeasure) / timespan;

                // smooth out velocity
                _velocity = 0.8f * v + 0.2f * _velocity;

                _lastMeasure = DateTime.Now;
                _pointAtLastMeasure = Position;
            }
        }

        protected override void OnTouch(List<TouchPoint> touches)
        {
            if (touches.Count == 1)
            {
                switch (touches[0].Action)
                {
                    case TouchAction.Down:
                        AnimationExtensions.AbortAnimation(this, "FlickAnimation");
                        break;

                    case TouchAction.Move:
                        break;

                    case TouchAction.Up:
                        if (!IsPanning)
                        {
                            for (int i = 0; i < Children.Count; i++)
                            {
                                if (Children[i].RenderBounds.Contains(touches[0].Point))
                                {
                                    _selectedItem = _visibleCells[i];

                                    Invalidate();

                                    Selected?.Invoke(this, new SkiaListSelectedItemEventArgs(_visibleCells[i].Group, _visibleCells[i].Row));
                                    break;
                                }
                            }
                        }
                        break;
                }
            }

            base.OnTouch(touches);
        }

        private double HeightForCell(int group, int row)
        {
            if (HasUnevenRows)
            {
                double rowHeight;

                if (!_cellHeightCache.TryGetValue(new CellInfo(group, row), out rowHeight))
                {
                    _heightCache = Source.GetCell(_heightCache, group, row);

                    rowHeight = _heightCache.SizeThatFits(Width, Double.PositiveInfinity).Height;

                    _cellHeightCache.Add(new CellInfo(group, row), rowHeight);
                }

                return rowHeight;
            }

            return RowHeight;
        }

        private double HeightForGroupHeader(int group)
        {
            if (Source.ShowGroupHeaders)
            {
                if (HasUnevenRows)
                {
                    double rowHeight;

                    if (!_cellHeightCache.TryGetValue(new CellInfo(group, -1), out rowHeight))
                    {
                        _groupHeaderHeightCache = Source.GetGroupHeader(_groupHeaderHeightCache, group);
                        rowHeight = _groupHeaderHeightCache != null ? _groupHeaderHeightCache.Height : 0;
                        _cellHeightCache.Add(new CellInfo(group, -1), rowHeight);
                    }

                    return rowHeight;
                }

                return RowHeight;
            }

            return 0;
        }

        private SkiaListItemInfo PixelYToInfo(double y)
        {
            var result = new SkiaListItemInfo { Group = 0, Row = 0, YPosition = 0 };

            double totalY = 0;

            for (int g = 0; g < Source.GroupCount; g++)
            {
                var headerHeight = HeightForGroupHeader(g);

                result.Group = g;
                result.Row = -1;
                result.YPosition = totalY;

                if (totalY + headerHeight > y)
                {
                    return result;
                }

                totalY += headerHeight;

                for (int r = 0; r < Source.RowsForGroup(g); r++)
                {
                    var cellHeight = HeightForCell(g, r);

                    if (totalY + cellHeight > y)
                    {
                        return result;
                    }

                    result.Group = g;
                    result.Row = r;
                    result.YPosition = totalY;

                    totalY += cellHeight;
                }
            }

            return result;
        }

        public struct CellInfo
        {
            public CellInfo(int group, int row)
            {
                Group = group;
                Row = row;
            }

            public int Group { get; set; }

            public int Row { get; set; }
        }

        public struct SkiaListItemInfo
        {
            public int Group;
            public int Row;
            public double YPosition;
        }
    }
}