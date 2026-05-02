using ActionCalculator.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ActionCalculator.Tests
{
    public class ActionCalculatorTests
    {
        private readonly ICalculator _calculator;
        private readonly ICalculationBuilder _calculationBuilder;

        public ActionCalculatorTests()
        {
            var services = new ServiceCollection();

            services.AddActionCalculatorServices();

            var serviceProvider = services.BuildServiceProvider();

            _calculator = serviceProvider.GetService<ICalculator>();
            _calculationBuilder = serviceProvider.GetService<ICalculationBuilder>();
        }

        [Theory]
        //rerollable
        [InlineData("2", 1, 0.83333, 0.97222)]
        [InlineData("2,2", 2, 0.69444, 0.92593, 0.94522)]
        [InlineData("2,3,4,5,6", 5, 0.01543, 0.05401, 0.09045, 0.10652, 0.10979, 0.11003)]
        [InlineData("2,3;4,5,6", 5, 0.01543, 0.05401, 0.09045, 0.10652, 0.10979, 0.11003)]
        [InlineData("5", 1, 0.33333, 0.55556)]
        //leap
        [InlineData("E3", 1, 0.66667, 0.88889)]
        [InlineData("E4", 1, 0.50000, 0.75000)]
        [InlineData("E5", 1, 0.33333, 0.55556)]
        [InlineData("P:E3", 0, 0.81481)]
        [InlineData("P:E3", 1, 0.66667, 0.88889)]
        [InlineData("L4:E3", 1, 0.66667, 0.77778)]
        [InlineData("E3\"", 0, 0.33333)]
        [InlineData("E3\"", 1, 0.33333, 0.55556)]
        //alternate branches
        [InlineData("(1/2)(1/4)(1/4)", 0, 1.00000)]
        [InlineData("(1/2,2)(1/4,3)(1/4,4)", 0, 0.70833)]
        [InlineData("2(1/2)(1/4)(1/4)2", 2, 0.69444, 0.92593, 0.94522)]
        //block
        [InlineData("-2D2,2,2D3", 3, 0.06944, 0.16011, 0.18872, 0.19129)]
        [InlineData("-3D1", 1, 0.00463, 0.00924)]
        [InlineData("1D5", 1, 0.83333, 0.97222)]
        [InlineData("2D3,K8", 0, 0.31250)]
        [InlineData("2D3,K8,J8", 0, 0.13021)]
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
        [InlineData("B,P:2D2!2{1D2}", 0, 0.80436)]
        [InlineData("B,P:2D2!2{1D2}", 1, 0.55556, 0.87654)]
        [InlineData("B,P:2D2!2^{1D2}", 1, 0.65998, 0.85751)]
        [InlineData("B,P:2D2!2*{1D2}", 1, 0.76475, 0.81207)]
        [InlineData("B,P:2D2!2'{1D2}", 1, 0.70782, 0.83745)]
        [InlineData("B,P:2D2!2^'{1D2}", 1, 0.74640, 0.82047)]
        [InlineData("B,P:2D2!2*'{1D2}", 1, 0.74897, 0.79835)]
        [InlineData("B,P:2D2!2^*'{1D2}", 1, 0.77418, 0.82510)]
        //brawler
        [InlineData("B,P:-2D2", 0, 0.23663)]
        [InlineData("B,P:-2D2", 1, 0.22222, 0.27160)]
        [InlineData("B,P:-2D2^*", 1, 0.23663, 0.26440)]
        [InlineData("B,P:-3D3", 1, 0.30556, 0.34201)]
        [InlineData("B:-2D2", 1, 0.14815, 0.23457)]
        [InlineData("B:1D1", 1, 0.19444, 0.30556)]
        [InlineData("B:3D1", 0, 0.46836)]
        [InlineData("B:3D1", 1, 0.42130, 0.66510)]
        //break tackle
        [InlineData("BT3:D5", 0, 0.83333)]
        [InlineData("BT3:D7", 0, 0.50000)]
        [InlineData("BT3:D5,D4", 1, 0.52778, 0.78704)]
        [InlineData("BT1:D3\",D4\"", 2, 0.16667, 0.38889, 0.44444)]
        [InlineData("BT1:D4\"", 1, 0.33333, 0.55556)]
        [InlineData("BT1:D5", 0, 0.50000)]
        [InlineData("BT1:D5,D4", 1, 0.30556, 0.57407)]
        [InlineData("BT1:D5\",D4\"", 0, 0.05556)]
        [InlineData("BT1:D7", 0, 0.16667)]
        [InlineData("BT2:D5,D4", 1, 0.44444, 0.72222)]
        [InlineData("BT2:D3¬,D4", 0, 0.63889)]
        [InlineData("BT2:D3¬,D4", 1, 0.55556, 0.86111)]
        //hatred
        [InlineData("H:1D3%", 1, 0.58333, 0.75000)]
        [InlineData("H:-2D2%", 0, 0.14815)]
        [InlineData("H:-2D2", 1, 0.14815, 0.21296)]
        //catch
        [InlineData("C:C3", 0, 0.88889)]
        //claw
        [InlineData("CL,MB1:2D3,K8,J8", 0, 0.18229)]
        [InlineData("CL,MB1:2D3,K9,J8", 0, 0.18229)]
        [InlineData("CL,SM:2D2,K9,J8", 0, 0.15271)]
        //dodge
        [InlineData("D:D2,D2", 1, 0.92593, 0.94522)]
        [InlineData("D:D2;D:D2", 0, 0.94522)]
        [InlineData("D:D3\"", 0, 0.55556)]
        [InlineData("D:D3\",D2\"", 0, 0.39815)]
        [InlineData("D2\";D3\"", 0, 0.16667)]
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
        [InlineData("F8,2", 0, 0.23148)]
        [InlineData("F8,A6,2", 0, 0.25077)]
        [InlineData("F8,A6,B,2", 0, 0.31507)]
        [InlineData("F8,B,2", 0, 0.32793)]
        [InlineData("F8,B,A6,2", 0, 0.33115)]
        [InlineData("SG:F8,2", 0, 0.28935)]
        //lone fouler
        [InlineData("LF:F8", 0, 0.65972)]
        [InlineData("DP1,LF:F8", 0, 0.82639)]
        //unstoppable momentum
        [InlineData("UM:1D2", 1, 0.55556)]
        [InlineData("UM:2D3", 1, 0.87500)]
        [InlineData("UM:-2D2", 0, 0.25926)]
        [InlineData("UM:-2D3", 0, 0.50000)]
        //lord of chaos (fractional)
        [InlineData("LC:-2D3", 1, 0.50000, 0.62500)]
        //savage blow
        [InlineData("SB:1D2", 0, 0.55556)]
        [InlineData("SB:2D2", 0, 0.80247)]
        [InlineData("SB:-2D2", 0, 0.30864)]
        //dauntless
        [InlineData("L2',2D2|1D2", 1, 0.51852, 0.76132)]
        [InlineData("L2,2D2|1D2", 0, 0.51852)]
        [InlineData("L2,2D2|1D2", 2, 0.46296, 0.75514, 0.79561)]
        [InlineData("P:L3'*,2D2|-2D2", 1, 0.47325, 0.69273)]
        [InlineData("P:L3'*,2D2|-2D2,1/2", 1, 0.23663, 0.34636)]
        [InlineData("BR,L4:L3,2D2|-2D2", 0, 0.50617)]
        //loner
        [InlineData("L3:2,2,2,2", 4, 0.48225, 0.69659, 0.73231, 0.73496, 0.73503)]
        [InlineData("L4:2,2", 2, 0.69444, 0.81019, 0.81501)]
        //throw team mate
        [InlineData("L3:H4-1';(27/512,G4)(54/512,G4,R2)(117/512,G4,R2,R2)", 0, 0.08314)]
        [InlineData("L3:H4-1';G4", 1, 0.27778, 0.43519)]
        [InlineData("L3:H4-1;G4", 1, 0.16667, 0.41049)]
        [InlineData("L3:H5;G4", 0, 0.33333)]
        [InlineData("L4,TB:H5';G4", 0, 0.38889)]
        [InlineData("L4,TB:H5;G4", 0, 0.38889)]
        [InlineData("L4,TB:H5';G4~S2", 0, 0.38889)]
        [InlineData("L4,TB:H5;G4~S2", 0, 0.38889)]
        //hail mary pass
        [InlineData("M2;C,DC:C1", 1, 0.35156, 0.41016)]
        [InlineData("M2;C,DC:C2", 1, 0.30121, 0.35141)]
        [InlineData("M2;C2", 0, 0.05534)]
        [InlineData("M2;DC:C2", 1, 0.19531, 0.33285)]
        [InlineData("BI:M2;C2", 1, 0.12639, 0.17519)]
        [InlineData("BI:M2;DC:C2", 0, 0.43509)]
        [InlineData("BI:M2;C,DC:C2", 0, 0.57135)]
        [InlineData("BI:M2;C,DC:C1", 1, 0.61852, 0.72161)]
        //mighty blow
        [InlineData("MB1,R:K9", 0, 0.58333)]
        [InlineData("MB1,R,S:K9", 0, 0.72222)]
        [InlineData("MB2:2D3,K8", 0, 0.54167)]
        [InlineData("MB2:2D3,K8,J8", 0, 0.32118)]
        [InlineData("MB1:2D3,K8", 0, 0.43750)]
        [InlineData("MB1:2D3,K8,J8", 0, 0.23438)]
        [InlineData("CR,MB1,R,S:K9", 0, 0.83333)]
        //pass
        [InlineData("L4,TB:P3;C3~S2", 0, 0.59536)]
        [InlineData("L4,TB:P3;C3", 0, 0.45275)]
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
        [InlineData("R:2D3,K8", 0, 0.43750)]
        [InlineData("R:2D3,K8,J8", 0, 0.23438)]
        [InlineData("SF,SH:U2,R2,R2,R2", 2, 0.84394, 0.89083, 0.89343)]
        //hypnogaze
        [InlineData("Y2'[:D2]", 1, 0.97222, 0.99537)]
        [InlineData("Y2';{D2}U2", 2, 0.81019, 0.96451, 0.96772)]
        [InlineData("Y2;U2", 0, 0.69444)]
        [InlineData("Y2[D2]", 0, 0.97222)]
        [InlineData("Y2[D2]", 2, 0.83333, 0.99537, 0.99923)]
        [InlineData("Y2;{D2}U2", 0, 0.81019)]
        [InlineData("MD:Y2;{D2}U2", 0, 0.94522)]
        //old pro
        [InlineData("OP:2D2,K8", 0, 0.32922)]
        [InlineData("MB1,OP:2D2,K8", 0, 0.40638)]
        [InlineData("MB2,OP:2D2,K8", 0, 0.46468)]
        //stab
        [InlineData("T8", 0, 0.41667)]
        [InlineData("T8[:2D2]", 0, 0.74074)]
        //chainsaw
        [InlineData("W8", 0, 0.41667)]
        [InlineData("W8[:2D2]", 0, 0.74074)]
        //consummate professional
        [InlineData("CP:D3~S2", 0, 0.88889)]
        [InlineData("CP,L3,SF:R2,R2,2D2~S2", 1, 0.52512, 0.68071)]
        [InlineData("CP:D2", 0, 0.83333)]
        [InlineData("CP:D5", 0, 0.50000)]
        [InlineData("CP:C3", 0, 0.83333)]
        [InlineData("CP:E5", 0, 0.50000)]
        public void CalculatorTests(string calculationString, int rerolls, params double[] expected)
        {
            var calculation = _calculationBuilder.Build(calculationString, rerolls);
            var result = _calculator.Calculate(calculation);

            Assert.Equal(expected.Length, result.Results.Length);

            for (var i = 0; i < expected.Length; i++)
            {
                Assert.Equal((decimal)expected[i], result.Results[i], 5);
            }

            Assert.Equal(calculationString, calculation.ToString());
        }

        [Theory]
        //star players
        [InlineData("Akhorne:D3,L3,2D2|-2D2,K9", 1, 0.18747, 0.23015)]
        [InlineData("Anqi:1D2", 0, 0.55556)]
        [InlineData("Barik:P2;C2", 1, 0.81019, 0.94522)]
        [InlineData("Bilerot:F8", 1, 0.82639)]
        [InlineData("Boa:D3,2D2", 1, 0.49383, 0.60357)]
        [InlineData("Bomber:D3,2D2", 1, 0.49383, 0.60357)]
        [InlineData("Borak:2D3,K8,F8", 1, 0.29774, 0.31901)]
        [InlineData("Karina:D3,2D2", 1, 0.49383, 0.60357)]
        [InlineData("Cindy:D3,2D2", 1, 0.49383, 0.60357)]
        [InlineData("Crumble:U3,D3", 1, 0.79012)]
        [InlineData("Deeproot:2D3,K8", 1, 0.43750, 0.49219)]
        [InlineData("Dribl:D3,F8", 1, 0.64198)]
        [InlineData("Drull:D3,2D2", 1, 0.49383, 0.60357)]
        [InlineData("Drull:T8", 0, 0.58333)]
        [InlineData("Eldril:D3,Y3,C3", 1, 0.70233, 0.79012)]
        [InlineData("Estelle:D3,2D2", 1, 0.49383, 0.60357)]
        [InlineData("Frank:D5,2D3,K8", 1, 0.21875, 0.30078)]
        [InlineData("Fungus:4/6,2D3,K8", 1, 0.38889, 0.43750)]
        [InlineData("Glart:2D3,K9", 1, 0.31250, 0.35156)]
        [InlineData("Gloriel:D3,P2;C2", 1, 0.72016, 0.84019)]
        [InlineData("Glotl:2D3,K8", 1, 0.43750, 0.49219)]
        [InlineData("Gobbo:D3,F8", 1, 0.37037)]
        [InlineData("Grak:2D3,K8", 1, 0.43750, 0.49219)]
        [InlineData("Grashnak:2D3,K8", 1, 0.43750, 0.49219)]
        [InlineData("Gretchen:D3,D3", 1, 0.74074, 0.76543)]
        [InlineData("Griff:R2,2D2,D3~S2", 1, 0.48011, 0.62236)]
        [InlineData("Grim:2D3,K8", 1, 0.43750, 0.49219)]
        [InlineData("Grom:R2,D5,2D3,K8", 1, 0.21267, 0.29243)]
        [InlineData("Guffle:2,2", 2, 0.69444, 0.81019, 0.81501)]
        [InlineData("H'thark:R2,D5,2D3", 1, 0.42535, 0.53168)]
        [InlineData("Hakflem:D3,2D2", 1, 0.49383, 0.60357)]
        [InlineData("Helmut:2D2,K8", 1, 0.32922, 0.40238)]
        [InlineData("Ivan:2D3,K8", 1, 0.43750, 0.49219)]
        [InlineData("Ivan*:2D3%,K8", 1, 0.59182, 0.62191)]
        [InlineData("Ivar:2,2", 2, 0.69444, 0.81019, 0.81501)]
        [InlineData("Jeremiah:D3,P2;C,DC:C2", 1, 0.84019)]
        [InlineData("Jordell:D3,2D2;DC:C2", 1, 0.41152, 0.57156)]
        [InlineData("Josef:2,2", 2, 0.69444, 0.84877, 0.85734)]
        [InlineData("Karla:D3,2D2", 1, 0.49383, 0.60357)]
        [InlineData("Kiroth:2,2", 2, 0.69444, 0.81019, 0.81501)]
        [InlineData("Kreek:2D3,K8", 1, 0.43750, 0.49219)]
        [InlineData("Lucien:2D3,K8", 1, 0.43750, 0.49219)]
        [InlineData("Luthor:2,2", 2, 0.69444, 0.81019, 0.81501)]
        [InlineData("Maple:2D3,K8", 1, 0.47801, 0.50231)]
        [InlineData("Max:2,2", 2, 0.69444, 0.81019, 0.81501)]
        [InlineData("Morg:H5;G4", 1, 0.38889, 0.60185)]
        [InlineData("Morg:H5;G4~S2", 1, 0.38889, 0.60185)]
        [InlineData("Nobbla:D3,2D2", 1, 0.49383, 0.60357)]
        [InlineData("Puggy:D3,2D2", 1, 0.49383, 0.64015)]
        [InlineData("Rashnack:F8,2", 1, 0.28935, 0.31346)]
        [InlineData("Rashnack:T8,J8", 0, 0.24306)]
        [InlineData("Rodney:C3", 0, 0.88889)]
        [InlineData("Rowana:E3,D3,2D2", 1, 0.43896, 0.53650)]
        [InlineData("Ripper:2D3,K8", 1, 0.43750, 0.49219)]
        [InlineData("Ripper:D3,2D2", 1, 0.45267, 0.55327)]
        [InlineData("Roxanna:D3,2D2", 1, 0.49383, 0.60357)]
        [InlineData("Rumbelow:2D3,K8", 1, 0.43750, 0.49219)]
        [InlineData("Scyla:2D3,K9", 1, 0.31250, 0.35156)]
        [InlineData("Swiftvine:2,2", 1, 0.69444, 0.81019)]
        [InlineData("Scrappa:R2,D3,F8", 1, 0.50412)]
        [InlineData("Skitter:D3,R2,T8", 1, 0.48868, 0.52941)]
        [InlineData("Skrorg:2D3,K9", 1, 0.31250, 0.35156)]
        [InlineData("Skrull:P2;C2", 1, 0.81019, 0.94522)]
        [InlineData("Thorsson:2,2", 2, 0.69444, 0.81019, 0.81501)]
        [InlineData("Valen:P2;C2", 1, 0.81019, 0.94522)]
        [InlineData("Varag:1D3%,2D3,K8", 1, 0.27884, 0.32948)]
        [InlineData("Wilhelm:2D2,K9,J8", 1, 0.15271, 0.18665)]
        //[InlineData("Willow:2D3,K8", 1, 0.31250, 0.35156)]
        [InlineData("Wither:2,2", 2, 0.69444, 0.81019, 0.81501)]
        [InlineData("Zolcath:R2,2D3,K8", 1, 0.42535, 0.47852)]
        [InlineData("Zug:2D3,K8", 1, 0.54167, 0.60938)]
        [InlineData("Zzharg:2,2", 1, 0.69444, 0.81019)]
        public void StarPlayerTests(string calculationString, int rerolls, params double[] expected)
        {
            var calculation = _calculationBuilder.Build(calculationString, rerolls);
            var result = _calculator.Calculate(calculation);

            Assert.Equal(expected.Length, result.Results.Length);

            for (var i = 0; i < expected.Length; i++)
            {
                Assert.Equal((decimal)expected[i], result.Results[i], 5);
            }
        }
    }
}
