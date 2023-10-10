﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Core.Tests
{
    public class ExtensionsTest
    {        
        [Test]
        public void WhenPrettifyBidAskSpotPriceItShouldConvertToAPrettyStringToBeDisplayedToTheUser()
        {
            var spotPrice = new SpotPrice("GBPUSD", 1.6020M, 1.8343M);
            Assert.That(spotPrice.ToPrettifiedBidAskPrice(), Is.EqualTo("1.60200/1.83430"));
        }

        [Test]
        public void WhenConvertingBidAskSpotPriceBackToSpotPriceObjectItShouldCaptureAllKeyElements()
        {
            var actualPrice = "1.60200/1.83430".ToSpotPrice("N/A");

            Assert.That(actualPrice.BidPrice, Is.EqualTo(1.60200));
            Assert.That(actualPrice.AskPrice, Is.EqualTo(1.83430));
            Assert.That(actualPrice.CurrencyPair, Is.EqualTo("N/A"));
        }
    }
}