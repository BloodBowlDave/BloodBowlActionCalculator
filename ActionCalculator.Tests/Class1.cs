using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ActionCalculator.Tests
{
    public class Class1
    {
        [Fact]
        public void asdasd()
        {
            var indexLists = new List<List<int>>();

            indexLists.Add(new List<int>{0,1,2});
            indexLists.Add(new List<int>{0,1});
            indexLists.Add(new List<int>{0,1,2});

            foreach (var indexList in indexLists)
            {
                var asdsa = new List<List<int>>();


            }
        }

    }
}
