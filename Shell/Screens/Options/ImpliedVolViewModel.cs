using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shell.Screens.Options
{
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ImpliedVolViewModel : Screen
    {
        private readonly IEventAggregator eventAggregator;

        [ImportingConstructor]
        public ImpliedVolViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            DisplayName = "Implied Volatility (Optons)";
        }
    }
}
