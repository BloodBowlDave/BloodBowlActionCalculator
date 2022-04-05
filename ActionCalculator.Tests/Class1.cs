using Xunit;

namespace ActionCalculator.Tests
{
    public class Class1
    {
        [Fact]
        public void Test()
        {
            var x = "asdasd{asd}asdas";
            var y = "asdasd{asdfsgdef}";
            var y2 = "{asdasd}{asdfsgdef}";
            var y3 = "{asdasda{sdfs}gdef}";

            var z = x.Split('{', '}');
            var w = y.Split('{', '}');
            var asd = y2.Split('{', '}');
            var as323d = y3.Split('{', '}');

            var s = "";
        }
    }
}
