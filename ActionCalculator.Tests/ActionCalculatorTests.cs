using ActionCalculator.Abstractions;
using ActionCalculator.Strategies;
using ActionCalculator.Utilities;
using Xunit;

namespace ActionCalculator.Tests
{
    public class ActionCalculatorTests
    {
        private readonly IActionCalculator _actionCalculator;

        public ActionCalculatorTests()
        {
            _actionCalculator = new ActionCalculator(
                new CalculationBuilder(
                    new ActionBuilder(new D6()),
                    new PlayerBuilder()),
                new ProbabilityComparer(),
                new ActionMediator(
                    new ActionStrategyFactory()));
        }

        [Theory]
        //rerollable
        [InlineData("2", 1, 0.83333, 0.97222)]
        [InlineData("2,2", 2, 0.69444, 0.92593, 0.94522)]
        [InlineData("2,3,4,5,6", 5, 0.01543, 0.05401, 0.09045, 0.10652, 0.10979, 0.11003)]
        [InlineData("2,3;4,5,6", 5, 0.01543, 0.05401, 0.09045, 0.10652, 0.10979, 0.11003)]
        [InlineData("5", 1, 0.33333, 0.55556)]
        //alternate branches
        [InlineData("(X1/2)(X1/4)(X1/4)", 0, 1.00000)]
        [InlineData("(X1/2,2)(X1/4,3)(X1/4,4)", 0, 0.70833)]
        [InlineData("2(X1/2)(X1/4)(X1/4)2", 2, 0.69444, 0.92593, 0.94522)]
        //block
        [InlineData("-2D2,2,2D3", 3, 0.06944, 0.16011, 0.18872, 0.19129)]
        [InlineData("-3D1", 1, 0.00463, 0.00924)]
        [InlineData("1D5", 1, 0.83333, 0.97222)]
        [InlineData("2D3,B8", 0, 0.31250)]
        [InlineData("2D3,B8,J8", 0, 0.13021)]
        [InlineData("2D5", 1, 0.97222, 0.99923)]
        [InlineData("3D1", 1, 0.42130, 0.66510)]
        [InlineData("P:3D1", 0, 0.48560)]
        [InlineData("B,P:1D1", 0, 0.26852)]
        [InlineData("B,P:1D1", 1, 0.19444, 0.30556)]
        [InlineData("B,P:1D1^", 1, 0.19444, 0.30556)]
        [InlineData("B,P:1D1^*", 0, 0.26852)]
        [InlineData("B,P:3D2", 0, 0.81396)]
        [InlineData("B,P:3D2", 1, 0.70370, 0.91221)]
        [InlineData("B,P:3D2^*", 0, 0.81396)]
        //frenzy
        [InlineData("1D3!2{1D3}", 0, 0.66667)]
        [InlineData("1D3!2{1D3}", 1, 0.50000, 0.83333)]
        [InlineData("2D2!2{1D2}", 0, 0.66667)]
        [InlineData("2D2!2{1D2}", 1, 0.55556, 0.85185)]
        [InlineData("2D2!2'{1D2}", 1, 0.66667, 0.81481)]
        [InlineData("1D3!2'{1D3}", 1, 0.66667, 0.86111)]
        [InlineData("2D2!2{-2D2}", 1, 0.55556, 0.81893)]
        [InlineData("2D2!2'{-2D2}", 1, 0.59259, 0.69136)]
        [InlineData("-2D2!2{1D2}", 1, 0.11111, 0.30864)]
        [InlineData("-2D2!2'{1D2}", 1, 0.22222, 0.41975)]
        [InlineData("P:2D2!2{1D2}", 1, 0.55556, 0.87380)]
        [InlineData("P:2D2!2*{1D2}", 1, 0.74897, 0.81207)]
        [InlineData("P:2D2!2*'{1D2}", 1, 0.69959, 0.77915)]
        //brawler
        [InlineData("B,P:-2D2", 0, 0.23663)]
        [InlineData("B,P:-2D2", 1, 0.23663, 0.26440)]
        [InlineData("B,P:-2D2^*", 1, 0.23663, 0.26440)]
        [InlineData("B,P:-3D3", 1, 0.30556, 0.34201)]
        [InlineData("B:-2D2", 1, 0.14815, 0.23457)]
        [InlineData("B:1D1", 0, 0.19444)]
        [InlineData("B:1D1", 1, 0.19444, 0.30556)]
        [InlineData("B:3D1", 0, 0.46836)]
        [InlineData("B:3D1", 1, 0.42130, 0.66510)]
        //break tackle
        [InlineData("BT1:D3\",D4\"", 2, 0.16667, 0.38889, 0.44444)]
        [InlineData("BT1:D4\"", 1, 0.33333, 0.55556)]
        [InlineData("BT1:D5", 0, 0.50000)]
        [InlineData("BT1:D5,D4", 0, 0.30556)]
        [InlineData("BT1:D5,D4", 1, 0.30556, 0.57407)]
        [InlineData("BT1:D5\",D4\"", 0, 0.05556)]
        [InlineData("BT1:D7", 0, 0.16667)]
        [InlineData("BT2:D5,D4", 1, 0.44444, 0.72222)]
        [InlineData("BT2:D3¬,D4", 0, 0.63889)]
        [InlineData("BT2:D3¬,D4", 1, 0.55556, 0.86111)]
        [InlineData("D,I3,L4:D3,D3", 0, 0.90741)]
        //catch
        [InlineData("C:C3", 0, 0.88889)]
        //claw
        [InlineData("CL,MB1:2D3,B8,J8", 0, 0.18229)]
        [InlineData("CL,MB1:2D3,B9,J8", 0, 0.18229)]
        [InlineData("CL,SM:2D2,B9,J8", 0, 0.15271)]
        //consummate professional
        [InlineData("CP,L3,SF:R2,R2,2D2", 1, 0.52512, 0.68071)]
        //dodge
        [InlineData("D:D2,D2", 1, 0.92593, 0.94522)]
        [InlineData("D:D2;D:D2", 0, 0.94522)]
        [InlineData("D:D3\"", 0, 0.55556)]
        [InlineData("D:D3\",D2\"", 0, 0.39815)]
        [InlineData("D2\";D3\"", 0, 0.16667)]
        [InlineData("D3\",D2\"", 0, 0.16667)]
        [InlineData("D3\",D2\"", 1, 0.16667, 0.39815)]
        //shadowing
        [InlineData("D:D3,S+2", 0, 0.88889)]
        [InlineData("D:D3,S+2'{D3,S+2'{D3}}", 1, 0.77778, 0.81481)]
        [InlineData("D:D3,S+2'{D3,S+2{D3}}", 1, 0.62963, 0.81687)]
        [InlineData("D:D3,S+2{D3,S+2{D3}}", 0, 0.77778)]
        [InlineData("D:D3,S+2{D3,S+2{D3}}", 1, 0.44444, 0.83333)]
        [InlineData("D:D3,S+2{D3}", 0, 0.81481)]
        //tentacles
        [InlineData("D:N+3'[:N+2,D3]D2", 1, 0.54630, 0.73148)]
        [InlineData("D:N+3[:N+2,D3]D2", 0, 0.54630)]
        [InlineData("D:N+3[:N+2,D3]D2", 1, 0.32407, 0.68827)]
        [InlineData("D:N+3[:N+2[:N+2,D3]D3]D2", 0, 0.65741)]
        [InlineData("N+2[:N+2,D3,U3]D3,U3", 0, 0.33333)]
        [InlineData("N+3", 0, 0.33333)]
        //fouling
        [InlineData("DP2:F8", 0, 0.72222)]
        [InlineData("DP2:F8,J8", 0, 0.42824)]
        [InlineData("DP1:F8", 0, 0.58333)]
        [InlineData("DP1:F8,J8", 0, 0.31250)]
        [InlineData("F8", 0, 0.41667)]
        [InlineData("F8,2", 0, 0.26524)]
        [InlineData("F8,A6,2", 0, 0.27890)]
        [InlineData("F8,A6,E,2", 0, 0.32445)]
        [InlineData("F8,E,2", 0, 0.33356)]
        [InlineData("F8,E,A6,2", 0, 0.33584)]
        [InlineData("SG:F8,2", 0, 0.28935)]
        //dauntless
        [InlineData("L2',2D2|1D2", 1, 0.51852, 0.76132)]
        [InlineData("L2,2D2|1D2", 0, 0.51852)]
        [InlineData("L2,2D2|1D2", 2, 0.46296, 0.75514, 0.79561)]
        [InlineData("P:L3'*,2D2|-2D2", 1, 0.47325, 0.69273)]
        [InlineData("P:L3'*,2D2|-2D2,X4", 1, 0.23663, 0.34636)]
        [InlineData("BR,L4:L3,2D2|-2D2", 0, 0.50617)]
        //loner
        [InlineData("L3:2,2,2,2", 4, 0.48225, 0.69659, 0.73231, 0.73496, 0.73503)]
        [InlineData("L4:2,2", 2, 0.69444, 0.81019, 0.81501)]
        //throw team mate
        [InlineData("L3:T4-1';(X27/512,G4)(X54/512,G4,R2)(X117/512,G4,R2,R2)", 0, 0.08314)]
        [InlineData("L3:T4-1';G4", 1, 0.27778, 0.43519)]
        [InlineData("L3:T4-1;G4", 1, 0.16667, 0.41049)]
        [InlineData("L3:T5;G4", 0, 0.33333)]
        [InlineData("L4,TB:T5';G4", 0, 0.38889)]
        [InlineData("L4,TB:T5;G4", 0, 0.38889)]
        //hail mary pass
        [InlineData("M2;C,DC:C1", 1, 0.35156, 0.41016)]
        [InlineData("M2;C,DC:C2", 1, 0.30121, 0.35141)]
        [InlineData("M2;C2", 0, 0.05534)]
        [InlineData("M2;DC:C2", 1, 0.19531, 0.33285)]
        //mighty blow
        [InlineData("MB1,R:B9", 0, 0.58333)]
        [InlineData("MB1,R,S:B9", 0, 0.72222)]
        [InlineData("MB2:2D3,B8", 0, 0.54167)]
        [InlineData("MB2:2D3,B8,J8", 0, 0.32118)]
        [InlineData("MB1:2D3,B8", 0, 0.43750)]
        [InlineData("MB1:2D3,B8,J8", 0, 0.23438)]
        [InlineData("CR,MB1,R,S:B9", 0, 0.83333)]
        //pass
        [InlineData("P4-2,I6;C2", 0, 0.13050)]
        [InlineData("P4-2,I7;C2", 0, 0.13419)]
        [InlineData("P4-2;C,DC:C2", 0, 0.28252)]
        [InlineData("P4-2;C1", 0, 0.16656)]
        [InlineData("P4-2;C2", 0, 0.16102)]
        [InlineData("P4-2;C2", 2, 0.13889, 0.29622, 0.32166)]
        [InlineData("P4-2;DC:C2", 0, 0.22141)]
        [InlineData("PA:P2,I6;C2", 1, 0.67515, 0.78768)]
        [InlineData("PA:P2;C2", 1, 0.81019, 0.94522)]
        [InlineData("CB,PA:P2,I6;C2", 1, 0.78768, 0.91896)]
        //ram
        [InlineData("R:2D3,B8", 0, 0.43750)]
        [InlineData("R:2D3,B8,J8", 0, 0.23438)]
        [InlineData("SF,SH:U2,R2,R2,R2", 2, 0.84394, 0.89083, 0.89343)]
        //hypnogaze
        [InlineData("Y2';{D2}", 1, 0.97222, 0.99537)]
        [InlineData("Y2';{D2}U2", 2, 0.81019, 0.96451, 0.96772)]
        [InlineData("Y2;U2", 0, 0.69444)]
        [InlineData("Y2;{D2}", 0, 0.97222)]
        [InlineData("Y2;{D2}", 2, 0.83333, 0.99537, 0.99923)]
        [InlineData("Y2;{D2}U2", 0, 0.81019)]
        [InlineData("MD:Y2;{D2}U2", 0, 0.94522)]
        //old pro
        [InlineData("OP:2D2,B8", 0, 0.32922)]
        [InlineData("MB1,OP:2D2,B8", 0, 0.40638)]
        [InlineData("MB2,OP:2D2,B8", 0, 0.46468)]
        public void ActionCalculatorReturnsExpectedResult(string calculation, int rerolls, params double[] expected)
        {
            var result = _actionCalculator.Calculate(calculation);

            Assert.Equal(expected.Length, result.ProbabilityResults[rerolls].Probabilities.Length);

            for (var i = 0; i < expected.Length; i++)
            {
                Assert.Equal((decimal)expected[i], result.ProbabilityResults[rerolls].Probabilities[i], 5);
            }

            Assert.Equal(calculation, result.Calculation.ToString());
        }
    }
}
