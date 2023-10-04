﻿using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shell.Screens.FixedIncome
{
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class BondViewModel : Screen
    {
        private readonly IEventAggregator eventAggregator;

        [ImportingConstructor]
        public BondViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            DisplayName = "BondPricer (FixedIncome)";
        }
    }
}
