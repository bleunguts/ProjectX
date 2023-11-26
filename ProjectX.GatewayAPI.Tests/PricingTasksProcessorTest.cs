using Castle.Core.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Moq;
using ProjectX.Core.Requests;
using ProjectX.Core.Services;
using ProjectX.GatewayAPI.ExternalServices;
using ProjectX.GatewayAPI.Processors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.GatewayAPI.Tests
{
    [TestFixture]
    public class PricingTasksProcessorTest
    {
        OptionsPricingByMaturitiesRequest _request = new OptionsPricingByMaturitiesRequest(10, Core.OptionType.Call, 100, 0.1, 0.1, 0.0, 0.3, OptionsPricingCalculatorType.OptionsPricerCpp);

        [Test]
        public async Task WhenOptionsPricingModelContainsCalculatorThatThrowsAnError_ItShouldRethrowGracefully()
        {
            var pricingModel = new Mock<IOptionsPricingModel>();
            pricingModel.Setup(pricingModel => pricingModel.Price(_request)).Throws(new Exception("Blow up"));

            var sut = new PricingTasksProcessor(
                NullLogger<PricingTasksProcessor>.Instance, 
                pricingModel.Object, 
                Mock.Of<IPricingResultsApiClient>());            

            Assert.That(async() => await sut.Process(_request, default), Throws.InstanceOf(typeof(ApplicationException)).With.Message.Contain("PricingModel processing threw an error"), "Should not blow up if calculators throw exception");
        }

        [Test]
        public async Task WhenOptionsPricingModelContainsCalculatorThatReturnsNull_ItShouldRethrowGracefully()
        {
            var pricingModel = new Mock<IOptionsPricingModel>();
            pricingModel.Setup(pricingModel => pricingModel.Price(_request)).Returns(() => null);

            var sut = new PricingTasksProcessor(
                NullLogger<PricingTasksProcessor>.Instance,
                pricingModel.Object,
                Mock.Of<IPricingResultsApiClient>());

            Assert.That(async () => await sut.Process(_request, default), Throws.InstanceOf(typeof(ApplicationException)).With.Message.Contain("PricingModel processing returned null"), "Should not blow up if calculators throw exception");
        }
    }
}
