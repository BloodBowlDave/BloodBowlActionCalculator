namespace ActionCalculator.Utilities
{
    public static class IntegerExtensions
    {
        public static int NormaliseD6(this int i) => i.ThisOrMinimum(2).ThisOrMaximum(6);
        public static int Normalise2D6(this int i) => i.ThisOrMinimum(2).ThisOrMaximum(12);
        public static int Normalise3D6(this int i) => i.ThisOrMinimum(3).ThisOrMaximum(18);
    }
}
