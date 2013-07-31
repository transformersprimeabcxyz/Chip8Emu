using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chip8Emu.Core
{
    public interface IGraphicsDevice
    {
        void ClearScreen();
        void DrawSprite(byte x, byte y, byte[] rows, out bool setCarryFlag);

    }
}
