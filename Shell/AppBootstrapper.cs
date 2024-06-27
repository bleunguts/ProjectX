using Caliburn.Micro;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using ProjectX.Core;
using ProjectX.Core.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Windows;
using static SkiaSharp.HarfBuzz.SKShaper;

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
            batch.AddExportedValue<ILogger<eFXTradeExecutionService>>(new NullLogger<eFXTradeExecutionService>());
            batch.AddExportedValue<ILogger<GatewayApiClient>>(new NullLogger<GatewayApiClient>());
            
            HostApplicationBuilder builder = Host.CreateApplicationBuilder();                                    
            GatewayApiClientOptions options = new();
            builder.Configuration.GetSection(nameof(GatewayApiClientOptions)).Bind(options);
            var results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(options, new ValidationContext(options), results))
                throw new Exception($"GatewayApiClientOptions config is invalid! {string.Join(", ", results)}");

            batch.AddExportedValue<IOptions<GatewayApiClientOptions>>(Options.Create(options));    
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
