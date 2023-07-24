using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices; 

namespace ezTools
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Union_UShort
    {
        [FieldOffset(0)]
        public ushort m_ushort;

        [FieldOffset(0)]
        public byte m_byte0;

        [FieldOffset(1)]
        public byte m_byte1;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct Union_UInt
    {
        [FieldOffset(0)]
        public uint m_uint;

        [FieldOffset(0)]
        public byte m_byte0;

        [FieldOffset(1)]
        public byte m_byte1;

        [FieldOffset(2)]
        public byte m_byte2;

        [FieldOffset(3)]
        public byte m_byte3;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct Union_Double
    {
        [FieldOffset(0)]
        public double m_double;

        [FieldOffset(0)]
        public byte m_byte0;

        [FieldOffset(1)]
        public byte m_byte1;

        [FieldOffset(2)]
        public byte m_byte2;

        [FieldOffset(3)]
        public byte m_byte3;

        [FieldOffset(4)]
        public byte m_byte4;

        [FieldOffset(5)]
        public byte m_byte5;

        [FieldOffset(6)]
        public byte m_byte6;

        [FieldOffset(7)]
        public byte m_byte7;
    }
}
