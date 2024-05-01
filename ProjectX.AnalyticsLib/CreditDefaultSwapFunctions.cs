using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using QLNet;

namespace ProjectX.AnalyticsLib;
/// <summary>
/// For CDS buyers or sellers the present value of a CDS contract is all what they care about.
/// For quants, we want to calculate accrual amount, risk annuity (DV01), dirty price, clean price @ $100
/// </summary>
public class CreditDefaultSwapFunctions
{
    public static CreditDefaultSwapPriceResult Price(
        DateTime evalDate,
        DateTime effectiveDate,
        DateTime maturityDate,
        double[] spreadsInBps,
        string[] tenors,
        double recoveryRate,
        double couponInBps,
        Frequency couponFrequency,
        Protection.Side protectionSide, 
        double flatInterestRate)
    {
        // we want to know the dirty price and clean price for a notional of $100, just like a bond with a face value of $100
        var cds = PV(evalDate, effectiveDate, maturityDate, spreadsInBps, tenors, recoveryRate, couponInBps, 100, protectionSide, flatInterestRate);
        double upfront = cds.PV;
        double dirtyPrice = protectionSide switch
        {
            Protection.Side.Buyer => 100 - upfront,
            Protection.Side.Seller => 100 + upfront,
            _ => throw new NotImplementedException(),
        };           
        int numDays = effectiveDate.Subtract(evalDate).Days + 1;
        double accrual = couponInBps * numDays / 360.0 / 100.0;
        double cleanPrice = protectionSide switch
        {
            Protection.Side.Buyer => dirtyPrice + accrual,
            Protection.Side.Seller => dirtyPrice - accrual,
            _ => throw new NotImplementedException(),
        };

        // compute risky annuity (dv01)
        // the risky duration (dv01) relates to a trade and is the change in mark-to-market of a CDS trade for a 1 basis point parallel shift in spreads.        
    
        // for a par trade Sinitial = SCurrent risky duration is equal to the risky annuity.  
        double cds2coupon = cds.FairSpread + 1;
        var cds2 = PV(evalDate, effectiveDate, maturityDate, spreadsInBps, tenors, recoveryRate, cds2coupon, 100, protectionSide, flatInterestRate);
        double riskyAnnuity = cds2.PV;
        return (cleanPrice, dirtyPrice, riskyAnnuity);
    }

    /// <summary>
    /// Uses Flat Interest Rate Curve. Other possible implementations include market ISDA rate curve (PiecewiseLogCubic)
    /// </summary>    
    public static CreditDefaultSwapPVResult PV(
        DateTime evaluationDate, 
        DateTime effectiveDate, 
        DateTime maturityDate, 
        double[] spreadsInBps, 
        string[] tenors, 
        double recoveryRate, 
        double couponInBps, 
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

public record struct CreditDefaultSwapPriceResult(double cleanPrice, double dirtyPrice, double riskyAnnuity)
{
    public static implicit operator (double cleanPrice, double dirtyPrice, double riskyAnnuity)(CreditDefaultSwapPriceResult value)
    {
        return (value.cleanPrice, value.dirtyPrice, value.riskyAnnuity);
    }

    public static implicit operator CreditDefaultSwapPriceResult((double cleanPrice, double dirtyPrice, double riskyAnnuity) value)
    {
        return new CreditDefaultSwapPriceResult(value.cleanPrice, value.dirtyPrice, value.riskyAnnuity);
    }
}