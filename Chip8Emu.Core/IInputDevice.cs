using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chip8Emu.Core
{
    public interface IInputDevice
    {
        bool IsKeyDown(int key);
        int AwaitKey();
    }
}
