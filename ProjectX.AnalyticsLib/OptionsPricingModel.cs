using ProjectX.AnalyticsLib.Shared;
using ProjectX.Core.Analytics;
using ProjectX.Core.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Core.Services
{
    public interface IOptionsPricingModel
    {        
        PlotOptionsPricingResult PlotGreeks(PlotOptionsPricingRequest pricingRequest);
        OptionsPricingByMaturityResults Price(OptionsPricingByMaturitiesRequest request);
    }

    [Export(typeof(IOptionsPricingModel)), PartCreationPolicy(CreationPolicy.NonShared)]    
    public class OptionsPricingModel : IOptionsPricingModel
    {
        private readonly IOptionsGreeksCalculator _blackScholesCSharpPricer;
        private readonly IOptionsGreeksCalculator _blackScholesCppPricer;
        private readonly IOptionsGreeksCalculator _monteCarloOptionsPricerCpp;        

        [ImportingConstructor]
        public OptionsPricingModel(IBlackScholesCSharpPricer csharpPricer, IBlackScholesCppPricer cppPricer, IMonteCarloCppOptionsPricer cppmcPricer)
        {
            _blackScholesCSharpPricer = csharpPricer;
            _blackScholesCppPricer = cppPricer;
            _monteCarloOptionsPricerCpp = cppmcPricer;        
        }
        public OptionsPricingByMaturityResults Price(OptionsPricingByMaturitiesRequest request)
        {
            (int timeSlices, OptionType optionType, double spot, double strike, double rate, double carry, double vol, OptionsPricingCalculatorType calculatorType) = request;
            
            var results = new List<(double, OptionGreeksResult)>();                  
            var timeIt = Stopwatch.StartNew();
            for (int i = 0; i < timeSlices; i++)
            {
                // break out into 10 time slices until maturity
                double maturity = (i + 1.0) / 10.0;

                // price option
                double price = Calc(calculatorType).PV(optionType, spot, strike, rate, carry, maturity, vol);
                double delta = Calc(calculatorType).Delta(optionType, spot, strike, rate, carry, maturity, vol);
                double gamma = Calc(calculatorType).Gamma(optionType, spot, strike, rate, carry, maturity, vol);
                double theta = Calc(calculatorType).Theta(optionType, spot, strike, rate, carry, maturity, vol);
                double rho = Calc(calculatorType).Rho(optionType, spot, strike, rate, carry, maturity, vol);
                double vega = Calc(calculatorType).Vega(optionType, spot, strike, rate, carry, maturity, vol);

                // return price & greeks
                var greeks = new OptionGreeksResult(
                    price,
                    delta,
                    gamma,
                    theta,
                    rho,
                    vega);                
                results.Add((maturity, greeks));                
            }
            timeIt.Stop();
            var resultPairs = new List<MaturityAndOptionGreeksResultPair>();
            foreach (var result in results)
            {
                resultPairs.Add(new MaturityAndOptionGreeksResultPair(result.Item1, result.Item2));
            }
            return new OptionsPricingByMaturityResults(request.Id, resultPairs, new AuditTrail(calculatorType, timeIt.ElapsedMilliseconds));            
        }

        private IOptionsGreeksCalculator Calc(OptionsPricingCalculatorType calculatorType)
        {
            return calculatorType switch
            {
                OptionsPricingCalculatorType.OptionsPricer => _blackScholesCSharpPricer,
                OptionsPricingCalculatorType.OptionsPricerCpp => _blackScholesCppPricer,                
                OptionsPricingCalculatorType.MonteCarloCppPricer => _monteCarloOptionsPricerCpp,
                _ => throw new NotImplementedException(nameof(calculatorType)),
            };
        }

        public PlotOptionsPricingResult PlotGreeks(PlotOptionsPricingRequest request)
        {
            (OptionGreeks greekType, OptionType optionType, double strike, double rate, double carry, double vol, OptionsPricingCalculatorType calculatorType) = request;
            double xmin = 0.1;
            double xmax = 3.0;
            double ymin = 10;
            double ymax = 190;
            var XLimitMin = xmin;
            var YLimitMin = ymin;
            var XSpacing = 0.1;
            var YSpacing = 5;
            var XNumber = Convert.ToInt16((xmax - xmin) / XSpacing) + 1;
            var YNumber = Convert.ToInt16((ymax - ymin) / YSpacing) + 1;

            MyPoint3D[,] pts = new MyPoint3D[XNumber, YNumber];
            double zmin = 10_000;
            double zmax = -10_000;
            for (int i = 0; i < XNumber; i++)
            {
                for (int j = 0; j < YNumber; j++)
                {
                    double x = XLimitMin + i * XSpacing;
                    double y = YLimitMin + j * YSpacing;
                    double z = Double.NaN;
                    var spot = y;
                    var maturity = x;
                    switch (greekType)
                    {
                        case OptionGreeks.Delta:
                            z = Calc(calculatorType).Delta(optionType, spot, strike, rate, carry, maturity, vol);
                            break;
                        case OptionGreeks.Gamma:
                            z = Calc(calculatorType).Gamma(optionType, spot, strike, rate, carry, maturity, vol);
                            break;
                        case OptionGreeks.Theta:
                            z = Calc(calculatorType).Theta(optionType, spot, strike, rate, carry, maturity, vol);
                            break;
                        case OptionGreeks.Rho:
                            z = Calc(calculatorType).Rho(optionType, spot, strike, rate, carry, maturity, vol);
                            break;
                        case OptionGreeks.Vega:
                            z = Calc(calculatorType).Vega(optionType, spot, strike, rate, carry, maturity, vol);
                            break;
                        case OptionGreeks.Price:
                            z = Calc(calculatorType).PV(optionType, spot, strike, rate, carry, maturity, vol);
                            break;
                    }
                    if (!double.IsNaN(z))
                    {
                        pts[i, j] = new MyPoint3D(x, y, z);
                        zmin = Math.Min(zmin, z);
                        zmax = Math.Max(zmax, z);
                    }
                }
            }            
            var plotResults = new PlotResults
            {
                PointArray = pts,
                zmin = zmin,
                zmax = zmax,
                XLimitMin = XLimitMin,
                YLimitMin = YLimitMin,
                XSpacing = XSpacing,
                YSpacing = YSpacing,
                XNumber = XNumber,
                YNumber = YNumber,
            };

            return new PlotOptionsPricingResult(request, plotResults);
        }

    
    }
}
