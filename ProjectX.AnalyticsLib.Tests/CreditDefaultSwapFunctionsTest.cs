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
    public void WhenValuingCdsPVItShouldReturnValidPVAndProbabilities()
    {
        var evalDate = new DateTime(2009, 6, 15);
        var effectiveDate = new DateTime(2009, 3, 20);
        var maturityDate = new DateTime(2014, 6, 20);
        var spreadsInBps = new double[] { 210.0 };
        var tenors = new string[] { "5Y" };
        var recoveryRate = 0.4;
        var couponInBps = 100;
        var notional = 10_000;
        Protection.Side protectionSide = Protection.Side.Buyer;
        var interestRate = 0.07;
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
        Assert.That(actual.PV, Is.EqualTo(471).Within(1), "PV must be equal to expected value within tolerance"); 
        Assert.That(actual.FairSpread, Is.EqualTo(218).Within(1), "Fair spread must be equal to expected value within tolerance"); 
    }

    [Test]
    public void WhenValuingCdsPVWithMultipleSpreadPointsItShouldReturnValidPVAndProbabilities()
    {
        var evalDate = new DateTime(2015, 5, 15);
        var effectiveDate = new DateTime(2015, 3, 20);
        var maturityDate = new DateTime(2018, 6, 20);
        var spreadsInBps = new double[] { 34.93, 53.6, 72.02, 106.39, 129.39, 139.46 };
        var tenors = new string[] { "1Y","2Y","3Y","5Y","7Y","10Y" };
        var recoveryRate = 0.4;
        var couponInBps = 100;
        var notional = 10_000;
        Protection.Side protectionSide = Protection.Side.Buyer;
        var interestRate = 0.07;
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
        Assert.That(actual.SurvivalProbabilityPercentage, Is.EqualTo(96.3).Within(1));
        Assert.That(actual.DefaultProbabilityPercentage, Is.EqualTo(3.6).Within(1));
        Assert.That(actual.HazardRatePercentage, Is.EqualTo(1.8).Within(1));
        Assert.That(actual.PV, Is.EqualTo(-137).Within(1), "PV must be equal to expected value within tolerance");
        Assert.That(actual.FairSpread, Is.EqualTo(51.3).Within(1), "Fair spread must be equal to expected value within tolerance");
    }
}
