
using ProjectXAnalyticsCppLib;

namespace ProjectX.AnalyticsCppLib.Tests
{
    public class Tests
    {        
        [Test]
        public void ShallBeAbleToPriceOptionWithCppOptionsPricingCalculator()
        {
            var calculator = new OptionsPricingCalculator();
            
            VanillaOption theOption = new();
            double spot = 11.0;
            Parameters vol = new Parameters();
            Parameters r = new Parameters();
            uint numberOfPaths = 10;            

            double price = calculator.MCValue(ref theOption, spot, ref vol, ref r, numberOfPaths);                        
            Assert.That(price, Is.EqualTo(11.0));            
        }
    }
}