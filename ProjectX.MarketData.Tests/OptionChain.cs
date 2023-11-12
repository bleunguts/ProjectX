namespace ProjectX.MarketData.Tests
{
    public class OptionChain
    {
        public string s { get; set; }
        public IEnumerable<string> optionSymbol { get; set; }
        public IEnumerable<string> underlying { get; set; }
        public IEnumerable<string> expiration { get; set; }
        public IEnumerable<string> side { get; set; }
        public IEnumerable<double> strike { get; set; }
        public IEnumerable<string> firstTraded { get; set; }
        public IEnumerable<int> dte { get; set; }
        public IEnumerable<double> bid { get; set; }
        public IEnumerable<double> bidSize { get; set; }
        public IEnumerable<double> mid { get; set; }
        public IEnumerable<double> ask { get; set; }
        public IEnumerable<double> askSize { get; set; }
        public IEnumerable<double> last { get; set; }
        public IEnumerable<double> openInterest { get; set; }
        public IEnumerable<double> volume { get; set; }
        public IEnumerable<bool> inTheMoney { get; set; }
        public IEnumerable<double> intrinsicValue { get; set; }
        public IEnumerable<double> extrinsicValue { get; set; }
        public IEnumerable<double> underlyingPrice { get; set; }
        public IEnumerable<double?> iv { get; set; }
        public IEnumerable<double?> delta { get; set; }
        public IEnumerable<double?> gamma { get; set; }
        public IEnumerable<double?> theta { get; set; }
        public IEnumerable<double?> vega { get; set; }
        public IEnumerable<double?> rho { get; set; }

    }
}
