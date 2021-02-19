using System;
using System.ComponentModel;

namespace ActionCalculator.Abstractions
{
    [Flags]
    public enum Skills
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
        [Description("F")]
        Frenzy = 512,
        [Description("MB")]
        MightyBlow = 1024,
        [Description("B")]
        Brawler = 2048,
        [Description("CL")]
        Claw = 4096,
        [Description("SG")]
        SneakyGit = 8192
    }
}
