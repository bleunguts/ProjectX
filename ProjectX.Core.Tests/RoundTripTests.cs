using Newtonsoft.Json;
using NUnit.Framework.Constraints;
using ProjectX.Core.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProjectX.Core.Tests
{
    public class RoundTripTests
    {
        private Random _random = new Random();
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

        [Test]
        public void CanJsonSerializeAndDeserializePlotResultsUsingMicrosoftJsonSerializer()
        {
            MyPoint3D[,] point3Ds = new MyPoint3D[,]
            {
                        { new MyPoint3D(1, 3, 4) }, { new MyPoint3D(1, 3, 4) },
                        { new MyPoint3D(1, 3, 4) }, { new MyPoint3D(1, 3, 4) },   { new MyPoint3D(1, 3, 4) },
                        { new MyPoint3D(1, 3, 4) }, { new MyPoint3D(1, 3, 5) },
            };          
            var obj = new PlotOptionsPricingResult(new PlotOptionsPricingRequest(OptionGreeks.Delta, OptionType.Call, 30, 0.6,0.5,0.3),
                new PlotResults()
                {
                    XLimitMin = 1,
                    XNumber = 2,
                    YLimitMin = 3,
                    YNumber = 4,
                    XSpacing = 5,
                    YSpacing = 6,
                    zmax = 7,
                    zmin = 8,
                    PointArray = point3Ds
                });
           
            var serialized = System.Text.Json.JsonSerializer.Serialize<PlotOptionsPricingResult>(obj, obj.JsonOptions());
            Console.WriteLine($"Json: {serialized}");
            var deserialized = System.Text.Json.JsonSerializer.Deserialize<PlotOptionsPricingResult>(serialized, serialized.JsonOptions());
            Console.WriteLine($"ConvertBack: {deserialized}");
        }
    }
}
