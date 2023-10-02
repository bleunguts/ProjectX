using System.Collections;
using System.Linq;

namespace ProjectX.Core.Services
{
    public class OptionsPricingResults: IEnumerable<(double maturity, OptionGreeksResult optionsGreeks)>
    {
        private readonly Dictionary<double, OptionGreeksResult> _greeksPerMaturity = new Dictionary<double, OptionGreeksResult>();
        public void AddResult(double maturity, OptionGreeksResult greeks) => _greeksPerMaturity.TryAdd(maturity, greeks);

        public IEnumerator<(double maturity, OptionGreeksResult optionsGreeks)> GetEnumerator()
        {
            return ConvertToTargetOutput().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => _greeksPerMaturity.Select(x => (x.Key, x.Value)).GetEnumerator();

        public int Count => _greeksPerMaturity.Count;

        public override string ToString() => $"{Count} results, maturties: {string.Join(',',ConvertToTargetOutput().Select(x=>x.Key))}, prices: {string.Join(',', ConvertToTargetOutput().Select(x => x.Value.price))}";

        private IEnumerable<(double Key, OptionGreeksResult Value)> ConvertToTargetOutput() => _greeksPerMaturity.Select(x => (x.Key, x.Value));
    } 
}
