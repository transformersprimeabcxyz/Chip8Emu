using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chip8Emu.Core
{
    public enum Chip8OpCodeName
    {
        CallRca,
        ClearScrean,
        Return,
        Jump,
        Call,
        SkipifVxEqN,
        SkipIfVxNeqN,
        SkipIfVxEqVy,
        SetVxN,
        AddNToVx,
        SetVxToVy,
        SetVxToVxOrVy,
        SetVxToVxAndVy,
        SetVxToVxXorVy,
        AddVyToVx,
        SubVyFromVx,
        ShiftRightVx,
        SetVxToVyMinVx,
        ShiftLeftVx,
        SkipIfVxNeqVy,
        SetIToAddr,
        JumpPlusV0,
        SetVxRandomAndN,
        DrawSprite,
        SkipIfKeyPressed,
        SkipIfKeyNotPressed,
        SetVxToDelayValue,
        AwaitKey,
        SetDelayToVx,
        SetSoundToVx,
        AddVxToI,
        SetIToCharVx,
        StoreDecVxInMem,
        StoreV0InMemPlusVx,
        FillV0ToVxWithMem,
        Unknown,
    }
}
