using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shell.Screens.TradingSignals;

[Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
public class SingleViewModel : Screen
{
    private readonly IEventAggregator eventAggregator;

    [ImportingConstructor]
    public SingleViewModel(IEventAggregator eventAggregator)
    {
        this.eventAggregator = eventAggregator;
        DisplayName = "Mean Reversion strategy (Backtesting)";
    }
}
