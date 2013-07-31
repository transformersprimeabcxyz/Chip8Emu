using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chip8Emu.Core
{
    public static class Chip8OpCodes
    {
        private static Random _random = new Random();

        public static readonly Chip8OpCode ClearScreen = new Chip8OpCode(Chip8OpCodeName.ClearScrean, 0x00E0, 0x0000, (cpu, value) => 
        { 
            cpu.Graphics.ClearScreen(); 
        });

        public static readonly Chip8OpCode Return = new Chip8OpCode(Chip8OpCodeName.Return, 0x00EE, 0x0000, (cpu, value) => 
        {
            cpu.ProgramCounter = cpu.CallStack.Pop();
        });

        public static readonly Chip8OpCode CallRca = new Chip8OpCode(Chip8OpCodeName.CallRca, 0x0000, 0x0FFF, (cpu, value) => { });


        public static readonly Chip8OpCode Jump = new Chip8OpCode(Chip8OpCodeName.Jump, 0x1000, 0x0FFF, (cpu, value) => 
        {
            cpu.ProgramCounter = value.GetL3Digits();
        });

        public static readonly Chip8OpCode Call = new Chip8OpCode(Chip8OpCodeName.Call, 0x2000, 0x0FFF, (cpu, value) => 
        {
            cpu.CallStack.Push(cpu.ProgramCounter);
            cpu.ProgramCounter = value.GetL3Digits();
        });

        public static readonly Chip8OpCode SkipIfVxEqN = new Chip8OpCode(Chip8OpCodeName.SkipifVxEqN, 0x3000, 0x0FFF, (cpu, value) => 
        {
            if (cpu.V[value.GetHDigit(1)] == value.GetLowerBits())
                cpu.ProgramCounter += 2;
        });

        public static readonly Chip8OpCode SkipIfVxNeqN = new Chip8OpCode(Chip8OpCodeName.SkipIfVxNeqN, 0x4000, 0x0FFF, (cpu, value) =>
        {
            if (cpu.V[value.GetHDigit(1)] != value.GetLowerBits())
                cpu.ProgramCounter += 2;
        });
       
        public static readonly Chip8OpCode SkipIfVxEqVy = new Chip8OpCode(Chip8OpCodeName.SkipIfVxEqVy, 0x5000, 0x0FF0, (cpu, value) =>
        {
            if (cpu.V[value.GetHDigit(1)] == cpu.V[value.GetHDigit(2)])
                cpu.ProgramCounter += 2;
        });
       
        public static readonly Chip8OpCode SetVxN = new Chip8OpCode(Chip8OpCodeName.SetVxN, 0x6000, 0x0FFF, (cpu, value) => 
        {
            cpu.V[value.GetHDigit(1)] = value.GetLowerBits();
        });
      
        public static readonly Chip8OpCode AddNToVx = new Chip8OpCode(Chip8OpCodeName.AddNToVx, 0x7000, 0x0FFF, (cpu, value) => 
        {
            cpu.V[value.GetHDigit(1)] += value.GetLowerBits();
        });
      
        public static readonly Chip8OpCode SetVxToVy = new Chip8OpCode(Chip8OpCodeName.SetVxToVy, 0x8000, 0x0FF0, (cpu, value) => 
        {
            cpu.V[value.GetHDigit(1)] = cpu.V[value.GetHDigit(2)];
        });
       
        public static readonly Chip8OpCode SetVxToVxOrVy = new Chip8OpCode(Chip8OpCodeName.SetVxToVxOrVy, 0x8001, 0x0FF0, (cpu, value) =>
        {
            cpu.V[value.GetHDigit(1)] |= cpu.V[value.GetHDigit(2)];
        });
       
        public static readonly Chip8OpCode SetVxToVxAndVy = new Chip8OpCode(Chip8OpCodeName.SetVxToVxAndVy, 0x8002, 0x0FF0, (cpu, value) =>
        {
            cpu.V[value.GetHDigit(1)] &= cpu.V[value.GetHDigit(2)];
        });
      
        public static readonly Chip8OpCode SetVxToVxXorVy = new Chip8OpCode(Chip8OpCodeName.SetVxToVxXorVy, 0x8003, 0x0FF0, (cpu, value) =>
        {
            cpu.V[value.GetHDigit(1)] ^= cpu.V[value.GetHDigit(2)];
        });
        
        public static readonly Chip8OpCode AddVyToVx = new Chip8OpCode(Chip8OpCodeName.AddVyToVx, 0x8004, 0x0FF0, (cpu, value) =>
        {
            // calculate result.
            ushort result = (ushort)(cpu.V[value.GetHDigit(1)] + cpu.V[value.GetHDigit(2)]);
            
            // set carry flag.
            cpu.V[0xF] = Convert.ToByte(result > byte.MaxValue);

            // safely set result.
            cpu.V[value.GetHDigit(1)] = (byte)(result & 0xFF);
        });
       
        public static readonly Chip8OpCode SubVyFromVx = new Chip8OpCode(Chip8OpCodeName.SubVyFromVx, 0x8005, 0x0FF0, (cpu, value) =>
        {
            // calculate result.
            short result = (short)(cpu.V[value.GetHDigit(1)] - cpu.V[value.GetHDigit(2)]);

            // set borrow flag.
            cpu.V[0xF] = Convert.ToByte(result >= 0);

            // safely set result.
            cpu.V[value.GetHDigit(1)] = (byte)(result & 0xFF);
        });
       
        public static readonly Chip8OpCode ShiftRightVx = new Chip8OpCode(Chip8OpCodeName.ShiftRightVx, 0x8006, 0x0FF0, (cpu, value) =>
        {
            // set flag to least significant bit
            cpu.V[0xF] = (byte)(cpu.V[value.GetHDigit(1)] & 1);

            cpu.V[value.GetHDigit(1)] >>= 1;
        });
       
        public static readonly Chip8OpCode SetVxToVyMinVx = new Chip8OpCode(Chip8OpCodeName.SetVxToVyMinVx, 0x8007, 0x0FF0, (cpu, value) =>
        {            
            // calculate result.
            short result = (short)(cpu.V[value.GetHDigit(2)] - cpu.V[value.GetHDigit(1)]);

            // set borrow flag.
            cpu.V[0xF] = Convert.ToByte(result >= 0);

            // safely set result.
            cpu.V[value.GetHDigit(1)] = (byte)(result & 0xFF);
        });
       
        public static readonly Chip8OpCode ShiftLeftVx = new Chip8OpCode(Chip8OpCodeName.ShiftLeftVx, 0x800E, 0x0FF0, (cpu, value) =>
        {
            // set flag to most significant bit
            cpu.V[0xF] = (byte)(cpu.V[value.GetHDigit(1)] >> 7);

            cpu.V[value.GetHDigit(1)] <<= 1;
        });
       
        public static readonly Chip8OpCode SkipIfVxNeqVy = new Chip8OpCode(Chip8OpCodeName.SkipIfVxNeqVy, 0x9000, 0x0FF0, (cpu, value) => 
        {
            if (cpu.V[value.GetHDigit(1)] != cpu.V[value.GetHDigit(2)])
                cpu.ProgramCounter += 2;
        });
       
        public static readonly Chip8OpCode SetIToAddr = new Chip8OpCode(Chip8OpCodeName.SetIToAddr, 0xA000, 0x0FFF, (cpu, value) => 
        {
            cpu.I = value.GetL3Digits();
        });
        
        public static readonly Chip8OpCode JumpPlusV0 = new Chip8OpCode(Chip8OpCodeName.JumpPlusV0, 0xB000, 0x0FFF, (cpu, value) => 
        {
            cpu.ProgramCounter = (uint)(value.GetL3Digits() + cpu.V[0]);
        });
       
        public static readonly Chip8OpCode SetVxRandomAndN = new Chip8OpCode(Chip8OpCodeName.SetVxRandomAndN, 0xC000, 0x0FFF, (cpu, value) =>
        {
            cpu.V[value.GetHDigit(1)] = (byte)(_random.Next(0, 256) & value.GetLowerBits());
        });
        
        public static readonly Chip8OpCode DrawSprite = new Chip8OpCode(Chip8OpCodeName.DrawSprite, 0xD000, 0x0FFF, (cpu, value) => 
        {
            bool setCarryFlag;
            cpu.Graphics.DrawSprite(cpu.V[value.GetHDigit(1)], cpu.V[value.GetHDigit(2)], cpu.ReadBytes(cpu.I, value.GetHDigit(3)), out setCarryFlag);

            cpu.V[0xF] = Convert.ToByte(setCarryFlag);
        });
       
        public static readonly Chip8OpCode SkipIfKeyPressed = new Chip8OpCode(Chip8OpCodeName.SkipIfKeyPressed, 0xE09E, 0x0F00, (cpu, value) => 
        {
            if (cpu.Input.IsKeyDown(cpu.V[value.GetHDigit(1)]))
                cpu.ProgramCounter += 2;
        });
       
        public static readonly Chip8OpCode SkipIfKeyNotPressed = new Chip8OpCode(Chip8OpCodeName.SkipIfKeyNotPressed, 0xE0A1, 0x0F00, (cpu, value) => 
        {
            if (!cpu.Input.IsKeyDown(cpu.V[value.GetHDigit(1)]))
                cpu.ProgramCounter += 2;
        });

        public static readonly Chip8OpCode SetVxToDelayValue = new Chip8OpCode(Chip8OpCodeName.SetVxToDelayValue, 0xF007, 0x0F00, (cpu, value) => 
        {
            cpu.V[value.GetHDigit(1)] = (byte)cpu.DelayTimer;
        });

        public static readonly Chip8OpCode AwaitKey = new Chip8OpCode(Chip8OpCodeName.AwaitKey, 0xF00A, 0x0F00, (cpu, value) => 
        {
            cpu.V[value.GetHDigit(1)] = (byte)cpu.Input.AwaitKey();
        });

        public static readonly Chip8OpCode SetDelayToVx = new Chip8OpCode(Chip8OpCodeName.SetDelayToVx, 0xF015, 0x0F00, (cpu, value) => 
        {
            cpu.DelayTimer = cpu.V[value.GetHDigit(1)];
        });

        public static readonly Chip8OpCode SetSoundToVx = new Chip8OpCode(Chip8OpCodeName.SetSoundToVx, 0xF018, 0x0F00, (cpu, value) =>
        {
            cpu.SoundTimer = cpu.V[value.GetHDigit(1)];
        });

        public static readonly Chip8OpCode AddVxToI = new Chip8OpCode(Chip8OpCodeName.AddVxToI, 0xF01E, 0x0F00, (cpu, value) => 
        {
            // calculate result.
            ushort result = (ushort)(cpu.I + cpu.V[value.GetHDigit(1)]);

            // set carry flag.
            cpu.V[0xF] = Convert.ToByte(result > 0xFFF);

            // set I.
            cpu.I = result;
        });

        public static readonly Chip8OpCode SetIToCharVx = new Chip8OpCode(Chip8OpCodeName.SetIToCharVx, 0xF029, 0x0F00, (cpu, value) => 
        {
            cpu.I = (ushort)(Chip8Interpreter.FontSetAddress + (5 * cpu.V[value.GetHDigit(1)]));
        });
      
        public static readonly Chip8OpCode StoreDecVxInMem = new Chip8OpCode(Chip8OpCodeName.StoreDecVxInMem, 0xF033, 0x0F00, (cpu, value) => 
        {
            byte valueToWrite = cpu.V[value.GetHDigit(1)];
            cpu.Memory[cpu.I] = (byte)(valueToWrite / 100);
            cpu.Memory[cpu.I + 1] =  (byte)((valueToWrite / 10) % 10);
            cpu.Memory[cpu.I + 2] = (byte)(valueToWrite % 10);
        });
      
        public static readonly Chip8OpCode StoreV0ToVxInMem = new Chip8OpCode(Chip8OpCodeName.StoreV0InMemPlusVx, 0xF055, 0x0F00, (cpu, value) => 
        {
            for (int i = 0; i < value.GetHDigit(1); i++)
            {
                cpu.Memory[cpu.I + i] = cpu.V[i];
            }

            //cpu.I += (ushort)(value.GetHDigit(1) + 1);
        });

        public static readonly Chip8OpCode FillV0ToVxWithMem = new Chip8OpCode(Chip8OpCodeName.FillV0ToVxWithMem, 0xF065, 0x0F00, (cpu, value) =>
        {
            for (int i = 0; i < value.GetHDigit(1); i++)
            {
                cpu.V[i] = cpu.Memory[cpu.I + i];
            }

            //cpu.I += (ushort)(value.GetHDigit(1) + 1);
        });

    
    }
}
