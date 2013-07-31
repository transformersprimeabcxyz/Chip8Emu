using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chip8Emu.Core
{
    public class Chip8Disassembler
    {
        private static Chip8OpCode[] _opcodes;

        static Chip8Disassembler()
        {
            List<Chip8OpCode> opcodes = new List<Chip8OpCode>();
            foreach (var field in typeof(Chip8OpCodes).GetFields())
            {
                if (field.FieldType == typeof(Chip8OpCode))
                {
                    opcodes.Add((Chip8OpCode)field.GetValue(null));
                }
            }
            _opcodes = opcodes.ToArray();
        }

        private byte[] _code;

        public Chip8Disassembler(byte[] data)
        {
            this._code = data;
        }

        public Chip8Instruction[] Disassemble()
        {
            List<Chip8Instruction> instructions = new List<Chip8Instruction>();
            for (uint i = 0; i < _code.Length; i += 2)
            {
                instructions.Add(Disassemble(i));
            }
            return instructions.ToArray();
        }

        public Chip8Instruction Disassemble(uint address)
        {
            ushort value = ReadUInt16(address);
            return new Chip8Instruction(address, MatchOpCode(value), value);
        }

        private Chip8OpCode MatchOpCode(ushort value)
        {
            foreach (var opcode in _opcodes)
            {
                if ((value & ~opcode.VariableMask) == opcode.Value)
                {
                    return opcode;
                }
            }
            return new Chip8OpCode(Chip8OpCodeName.Unknown, 0, 0, (c, v) => { });
//            throw new BadImageFormatException(string.Format("Invalid or not supported opcode {0:X4}.", value));
        }

        private ushort ReadUInt16(uint address)
        {
            return (ushort)(ReadByte(address) << 8 | ReadByte(address + 1));
        }

        private byte ReadByte(uint address)
        {
            return _code[address];
        }
    }
}
