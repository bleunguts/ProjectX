using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shell.Screens.FX
{
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class FXPricerViewModel : Screen
    {
        private readonly IEventAggregator _events;        

        [ImportingConstructor]
        public FXPricerViewModel(IEventAggregator events)
        {
            _events = events;
            DisplayName = "FXPricer (FX)"; 
        }

        #region Bindable Properties 
        private string _status = "Ready.";
        private string _priceStream = "N/A";
        private IEnumerable<double> _prices = new List<double>();
        private string _latestPrice = "N/A";
        private int _notional = 1000;
        private IEnumerable<string> _currencies = new string[] 
        { 
            "GBP/USD",
            "EUR/USD"
        };

        public IEnumerable<string> Currencies
        {
            get { return _currencies; }
            set { _currencies = value; NotifyOfPropertyChange(() => Currencies); }
        }
       
        public string Status
        {
            get { return _status; }
            set { _status = value; NotifyOfPropertyChange(() => Status); }
        }

        public string PriceStream
        {
            get { return _priceStream; }
            set { _priceStream = value; NotifyOfPropertyChange(() => PriceStream); }
        }

        public IEnumerable<double> Prices
        {
            get { return _prices; }
            set { _prices = value; NotifyOfPropertyChange(() => Prices); }
        }

        public string LatestPrice
        {
            get { return _latestPrice; }
            set { _latestPrice = value; NotifyOfPropertyChange(() => LatestPrice); }
        }

        public int Notional
        {
            get { return _notional; }
            set { _notional = value; NotifyOfPropertyChange(() => Notional); }
        }
        #endregion

        public void BuyTrade()
        {
            throw new NotImplementedException();
        }

        public void SellTrade()
        {
            throw new NotImplementedException();
        }

        public void ClearHistory()
        {
            throw new NotImplementedException();
        }

        public void Subscribe()
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe() { throw new NotImplementedException(); }
    }
}
