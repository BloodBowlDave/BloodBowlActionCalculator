using System.ComponentModel;

namespace ActionCalculator.Models
{
    [Flags]
    public enum CalculatorSkills : ulong
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
        [HideFromPlayerEditor]
        DivingTackle = 32768,
        [Description("CP")]
        [HideFromPlayerEditor]
        ConsummateProfessional = 65536,
        [Description("OP")]
        [HideFromPlayerEditor]
        OldPro = 131072,
        [Description("TB")]
        [HideFromPlayerEditor]
        TheBallista = 262144,
        [Description("MD")]
        [HideFromPlayerEditor]
        MesmerisingDance = 524288,
        [Description("R")]
        [HideFromPlayerEditor]
        Ram = 1048576,
        [Description("BB")]
        [HideFromPlayerEditor]
        BrutalBlock = 2097152,
        [Description("SM")]
        [HideFromPlayerEditor]
        SavageMauling = 4194304,
        [Description("CR")]
        [HideFromPlayerEditor]
        CrushingBlow = 8388608,
        [Description("S")]
        [HideFromPlayerEditor]
        Slayer = 16777216,
        [Description("BR")]
        [HideFromPlayerEditor]
        BlindRage = 67108864,
        [Description("BI")]
        [HideFromPlayerEditor]
        BlastIt = 134217728,
        [Description("WD")]
        [HideFromPlayerEditor]
        WhirlingDervish = 268435456,
        [Description("H")]
        Hatred = 536870912,
        [Description("LF")]
        LoneFouler = 1073741824,
        [Description("SB")]
        [HideFromPlayerEditor]
        SavageBlow = 2147483648,
        [Description("ASP")]
        [HideFromPlayerEditor]
        ASneakyPair = 4294967296,
        [Description("UM")]
        [HideFromPlayerEditor]
        UnstoppableMomentum = 8589934592,
        [Description("DS")]
        [HideFromPlayerEditor]
        DwarvenScourge = 17179869184,
        [Description("LC")]
        [HideFromPlayerEditor]
        LordOfChaos = 34359738368,
        [Description("HL")]
        [HideFromPlayerEditor]
        HalflingLuck = 68719476736
    }
}
