﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Reactive.Testing;
using ProjectX.Core.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using ProjectX.Core.Services;

namespace ProjectX.Core.Tests
{
    public class FXMarketServiceTest
    {
        private static ILogger<FXMarketService> _logger;
        private static decimal RawSpreadInPips = 2.0M;
        readonly Mock<IFXSpotPriceStream> _priceGenerator = new Mock<IFXSpotPriceStream>();
        readonly Mock<IFXSpotPricer> _fxPricer = new Mock<IFXSpotPricer>();

        [SetUp]
        public void SetUp()
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();
            var factory = serviceProvider.GetService<ILoggerFactory>();
            _logger = factory.CreateLogger<FXMarketService>();
        }

        [Test]
        public async Task WhenSubscribingToFxSpotPriceEventsThenItShouldGetPriceResponses()
        {
            // arrange
            var recieved = new List<System.Reactive.Timestamped<SpotPriceResponse>>();
            var errors = new List<Exception>();            

            // act
            FXMarketService _sut = new FXMarketService(_logger, _priceGenerator.Object, _fxPricer.Object);            
            var spotPriceEvents = _sut.StreamSpotPricesFor(new SpotPriceRequest("EURUSD", "tests", SpotPriceSubscriptionMode.Subscribe));
            spotPriceEvents!.Subscribe(recieved.Add,errors.Add);            
            await Task.Delay(950);
            Console.WriteLine($"{recieved.Count} responses received.");
            foreach (var response in recieved)
                Console.WriteLine($"Yay response received in test @ [{response.Timestamp}]: {response.Value.SpotPrice.ToString()}");

            // assert
            Assert.That(recieved, Has.Count.GreaterThanOrEqualTo(9), $"Responses: {string.Join(Environment.NewLine, recieved)}  ");
            Assert.That(errors, Has.Count.EqualTo(0), $"Errors occured: {string.Join(Environment.NewLine, errors)} ");                                    
        }

        [Test]  
        public void WhenUnsubscribingFromPriceStreamShouldRemoveFromInternalDictionary()        
        {
            // arrange
            FXMarketService _sut = new FXMarketService(_logger, _priceGenerator.Object, _fxPricer.Object);
            _sut.StreamSpotPricesFor(new SpotPriceRequest("EURUSD", "tests", SpotPriceSubscriptionMode.Subscribe));
            Assert.That(_sut.SpotPriceStreamsFor("EURUSD"), Is.Not.Null);

            // act
            _sut.UnStream("EURUSD");

            // assert
            Assert.That(_sut.SpotPriceStreamsFor("EURUSD"), Is.Null);

        }
    }
}
