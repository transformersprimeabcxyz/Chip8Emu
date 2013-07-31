
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chip8Emu.Core
{
    public struct Chip8OpCode
    {
        public Chip8OpCode(Chip8OpCodeName name, ushort value, ushort variableMask, Action<Chip8Cpu, ushort> action)
        {
            Name = name;
            Value = value;
            VariableMask = variableMask;
            Action = action;
        }

        public Chip8OpCodeName Name;
        public ushort Value;
        public ushort VariableMask;
        public Action<Chip8Cpu, ushort> Action;

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
