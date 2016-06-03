using System;
namespace Xamarin.Forms.Platform.Skia
{
    public class SkiaListSelectedItemEventArgs
    {
        public SkiaListSelectedItemEventArgs(int group, int row)
        {
            Group = group;
            Row = row;
        }

        public int Group { get; private set; }

        public int Row { get; private set; }
    }
}

