using System.ComponentModel.DataAnnotations;
using LiveChartsCore.SkiaSharpView.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProjectX.Core;
using ProjectX.MarketData;
using ProjectX.MarketData.Cache;
using ShellX.Pages;
using ShellX.Services;
using ShellX.ViewModels;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace ShellX
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseSkiaSharp()
                .UseLiveCharts()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            using (var stream = FileSystem.OpenAppPackageFileAsync("appsettings.json").GetAwaiter().GetResult())
            {
                builder.Configuration.AddJsonStream(stream);
            }

            RegisterServices(builder);

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        private static void RegisterServices(MauiAppBuilder builder)
        {
            var services = builder.Services;

            var gatewayOptions = new GatewayApiClientOptions();
            builder.Configuration.GetSection(nameof(GatewayApiClientOptions)).Bind(gatewayOptions);
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                // Inside the emulator "localhost" is the emulator itself; the Gateway runs on the host PC.
                gatewayOptions.BaseUrl = GatewayAddress.ForAndroidEmulator(gatewayOptions.BaseUrl);
                gatewayOptions.SignalRUrl = GatewayAddress.ForAndroidEmulator(gatewayOptions.SignalRUrl);
            }
            var results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(gatewayOptions, new ValidationContext(gatewayOptions), results, validateAllProperties: true))
                throw new InvalidOperationException($"GatewayApiClientOptions config is invalid! {string.Join(", ", results)}");
            services.AddSingleton(Options.Create(gatewayOptions));

            var marketDataOptions = new MarketDataOptions();
            builder.Configuration.GetSection(nameof(MarketDataOptions)).Bind(marketDataOptions);
            services.AddSingleton(Options.Create(marketDataOptions));

            services.AddTransient<IHubConnector, HubConnector>();
            services.AddTransient<IGatewayApiClient, GatewayApiClient>();

            // Created on first use so the app starts without the fmpapikey environment variable;
            // only the machine learning screen needs it.
            services.AddSingleton<IMachineLearningApiClient>(sp =>
            {
                var cacheFile = sp.GetRequiredService<IOptions<MarketDataOptions>>().Value.CacheFile;
                if (!Path.IsPathRooted(cacheFile))
                    cacheFile = Path.Combine(FileSystem.AppDataDirectory, cacheFile);

                var apiKey = new EnvironmentVariableLoader().FromEnvironmentVariable("fmpapikey");
                var stockMarketSource = new FileBackedStockMarketDataSource(
                    new FMPStockMarketSource(Options.Create(new FMPStockMarketSourceOptions { ApiKey = apiKey })),
                    Options.Create(new FileBackedStoreMarketDataSourceOptions { Filename = cacheFile }));
                return new MachineLearningApiClient(stockMarketSource);
            });

            services.AddSingleton<StatusService>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<AppShell>();
            services.AddTransient<BlackScholesViewModel>();
            services.AddTransient<BlackScholesPage>();
        }
    }
}
