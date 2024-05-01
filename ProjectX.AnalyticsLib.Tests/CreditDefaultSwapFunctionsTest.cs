using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLNet;

namespace ProjectX.AnalyticsLib.Tests;

public class CreditDefaultSwapFunctionsTest
{
    [Test]
    public void WhenValuingCdsPVUsingFlatInterestCurveItShouldReturnValidPV()
    {
        var evalDate = new DateTime(2009, 6, 15);
        var effectiveDate = new DateTime(2009, 3, 20);
        var maturityDate = new DateTime(2014, 6, 20);
        var spreadsInBps = new int[] { 210 };
        var tenors = new string[] { "5Y" };
        var recoveryRate = 0.4;
        var couponInBps = 100;
        var notional = 10_000;
        Protection.Side protectionSide = Protection.Side.Buyer;
        var interestRate = 0.15;
        var actual = CreditDefaultSwapFunctions.PV(
            evalDate, 
            effectiveDate,
            maturityDate,
            spreadsInBps,
            tenors,
            recoveryRate,
            couponInBps,
            notional,
            protectionSide, 
            interestRate);
        Assert.That(actual.SurvivalProbabilityPercentage, Is.EqualTo(82).Within(1));
        Assert.That(actual.DefaultProbabilityPercentage, Is.EqualTo(17).Within(1));
        Assert.That(actual.HazardRatePercentage, Is.EqualTo(3).Within(1));
        Assert.That(actual.PV, Is.EqualTo(405).Within(1), "PV must be equal to expected value within tolerance"); 
        Assert.That(actual.FairSpread, Is.EqualTo(224).Within(1), "Fair spread must be equal to expected value within tolerance"); 
    }
}
