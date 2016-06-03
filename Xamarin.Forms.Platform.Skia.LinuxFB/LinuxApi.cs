using System;
using System.Runtime.InteropServices;

namespace Xamarin.Forms.Platform.Skia
{
    public class LinuxApi
    {
        [DllImport("libc", EntryPoint = "close", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int close(int fildes);

        [DllImport("libc", EntryPoint = "ioctl", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ioctl(int fd, uint cmd, ref fb_fix_screeninfo info);

        [DllImport("libc", EntryPoint = "ioctl", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ioctl(int fd, uint cmd, ref fb_var_screeninfo info);

        [DllImport("libc", EntryPoint = "ioctl", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ioctl(int fd, uint cmd, ref UInt32 info);

        [DllImport("libc", EntryPoint = "ioctl", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ioctl(int fd, uint cmd, UInt32 info);

        [DllImport("libc", EntryPoint = "mmap", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr mmap(IntPtr addr, uint len, ProtectionFlags prot, MMapFlags flags, int fd, int offset);

        [DllImport("libc", EntryPoint = "munmap", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int munmap(IntPtr addr, UInt32 length);

        [DllImport("libc", EntryPoint = "open", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int open(string path, int flag);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct fb_bitfield
        {
            public UInt32 offset;
            public UInt32 length;
            public UInt32 msb_right;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct fb_fix_screeninfo
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] id;
            public UInt32 smem_start;
            public UInt32 smem_len;
            public UInt32 type;
            public UInt32 type_aux;
            public UInt32 visual;
            public UInt16 xpanstep;
            public UInt16 ypanstep;
            public UInt16 ywrapstep;
            public UInt32 line_length;
            public UInt32 mmio_start;
            public UInt32 mmio_len;
            public UInt32 accel;
            public UInt16 capabilities;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public UInt16[] reserved;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct fb_var_screeninfo
        {
            public UInt32 xres;
            public UInt32 yres;
            public UInt32 xres_virtual;
            public UInt32 yres_virtual;
            public UInt32 xoffset;
            public UInt32 yoffset;
            public UInt32 bits_per_pixel;
            public UInt32 grayscale;
            public fb_bitfield red;
            public fb_bitfield green;
            public fb_bitfield blue;
            public fb_bitfield transp;
            public UInt32 nonstd;
            public UInt32 activate;
            public UInt32 height;
            public UInt32 width;
            public UInt32 accel_flags;
            public UInt32 pixclock;
            public UInt32 left_margin;
            public UInt32 right_margin;
            public UInt32 upper_margin;
            public UInt32 lower_margin;
            public UInt32 hsync_len;
            public UInt32 vsync_len;
            public UInt32 sync;
            public UInt32 vmode;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public UInt32[] reserved;
        };

        [Flags]
        public enum ProtectionFlags
        {
            PROT_NONE = 0,
            PROT_READ = 1,
            PROT_WRITE = 2,
            PROT_EXEC = 4
        }

        [Flags]
        public enum MMapFlags
        {
            MAP_FILE = 0,
            MAP_SHARED = 1,
            MAP_PRIVATE = 2,
            MAP_TYPE = 0xf,
            MAP_FIXED = 0x10,
            MAP_ANONYMOUS = 0x20,
            MAP_ANON = 0x20
        }

        public const uint KDSETMODE = 0x4B3A;
        public const uint KD_TEXT = 0x00;
        public const uint KD_GRAPHICS = 0x01;

        public const int O_ACCMODE = 00000003;
        public const int O_RDONLY = 00000000;
        public const int O_WRONLY = 00000001;
        public const int O_RDWR = 00000002;

        public const int FBIOGET_VSCREENINFO = 0x4600;
        public const int FBIOPUT_VSCREENINFO = 0x4601;
        public const int FBIOGET_FSCREENINFO = 0x4602;

        public const int FBIOPAN_DISPLAY = 0x4606;

        public const int IOC_NRBITS = 8;
        public const int IOC_TYPEBITS = 8;

        public const int IOC_SIZEBITS = 14;
        public const int IOC_DIRBITS = 2;

        public const uint IOC_NRMASK = (1 << IOC_NRBITS) - 1;
        public const uint IOC_TYPEMASK = (1 << IOC_TYPEBITS) - 1;
        public const uint IOC_SIZEMASK = (1 << IOC_SIZEBITS) - 1;
        public const uint IOC_DIRMASK = (1 << IOC_DIRBITS) - 1;

        public const int IOC_NRSHIFT = 0;
        public const int IOC_TYPESHIFT = IOC_NRSHIFT + IOC_NRBITS;
        public const int IOC_SIZESHIFT = IOC_TYPESHIFT + IOC_TYPEBITS;
        public const int IOC_DIRSHIFT = IOC_SIZESHIFT + IOC_SIZEBITS;

        public const uint IOC_NONE = 0;
        public const uint IOC_WRITE = 1;
        public const uint IOC_READ = 2;

        public static uint IOC(uint dir, uint type, uint nr, uint size)
        {
            return (dir << IOC_DIRSHIFT) |
                (type << IOC_TYPESHIFT) |
                (nr << IOC_NRSHIFT) |
                (size << IOC_SIZESHIFT);
        }

        public static uint IO(uint type, uint nr)
        {
            return IOC(IOC_NONE, type, nr, 0);
        }
        public static uint IOR(uint type, uint nr, uint size)
        {
            return IOC(IOC_READ, type, nr, size);
        }
        public static uint IOW(uint type, uint nr, uint size)
        {
            return IOC(IOC_WRITE, type, nr, size);
        }
        public static uint IOWR(uint type, uint nr, uint size)
        {
            return IOC(IOC_READ | IOC_WRITE, type, nr, size);
        }
        public static uint IOC_DIR(uint nr)
        {
            return (nr >> IOC_DIRSHIFT) & IOC_DIRMASK;
        }
        public static uint IOC_TYPE(uint nr)
        {
            return (nr >> IOC_TYPESHIFT) & IOC_TYPEMASK;
        }
        public static uint IOC_NR(uint nr)
        {
            return (nr >> IOC_NRSHIFT) & IOC_NRMASK;
        }
        public static uint IOC_SIZE(uint nr)
        {
            return (nr >> IOC_SIZESHIFT) & IOC_SIZEMASK;
        }

    }
}