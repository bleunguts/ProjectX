using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shell.Screens.MarketPrices
{
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class EquitiesMarketViewModel : Screen
    {
        private readonly IEventAggregator eventAggregator;

        [ImportingConstructor]
        public EquitiesMarketViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            DisplayName = "Equities (MarketPrices)";
        }
    }
}
