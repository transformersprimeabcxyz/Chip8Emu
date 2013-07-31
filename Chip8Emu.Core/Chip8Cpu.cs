using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chip8Emu.Core
{
    public class Chip8Cpu
    {
        public Chip8Cpu(IGraphicsDevice graphicsDevice, IInputDevice inputDevice)
        {
            Graphics = graphicsDevice;
            Input = inputDevice;
        }

        public uint ProgramCounter = 0x200;
        public byte[] V = new byte[16];
        public ushort I = 0;
        public Stack<uint> CallStack = new Stack<uint>();
        public byte[] Memory = new byte[0x1000];
        public IGraphicsDevice Graphics;
        public IInputDevice Input;
        public byte DelayTimer;
        public byte SoundTimer;

        public byte[] ReadBytes(int address, int count)
        {
            byte[] bytes = new byte[count];
            Buffer.BlockCopy(Memory, address, bytes, 0, count);
            return bytes;
        }

        public void DecrementTimers()
        {
            if (DelayTimer > 0)
                DelayTimer--;
            if (SoundTimer > 0)
                SoundTimer--;
        }
    }
}
