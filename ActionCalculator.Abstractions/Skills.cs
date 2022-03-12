using System;
using System.ComponentModel;

namespace ActionCalculator.Abstractions
{
    [Flags]
    public enum Skills : ulong
    {
        None = 0,
        [Description("D")]
        Dodge = 1,
        [Description("SH")]
        SureHands = 2,
        [Description("SF")]
        SureFeet = 4,
        [Description("PA")]
        Pass = 8,
        [Description("C")]
        Catch = 16,
        [Description("P")]
        Pro = 32,
        [Description("L")]
        Loner = 64,
        [Description("DC")]
        DivingCatch = 128,
        [Description("DP")]
        DirtyPlayer = 256,
        [Description("MB")]//not done
        MightyBlow = 512,
        [Description("B")]
        Brawler = 1024,
        [Description("CL")]//not done
        Claw = 2048,
        [Description("SG")]
        SneakyGit = 4096,
        [Description("DT")]//not done
        DivingTackle = 8192,
        [Description("CB")]
        CloudBurster = 16384,
        [Description("S")]
        Stunty = 32768
    }
}
