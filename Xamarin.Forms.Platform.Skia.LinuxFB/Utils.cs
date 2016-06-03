using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Xamarin.Forms.Platform.Skia
{
    public static class Utils
    {
        public static T BytesToStruct<T>(byte[] rawData, int position) where T : struct
        {
            int rawsize = Marshal.SizeOf(typeof(T));
            IntPtr buffer = Marshal.AllocHGlobal(rawsize);
            Marshal.Copy(rawData, position, buffer, rawsize);
            T retobj = (T)Marshal.PtrToStructure<T>(buffer);
            Marshal.FreeHGlobal(buffer);
            return retobj;
        }
    }
}
