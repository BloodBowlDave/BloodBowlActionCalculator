using System;
using System.ComponentModel;

namespace ActionCalculator.Abstractions
{
    [Flags]
    public enum Skills
    {
        None = 0,
        [Description("D")]//done
        Dodge = 1,
        [Description("SH")]//done
        SureHands = 2,
        [Description("SF")]//done
        SureFeet = 4,
        [Description("PA")]//not done
        Pass = 8,
        [Description("C")]//done
        Catch = 16,
        [Description("P")]//not done
        Pro = 32,
        [Description("L")]//done
        Loner = 64,
        [Description("DC")]//not done
        DivingCatch = 128,
        [Description("DP")]//not done
        DirtyPlayer = 256,
        [Description("MB")]//not done
        MightyBlow = 512,
        [Description("B")]//not done
        Brawler = 1024,
        [Description("CL")]//not done
        Claw = 2048,
        [Description("SG")]//not done
        SneakyGit = 4096
    }
}
