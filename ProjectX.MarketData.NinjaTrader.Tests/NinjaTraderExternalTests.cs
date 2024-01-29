using NUnit.Framework;
using System.Threading.Tasks;
using System;
using NinjaTrader.Client;

namespace ProjectX.MarketData.NinjaTrader.Tests
{
    public class NinjaTraderExternalTests
    {
        /*
         * An external project using the NinjaTrader.Client.dll should target .NET 4.8 for compatibility with NT8 8.0.23.0 and later.
         * To test the Ninja8API, you must enable the AT Interface option in NinjaTrader 8 by opening NInjaTrader and going to the Tools > Options > Automated Trading Interface tab. Enable the 'AT Interface' checkbox option and click OK.
         * After enabling the 'AT Interface' option, restart NinjaTrader.
         * https://forum.ninjatrader.com/forum/ninjatrader-8/add-on-development/1260152-nt8-api-ninjatrader-client-dll-demo
         */
        private Client _client = new Client();

        [Test]
        public async Task SubscribeTo()
        {
            const string instrumentReceive = "BTCUSD";
            try
            {
                //int setup = _client.SetUp("localhost", 36973);
                //Console.WriteLine($"{DateTime.Now} | setup: {setup}");

                int connect = _client.Connected(1);
                Console.WriteLine($"{DateTime.Now} | connect: {connect}");

                var mdSubscribed = _client.SubscribeMarketData(instrumentReceive);
                Console.WriteLine($"{DateTime.Now} | subscribed: {instrumentReceive}");

                if (mdSubscribed != 0)
                {
                    throw new Exception($"Subscription error to {instrumentReceive}, error code:{mdSubscribed}");
                }

                for (int i = 0; i < 3; i++)
                {
                    var askPriceReceive = _client.MarketData(instrumentReceive, 2);
                    var bidPriceReceive = _client.MarketData(instrumentReceive, 1);
                    var lastPriceReceive = _client.MarketData(instrumentReceive, 0);

                    Console.WriteLine($"{DateTime.Now} | {instrumentReceive} | Last: {lastPriceReceive}, Ask: {askPriceReceive}, Bid: {bidPriceReceive}");
                    await Task.Delay(1000);
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine($"Error occurred {exp.Message}");
            }
            finally
            {
                var mdSubscribed = _client.UnsubscribeMarketData(instrumentReceive);
                if (mdSubscribed != 0)
                {
                    Console.WriteLine($"Unsubscription error to {instrumentReceive}, error code:{mdSubscribed}");
                }
                _client.TearDown();
            }
        }
    }
}