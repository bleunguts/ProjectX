using Caliburn.Micro;
using Microsoft.Extensions.Logging;
using ProjectX.Core;
using ProjectX.Core.MarketData;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;
using System.Collections.ObjectModel;

namespace Shell.Screens.FX
{
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class FXPricerViewModel : Screen
    {
        private readonly IEventAggregator _events;
        private readonly IFXMarketService _fXMarketService;

        [ImportingConstructor]
        public FXPricerViewModel(IEventAggregator events, IFXMarketService fXMarketService)
        {
            _events = events;
            _fXMarketService = fXMarketService;
            DisplayName = "FXPricer (FX)"; 
        }

        #region Bindable Properties 
        private string _status = "Ready.";
        private string _priceStream = "N/A";
        private ObservableCollection<string> _prices = new ObservableCollection<string>();
        private string _latestPrice = "N/A";
        private int _notional = 1000;
        private IEnumerable<string> _currency = new string[] 
        { 
            "GBPUSD",
            "EURUSD"
        };
        private string selectedCurrency = "EURUSD";
        private string clientName = "ProjectX Trader";      
        private string _latestPriceTimeStamp = "---";

        public IEnumerable<string> Currency
        {
            get { return _currency; }
            set { _currency = value; NotifyOfPropertyChange(() => Currency); }
        }

        public string SelectedCurrency
        {
            get { return selectedCurrency; }
            set { selectedCurrency = value; NotifyOfPropertyChange(() => SelectedCurrency); }
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

        public ObservableCollection<string> Prices
        {
            get { return _prices; }
            set { _prices = value; NotifyOfPropertyChange(() => Prices); }
        }     

        public string LatestPrice
        {
            get { return _latestPrice; }
            set { _latestPrice = value; NotifyOfPropertyChange(() => LatestPrice); }
        }
        public string LatestPriceTimeStamp
        {
            get { return _latestPriceTimeStamp; }
            set { _latestPriceTimeStamp = value; NotifyOfPropertyChange(() => LatestPriceTimeStamp); }
        }
            
        public int Notional
        {
            get { return _notional; }
            set { _notional = value; NotifyOfPropertyChange(() => Notional); }
        }

        public string ClientName
        {
            get { return clientName; }
            set { clientName = value; NotifyOfPropertyChange(() => ClientName); }
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

        public void ClearHistory() => ClearPricesList();

        public void Subscribe()
        {
            if (_currentDisposableStream != null)
            {
                _currentDisposableStream.Dispose();
            }
            var selectedCurrency = SelectedCurrency ?? throw new ArgumentNullException(nameof(SelectedCurrency));
            _currentDisposableStream = _fXMarketService.StreamSpotPricesFor(new ProjectX.Core.SpotPriceRequest(selectedCurrency, ClientName))
                .                                        Subscribe(priceResponse => UpdatePrice(priceResponse));                   
        }

        void UpdatePrice(Timestamped<SpotPriceResponse> response)
        {
            try
            {
                var latestPrice = FormatPrice(response.Value.SpotPrice);
                LatestPrice = latestPrice;
                LatestPriceTimeStamp = response.Timestamp.ToLocalTime().ToString();
                PriceStream = response.Value.SpotPrice.CurrencyPair;

                App.Current.Dispatcher.Invoke((System.Action)delegate
                {
                    AddPriceToPricesList(latestPrice);                
                });

                Status = $"[{DateTime.Now.ToLocalTime()}] New Spot Price arrived at {response.Timestamp.ToLocalTime()} for {response.Value.SpotPrice.CurrencyPair}." + Environment.NewLine + Status;
            }
            catch (Exception exp)
            {
                MessageBox.Show(string.Format("Cannot interpret PriceResponse, Reason:'{0}'", exp.Message, "Error"));
            }

            string FormatPrice(SpotPrice price) => $"{price.BidPrice.ToString("#.00000")}/{price.AskPrice.ToString("#.00000")}";
        }

        private void ClearPricesList()
        {
            _prices.Clear();
            NotifyOfPropertyChange(() => Prices);
        }
        private void AddPriceToPricesList(string price)
        {
            _prices.Insert(0, price);            
            NotifyOfPropertyChange(() => Prices);
        }

        public void Unsubscribe() 
        {
            _fXMarketService.UnStream(SelectedCurrency);
            if(_currentDisposableStream != null)
            {
                _currentDisposableStream.Dispose();
                _currentDisposableStream = null;
            }            
        }

        private IDisposable? _currentDisposableStream;
    }
}
