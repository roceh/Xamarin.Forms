using System.Collections;

namespace Xamarin.Forms.Platform.Skia
{
    public class ListViewRendererSource : SkiaListSource
    {
        ListView _listView;

        public ListViewRendererSource(ListView listView)
        {
            _listView = listView;
        }

        public override int GroupCount
        {
            get
            {
                return _listView.IsGroupingEnabled ? (_listView.ItemsSource as IList).Count : 1;
            }
        }

        public override bool ShowGroupHeaders
        {
            get
            {
                return _listView.IsGroupingEnabled;
            }
        }

        public override SkiaView GetGroupHeader(SkiaView reuse, int group)
        {
            if (_listView.IsGroupingEnabled && _listView.GroupHeaderTemplate != null)
            {
                if (reuse == null)
                {
                    var cell = _listView.GroupHeaderTemplate.CreateContent() as ViewCell;

                    var collection = (_listView.ItemsSource as IList);

                    cell.BindingContext = collection[group];                  

                    cell.Parent = _listView;

                    var renderer = Platform.GetRenderer(cell.View);

                    if (renderer == null)
                    {
                        renderer = Platform.CreateRenderer(cell.View);
                        Platform.SetRenderer(cell.View, renderer);
                    }

                    var holder = new ViewCellHolder(cell, renderer.NativeView);

                    return holder;
                }
                else
                {
                    var holder = reuse as ViewCellHolder;

                    var collection = (_listView.ItemsSource as IList);
                                       
                    holder.ViewCell.BindingContext = collection[group];

                    return holder;
                }
            }

            return null;
        }


        public override SkiaView GetCell(SkiaView reuse, int group, int row)
        {
            if (reuse == null)
            {
                var cell = _listView.ItemTemplate.CreateContent() as ViewCell;

                var collection = (_listView.ItemsSource as IList);

                if (_listView.IsGroupingEnabled)
                {
                    var subCollection = collection[group] as IList;
                    cell.BindingContext = subCollection[row];
                }
                else
                {
                    cell.BindingContext = collection[row];
                }

                cell.Parent = _listView;

                var renderer = Platform.GetRenderer(cell.View);

                if (renderer == null)
                {
                    renderer = Platform.CreateRenderer(cell.View);
                    Platform.SetRenderer(cell.View, renderer);
                }

                var holder = new ViewCellHolder(cell, renderer.NativeView);

                return holder;
            }
            else
            {
                var holder = reuse as ViewCellHolder;

                var collection = (_listView.ItemsSource as IList);

                if (_listView.IsGroupingEnabled)
                {
                    var subCollection = collection[group] as IList;
                    holder.ViewCell.BindingContext = subCollection[row];
                }
                else
                {
                    holder.ViewCell.BindingContext = collection[row];
                }

                return holder;
            }
        }


        public override int RowsForGroup(int group)
        {
            var collection = (_listView.ItemsSource as IList);

            if (collection != null)
            {
                if (_listView.IsGroupingEnabled)
                {
                    return (collection[group] as IList).Count;
                }

                return collection.Count;
            }

            return 0;
        }

        public class ViewCellHolder : SkiaView
        {
            public ViewCellHolder(ViewCell viewCell, SkiaView nativeView)
            {
                ViewCell = viewCell;
                Children.Add(nativeView);
            }

            public ViewCell ViewCell { get; private set; }

            public override void Layout()
            {
                base.Layout();

                ViewCell.View.Layout(new Rectangle(0, 0, Width, Height));
            }

            public override Size SizeThatFits(double widthConstraint, double heightConstraint)
            {
                return Children[0].SizeThatFits(widthConstraint, heightConstraint);
            }
        }
    }
}