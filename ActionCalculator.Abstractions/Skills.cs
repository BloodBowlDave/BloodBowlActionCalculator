using System.ComponentModel;

namespace ActionCalculator.Abstractions
{
    [Flags]
    public enum Skills : uint
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
        [Description("MB")]
        MightyBlow = 512,
        [Description("B")]
        Brawler = 1024,
        [Description("CL")]
        Claw = 2048,
        [Description("SG")]
        SneakyGit = 4096,
        [Description("DT")]
        DivingTackle = 8192,
        [Description("CB")]
        CloudBurster = 16384,
        [Description("BT")]
        BreakTackle = 32768,
        [Description("CP")]
        ConsummateProfessional = 65536,
        [Description("OP")]
        OldPro = 131072,
        [Description("TB")]
        TheBallista = 262144,
        [Description("MD")]
        MesmerisingDance = 524288,
        [Description("R")]
        Ram = 1048576,
        [Description("BB")]
        BrutalBlock = 2097152,
        [Description("SM")]
        SavageMauling = 4194304,
        [Description("CR")]
        CrushingBlow = 8388608,
        [Description("S")]
        Slayer = 16777216,
        [Description("I")]
        Incorporeal = 33554432
    }
}
