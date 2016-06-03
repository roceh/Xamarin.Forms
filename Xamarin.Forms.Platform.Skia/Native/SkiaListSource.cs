using System;
namespace Xamarin.Forms.Platform.Skia
{
    public class SkiaListSource
    {
        public SkiaListSource()
        {
        }

        public virtual SkiaView GetGroupHeader(SkiaView reuse, int group)
        {
            return null;
        }

        public virtual SkiaView GetCell(SkiaView reuse, int group, int row)
        {
            return null;
        }

        public virtual int GroupCount
        {
            get
            {
                return 0;
            }
        }

        public virtual bool ShowGroupHeaders
        {
            get
            {
                return false;
            }
        }

        public virtual int RowsForGroup(int group)
        {
            return 0;
        }
    }
}

