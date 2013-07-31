using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chip8Emu.Core
{
    internal static class Extensions
    {
        public static byte GetHDigit(this ushort value, int index)
        {
            return (byte)(value >> (12 - (index * 4)) & 0xF);
        }

        public static byte GetLowerBits(this ushort value)
        {
            return (byte)(value & 0xFF);
        }

        public static byte GetHigherBits(this ushort value)
        {
            return (byte)((value & 0xFF00) >> 4);
        }

        public static ushort GetL3Digits(this ushort value)
        {
            return (ushort)(value & 0x0FFF);
        }
    }
}
