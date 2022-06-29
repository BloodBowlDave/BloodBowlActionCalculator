using System.ComponentModel;

namespace ActionCalculator.Models
{
    [Flags]
    public enum Skills : uint
    {
        None = 0,
        [Description("D")]
        Dodge = 1,
        [Description("BT")]
        BreakTackle = 2, 
        [Description("SH")]
        SureHands = 4,
        [Description("SF")]
        SureFeet = 8,
        [Description("PA")]
        Pass = 16,
        [Description("CB")]
        CloudBurster = 32,
        [Description("C")]
        Catch = 64,
        [Description("DC")]
        DivingCatch = 128,
        [Description("B")]
        Brawler = 256,
        [Description("CL")]
        Claw = 512,
        [Description("MB")]
        MightyBlow = 1024,
        [Description("P")]
        Pro = 2048,
        [Description("L")]
        Loner = 4096,
        [Description("DP")]
        DirtyPlayer = 8192, 
        [Description("SG")]
        SneakyGit = 16384,
        [Description("DT")]
        DivingTackle = 32768,
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
        Incorporeal = 33554432,
        [Description("BR")]
        BlindRage = 67108864,
        [Description("BI")]
        BlastIt = 134217728,
        [Description("WD")]
        WhirlingDervish = 268435456
    }
}
