using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shell.Screens.TradingSignals;

[Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
public class PairsTradingViewModel : Screen
{
    private readonly IEventAggregator eventAggregator;

    [ImportingConstructor]
    public PairsTradingViewModel(IEventAggregator eventAggregator)
    {
        this.eventAggregator = eventAggregator;
        DisplayName = "Pairs Trading mean reverting strategy (Backtesting)";
    }
}
