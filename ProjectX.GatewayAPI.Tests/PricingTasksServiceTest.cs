using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ProjectX.Core;
using ProjectX.Core.Requests;
using ProjectX.GatewayAPI.BackgroundServices;
using ProjectX.GatewayAPI.Processors;

namespace ProjectX.GatewayAPI.Tests
{
    public class PricingTasksServiceTest
    {
        [Test]
        public async Task ShouldCallPricingTasksProcessor_ForEachMessageInChannel()
        {
            var pricingProcessor = new FakeProcessor();
            var sc = new ServiceCollection();
            sc.AddTransient<IPricingTasksProcessor>(s => pricingProcessor);

            var sp = sc.BuildServiceProvider();

            var channel = new PricingTasksChannel(NullLogger<PricingTasksChannel>.Instance);

            var demoRequests = new OptionsPricingByMaturitiesRequest[]
            {
                new(10, OptionType.Call, 100.0, 150.0, 1.0, 1.0, 0.3),
                new(10, OptionType.Put, 100.0, 150.0, 1.0, 1.0, 0.3),
            };

            await channel.SendRequestAsync(demoRequests[0]);
            channel.CompleteWriter();

            var sut = new PricingTasksService(
                NullLogger<PricingTasksService>.Instance,
                channel,
                sp,
                Mock.Of<IHostApplicationLifetime>());

            await sut.StartAsync(default);
            await sut.ExecuteTask;

            Assert.That(pricingProcessor.ExecutionCount, Is.EqualTo(1));
        }

        private class FakeProcessor : IPricingTasksProcessor
        {
            public int ExecutionCount { get; set; } = 0;

            public Task Process(OptionsPricingByMaturitiesRequest multipleTimeslicesOptionsPricingRequest, CancellationToken cancellationToken = default)
            {
                ExecutionCount++;
                return Task.CompletedTask;
            }
        }
    }
}