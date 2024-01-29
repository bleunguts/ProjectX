using ProjectX.Core.Strategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Core;

public class SignalBuilder
{
    private const int MaxSignals = 10;
    private static readonly Random _random = new();

    private readonly string _ticker;
    private DateTime _currentDate;

    public SignalBuilder(string ticker, DateTime? startDate = null)
    {
        this._ticker = ticker;
        this._currentDate = startDate ?? DateTime.Now.AddDays(-90);
    }

    public PriceSignal NewSignal(double v)
    {
        return NewSignal((decimal)v);
    }

    public PriceSignal NewSignal(decimal signal, int low = 10, int high = 20)
    {
        var price = _random.Next(low, high) + (decimal)_random.NextDouble();
        var pricePredicted = price + (decimal)_random.NextDouble();        
        var lowerBand = low - (decimal)_random.NextDouble();
        var upperBand = high + (decimal)_random.NextDouble();

        var entity = new PriceSignal()
        {
            Ticker = _ticker,
            Date = _currentDate,
            Price = price,
            PricePredicted = pricePredicted,
            LowerBand = lowerBand,
            UpperBand = upperBand,
            Signal = signal
        };
        
        MoveNext();
        return entity;
    }

    public IEnumerable<PriceSignal> Build(int low, int high)
    {
        var signals = new List<PriceSignal>();
        var date = _currentDate;
        for (int i = 0; i <= MaxSignals; i++)
        {
            var price = _random.Next(low, high);
            var pricePredicted = price + _random.NextDouble();
            var signal = price + _random.NextDouble();
            var lowerBand = low - _random.NextDouble();
            var upperBand = high + _random.NextDouble();
            signals.Add(new PriceSignal
            {
                Ticker = _ticker,
                Price = price,
                Date = date,
                LowerBand = (decimal)lowerBand,
                UpperBand = (decimal)upperBand,
                PricePredicted = (decimal)pricePredicted,
                Signal = (decimal)signal
            });
            MoveNext();
        }
        return signals;
    }

    private void MoveNext()
    {
        _currentDate = _currentDate.AddDays(1);
    }    
}
