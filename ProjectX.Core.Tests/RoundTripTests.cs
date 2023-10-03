using Newtonsoft.Json;
using NUnit.Framework.Constraints;
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

        [Test]
        public void CanJsonSerializeAndDeserializePlotResults()
        {
            var obj = new PlotOptionsPricingResult(Guid.NewGuid(),
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
                    PointArray = new Point3D[,] 
                    {
                        { new Point3D(1, 3, 4) }, { new Point3D(1, 3, 4) },
                        { new Point3D(1, 3, 4) }, { new Point3D(1, 3, 4) },   { new Point3D(1, 3, 4) },
                        { new Point3D(1, 3, 4) }, { new Point3D(1, 3, 5) },
                    }
                });
            var serialized = JsonConvert.SerializeObject(obj);
            Console.WriteLine($"Json: {serialized}");
            var deserialized = JsonConvert.DeserializeObject<PlotOptionsPricingResult>(serialized);
            Console.WriteLine($"ConvertBack: {deserialized}");
        }
    }
}
