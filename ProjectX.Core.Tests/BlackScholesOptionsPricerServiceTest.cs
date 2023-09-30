using ProjectX.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Core.Tests
{
    public class BlackScholesOptionsPricerServiceTest
    {
        private readonly BlackScholesOptionsPricerService _sut = new BlackScholesOptionsPricerService();
     
        [Test]
        public void WhenPlottingGreeksZValuesAreValid()
        {                        
            var result = _sut.PlotGreeks(OptionGreeks.Price, OptionType.Call, 100, 0.1, 0.04, 0.3);
            AssertZValue(result.zmin, result.zmax, rounding: 1);
        }

        private static void AssertZValue(double zmin, double zmax, int rounding)
        {
            var theZmin = Math.Round(zmin, rounding);
            var theZmax = Math.Round(zmax, rounding);
            var theZTick = Math.Round((zmax - zmin) / 5.0, rounding);

            Assert.IsTrue(zmin < zmax);
            Assert.IsTrue(theZTick > 0);
        }       
    }
}
