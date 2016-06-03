using System;

namespace Xamarin.Forms.Platform.Skia
{
    public class SkiaListCellRequestEventArgs : EventArgs
    {
        public SkiaListCellRequestEventArgs(int group, int row)
        {
            Group = group;
            Row = row;
        }

        public SkiaView CellView { get; set; }

        public int Group { get; private set; }

        public int Row { get; private set; }
    }
}