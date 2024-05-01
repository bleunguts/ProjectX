using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using QLNet;

namespace ProjectX.AnalyticsLib;

public class CreditDefaultSwapFunctions
{
    /// <summary>
    /// Uses Flat Interest Rate Curve - instead of PiecewiseLogCubic ISDA rate curves
    /// </summary>    
    public static CreditDefaultSwapPVResult PV(
        DateTime evaluationDate, 
        DateTime effectiveDate, 
        DateTime maturityDate, 
        double[] spreadsInBps, 
        string[] tenors, 
        double recoveryRate, 
        int couponInBps, 
        int notional, 
        Protection.Side protectionSide, 
        double flatInterestRate)
    {                  
        Calendar calendar = new TARGET();
        var evalDate = calendar.adjust(evaluationDate.ToQuantLibDate());
        var settlementDate = calendar.advance(evalDate, new Period(2, TimeUnit.Days));
        var maturity = maturityDate.ToQuantLibDate();
        var flatInterestRateQuote = new Handle<Quote>(new SimpleQuote(flatInterestRate));
        var curve = new RelinkableHandle<YieldTermStructure>();
        var flatCurve = new FlatForward(evalDate, flatInterestRateQuote, new Actual365Fixed());
        curve.linkTo(flatCurve);
             
        Settings.setEvaluationDate(evalDate);

        // compute hazard rates based on spreads (basis points) and corresponding tenors     
        List<Date> dates = [evalDate];
        List<double> hazardRates = [0.0];
        for (int i = 0; i < tenors.Length; i++)
        {
            var period = tenors[i].ToPeriod();
            var spread = spreadsInBps[i] / 10_000.0;
            var date = evalDate + period;

            // implied hazard rates by building a cds
            var cdsValuation = new MakeCreditDefaultSwap(period, spread)
                                    .withNominal(10_000_000.0)                                    
                                    .value();         
            var hazardRate = cdsValuation.impliedHazardRate(
                                        0.0,
                                        curve,
                                        new Actual365Fixed(),
                                        recoveryRate,
                                        1E-10,
                                        PricingModel.Midpoint
                            );
            dates.Add(date);
            hazardRates.Add(hazardRate);
        }
        hazardRates[0] = hazardRates[1];

        // bootstrap hazard rates
        RelinkableHandle<DefaultProbabilityTermStructure> piecewiseFlatHazardRate = new RelinkableHandle<DefaultProbabilityTermStructure>();
        var hazardRateCurve = new InterpolatedHazardRateCurve<Linear>(dates, hazardRates, new Actual365Fixed());
        piecewiseFlatHazardRate.linkTo(hazardRateCurve);
        piecewiseFlatHazardRate.link.enableExtrapolation();
        
        // build instrument
        var schedule = new Schedule(effectiveDate.ToQuantLibDate(), maturity, new Period(Frequency.Annual), calendar, BusinessDayConvention.Following, BusinessDayConvention.Following, DateGeneration.Rule.Twentieth, false);
        CreditDefaultSwap cds = new CreditDefaultSwap(protectionSide, notional, couponInBps / 10_000.0, schedule, BusinessDayConvention.ModifiedFollowing, new Actual365Fixed());
        cds.setPricingEngine(new MidPointCdsEngine(piecewiseFlatHazardRate, recoveryRate, curve));
        cds.recalculate();

        // results
        var pv = cds.NPV();        
        var fairSpread = 10000.0 * cds.fairSpread();
        var survivalProbabilityPercentage = 100.0 * piecewiseFlatHazardRate.link.survivalProbability(maturity);
        var hazardRatePercentage = 100.0 * piecewiseFlatHazardRate.link.hazardRate(maturity);
        var defaultProbabilityPercentage = 100.0 * piecewiseFlatHazardRate.link.defaultProbability(maturity);
   
        return (pv, fairSpread, survivalProbabilityPercentage, hazardRatePercentage, defaultProbabilityPercentage); 
    }
}

public record struct CreditDefaultSwapPVResult(double PV, double FairSpread, double SurvivalProbabilityPercentage, double HazardRatePercentage, double DefaultProbabilityPercentage)
{
    public static implicit operator (double pv, double fairSpread, double survivalProbabilityPercentage, double hazardRatePercenatge, double defaultProbabilityPercentage)(CreditDefaultSwapPVResult value)
    {
        return (value.PV, value.FairSpread, value.SurvivalProbabilityPercentage, value.HazardRatePercentage, value.DefaultProbabilityPercentage);
    }

    public static implicit operator CreditDefaultSwapPVResult((double pv, double fairSpread, double survivalProbabilityPercentage, double hazardRatePercenatge, double defaultProbabilityPercentage) value)
    {
        return new CreditDefaultSwapPVResult(value.pv, value.fairSpread, value.survivalProbabilityPercentage, value.hazardRatePercenatge, value.defaultProbabilityPercentage);
    }
}
