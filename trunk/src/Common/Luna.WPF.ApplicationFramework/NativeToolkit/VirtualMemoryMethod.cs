using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Luna.NativeToolkit
{
    public static class VirtualMemoryMethod
    {
        /// <summary>
        /// Reserves or commits a region of pages in the virtual address space of the calling process
        /// </summary>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr VirtualAlloc(IntPtr lpAddress, UIntPtr dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualFree(IntPtr lpAddress, UIntPtr dwSize,uint dwFreeType);

        [DllImport("kernel32.dll")]
        public static extern bool VirtualLock(IntPtr lpAddress, UIntPtr dwSize);

        [DllImport("kernel32.dll")]
        public static extern bool VirtualUnlock(IntPtr lpAddress, UIntPtr dwSize);
    }

    [Flags()]
    public enum AllocationType : uint
    {
        COMMIT = 0x1000,
        RESERVE = 0x2000,
        RESET = 0x80000,
        LARGE_PAGES = 0x20000000,
        PHYSICAL = 0x400000,
        TOP_DOWN = 0x100000,
        WRITE_WATCH = 0x200000
    }

    [Flags()]
    public enum MemoryProtection : uint
    {
        EXECUTE = 0x10,
        EXECUTE_READ = 0x20,
        EXECUTE_READWRITE = 0x40,
        EXECUTE_WRITECOPY = 0x80,
        NOACCESS = 0x01,
        READONLY = 0x02,
        READWRITE = 0x04,
        WRITECOPY = 0x08,
        GUARD_Modifierflag = 0x100,
        NOCACHE_Modifierflag = 0x200,
        WRITECOMBINE_Modifierflag = 0x400
    }
}
