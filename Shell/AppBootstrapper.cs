using Caliburn.Micro;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectX.Core.MarketData;
using ProjectX.Core.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Windows;

namespace Shell
{
    public class AppBootstrapper : BootstrapperBase
    {
        private CompositionContainer container;

        public AppBootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {            
            List<ComposablePartCatalog> catalogues = new List<ComposablePartCatalog>();
            catalogues.Add(new DirectoryCatalog(".", "ProjectX*.Dll"));
            catalogues.AddRange(AssemblySource.Instance.Select(x => new AssemblyCatalog(x)).OfType<ComposablePartCatalog>());
            var catalog = new AggregateCatalog(catalogues);

            container = new CompositionContainer(catalog);
            var batch = new CompositionBatch();
            batch.AddExportedValue<IWindowManager>(new WindowManager());
            batch.AddExportedValue<IEventAggregator>(new EventAggregator());
            batch.AddExportedValue(container);
            batch.AddExportedValue<ILogger<FXMarketService>>(new NullLogger<FXMarketService>());
            batch.AddExportedValue<IFXSpotPriceStream>(new RandomFXSpotPriceStream(2, 500));

            container.Compose(batch);                        
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            string contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
            var exports = container.GetExportedValues<object>(contract);

            if (exports.Any())
                return exports.First();

            throw new Exception($"Could not locate any instance of contract {contract}.");
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
        }

        protected override void BuildUp(object instance)
        {
            container.SatisfyImportsOnce(instance);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {                        
            DisplayRootViewFor<ShellViewModel>();
        }
    }
}
