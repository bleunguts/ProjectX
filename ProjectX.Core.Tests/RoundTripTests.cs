using Newtonsoft.Json;
using ProjectX.Core.Requests;
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
            var obj = new OptionsPricingByMaturityResults(Guid.NewGuid(), 
                new List<MaturityAndOptionGreeksResultPair> 
                { 
                    new (1.0, new OptionGreeksResult(1.0, 1.0, 1.0, 1.0, 1.0, 1.0)),
                    new (3.0, new OptionGreeksResult(5.0, 1.0, 1.0, 1.0, 1.0, 5.0)) 
                });
            var serialized = JsonConvert.SerializeObject(obj);
            Console.WriteLine($"Json: {serialized}");
            var deserialized = JsonConvert.DeserializeObject<OptionsPricingByMaturityResults>(serialized);
            Console.WriteLine($"ConvertBack: {deserialized}");
        }
    }
}
