using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shell.Screens.MachineLearning
{
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PriceMovementPredictionViewModel : Screen
    {
        private readonly IEventAggregator eventAggregator;

        [ImportingConstructor]
        public PriceMovementPredictionViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            DisplayName = "Price Movement Prediction Classification (MachineLearning)";
        }
    }
}
