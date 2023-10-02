using Newtonsoft.Json;
using ProjectX.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Core.Tests
{
    public class RoundTripTests
    {
        [Test]
        public void WhenOptionsPricingResultsIsBeingSerializedAndDeserialiezedThenItShouldNotBlowUp()
        {
            var obj = new OptionsPricingResults(Guid.NewGuid(), 
                new List<(double maturities, OptionGreeksResult optionGreeks)> 
                { 
                    (1.0, new OptionGreeksResult(1.0, 1.0, 1.0, 1.0, 1.0, 1.0)),
                    (3.0, new OptionGreeksResult(5.0, 1.0, 1.0, 1.0, 1.0, 5.0)) 
                });
            var serialized = JsonConvert.SerializeObject(obj);
            Console.WriteLine($"Json: {serialized}");
            var deserialized = JsonConvert.DeserializeObject<OptionsPricingResults>(serialized);
            Console.WriteLine($"ConvertBack: {deserialized}");
        }
    }
}
