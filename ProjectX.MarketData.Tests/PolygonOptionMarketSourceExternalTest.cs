using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.MarketData.Tests
{
    public class PolygonOptionMarketSourceExternalTest
    {
        [Test]
        public void foo()
        {
            const string filename = "C:\\Users\\bleun\\Documents\\_\\2. [Work] Professional Profile\\Freelancing\\ProjectX\\OptionChain.json";
            var o = JsonConvert.DeserializeObject<OptionChain>(File.ReadAllText(filename));            
            var newOptionChain = new OptionChain()
            {
                s = o.s,
                optionSymbol = A(o.optionSymbol.First()),
                underlying  = A(o.underlying.First()),  
                expiration = A(o.expiration.First()),   
                side= A(o.side.First()),
                strike = A(o.strike.First()),
                firstTraded = A(o.firstTraded.First()),
                dte = A(o.dte.First()),
                bid = A(o.bid.First()),
                bidSize = A(o.bidSize.First()),
                mid = A(o.mid.First()),
                ask = A(o.ask.First()),
                askSize = A(o.askSize.First()),
                last = A(o.last.First()),
                openInterest = A(o.openInterest.First()),
                volume = A(o.volume.First()),
                inTheMoney = A(o.inTheMoney.First()),
                intrinsicValue = A(o.intrinsicValue.First()),
                extrinsicValue = A(o.extrinsicValue.First()),
                underlyingPrice = A(o.underlyingPrice.First()),
                iv =    A(o.iv.First()),
                delta =     A(o.delta.First()),
                gamma = A(o.gamma.First()),
                theta = A(o.theta.First()),
                vega = A(o.vega.First()),
                rho = A(o.rho.First())
            };
            
            var json = JsonConvert.SerializeObject(newOptionChain);
            Console.WriteLine(json);
        }

        static IEnumerable<T> A<T>(T t)
        {
            return new[] { t };
        }
    }
}
