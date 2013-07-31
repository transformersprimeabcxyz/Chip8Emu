using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chip8Emu.Core
{
    public class Chip8Instruction
    {
        public Chip8Instruction(uint offset, Chip8OpCode code, ushort value)
        {
            Offset = offset;
            OpCode = code;
            Value = value;
        }

        public uint Offset;
        public Chip8OpCode OpCode;
        public ushort Value;

        public void Execute(Chip8Cpu cpu)
        {
            OpCode.Action(cpu, Value);
        }
    }
}
