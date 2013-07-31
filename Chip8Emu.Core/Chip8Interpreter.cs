using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chip8Emu.Core
{
    public class Chip8Interpreter
    {
        public const ushort FontSetAddress = 0x0000;
        public const ushort CodeAddress = 0x0200;

        public static byte[] FontSet = new byte[] 
        {
            0xF0, 0x90, 0x90, 0x90, 0xF0, // 0
            0x20, 0x60, 0x20, 0x20, 0x70, // 1
            0xF0, 0x10, 0xF0, 0x80, 0xF0, // 2
            0xF0, 0x10, 0xF0, 0x10, 0xF0, // 3
            0x90, 0x90, 0xF0, 0x10, 0x10, // 4
            0xF0, 0x80, 0xF0, 0x10, 0xF0, // 5
            0xF0, 0x80, 0xF0, 0x90, 0xF0, // 6
            0xF0, 0x10, 0x20, 0x40, 0x40, // 7
            0xF0, 0x90, 0xF0, 0x90, 0xF0, // 8
            0xF0, 0x90, 0xF0, 0x10, 0xF0, // 9
            0xF0, 0x90, 0xF0, 0x90, 0x90, // A
            0xE0, 0x90, 0xE0, 0x90, 0xE0, // B
            0xF0, 0x80, 0x80, 0x80, 0xF0, // C
            0xE0, 0x90, 0x90, 0x90, 0xE0, // D
            0xF0, 0x80, 0xF0, 0x80, 0xF0, // E
            0xF0, 0x80, 0xF0, 0x80, 0x80, // F
        };

        public event EventHandler Suspended;
        public event EventHandler Resumed;
        public event EventHandler Stepped;
         
        private Chip8Cpu _cpu;
        private byte[] _code;
        private bool _suspended = true;
        private Chip8Disassembler _disassembler;
        private int _counter = 0;

        public Chip8Interpreter(Chip8Cpu cpu, byte[] code)
        {
            this._cpu = cpu;
            Buffer.BlockCopy(FontSet, 0, cpu.Memory, FontSetAddress, FontSet.Length);
            Buffer.BlockCopy(code, 0, cpu.Memory, CodeAddress, code.Length);
            this._code = code;
            this._disassembler = new Chip8Disassembler(cpu.Memory);
        }

        public void Suspend()
        {
            _suspended = true;
            if (Suspended != null)
                Suspended(this, EventArgs.Empty);
        }

        public void Resume()
        {
            _suspended = false;
            if (Resumed != null)
                Resumed(this, EventArgs.Empty);
        }

        public void InterpetCode()
        {
            while (_cpu.ProgramCounter < _cpu.Memory.Length)
            {
                do
                {
                    _counter++;
                    if (_counter > 5)
                    {
                        Thread.Sleep(1);
                        _counter = 0;
                    }
                }
                while (_suspended);

                ExecuteNext();
            }
        }

        public void Step()
        {
            ExecuteNext();
            if (Stepped != null)
                Stepped(this, EventArgs.Empty);
        }

        private void ExecuteNext()
        {
            _cpu.DecrementTimers();
            var instruction = _disassembler.Disassemble(_cpu.ProgramCounter);
            _cpu.ProgramCounter += 2;
            instruction.Execute(_cpu);
        }

    }
}
