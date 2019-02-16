using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DartboardEngine.Models.Structs
{
    public static class StructMarshaller
    {
        public static byte[] Encode<T>(T item) where T: struct
        {
            int size = Marshal.SizeOf(item);
            byte[] arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(item, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        public static T Decode<T>(byte[] arr) where T: struct
        {
            T item = default(T);

            int size = Marshal.SizeOf(item);
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(arr, 0, ptr, size);

            item = (T)Marshal.PtrToStructure(ptr, item.GetType());
            Marshal.FreeHGlobal(ptr);

            return item;
        }
    }
}
