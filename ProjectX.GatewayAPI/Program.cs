using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using ProjectX.AnalyticsLib;
using ProjectX.AnalyticsLib.OptionsCalculators;
using ProjectX.AnalyticsLib.Shared;
using ProjectX.AnalyticsLibNativeShim;
using ProjectX.Core;
using ProjectX.Core.Services;
using ProjectX.GatewayAPI.BackgroundServices;
using ProjectX.GatewayAPI.ExternalServices;
using ProjectX.GatewayAPI.Processors;
using ProjectX.MarketData;
using System.Runtime.CompilerServices;

//https://learn.microsoft.com/en-us/aspnet/core/migration/50-to-60?view=aspnetcore-8.0&tabs=visual-studio#smhm

var builder = WebApplication.CreateBuilder(args);

var startup = new Startup(builder.Configuration);

startup.ConfigureServices(builder.Services);

var app = builder.Build();

startup.Configure(app, app.Environment);

app.Run();

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        // Add services to the container.
        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new Array2DConverter());
        });
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new Array2DConverter());
        });

        // quant pricers
        services.TryAddSingleton<IAPI, API>();
        services.TryAddScoped<IBlackScholesCSharpPricer, BlackScholesOptionsPricer>();
        services.TryAddScoped<IBlackScholesCppPricer, BlackScholesCppOptionsPricerWrapper>();
        services.TryAddScoped<IMonteCarloCppOptionsPricer, MonteCarloCppOptionsPricerWrapper>();
        // others
        services.TryAddScoped<IOptionsPricingModel, OptionsPricingModel>();
        services.AddSingleton<IFXSpotPricer, FXSpotPricer>();
        services.AddSingleton<IFXSpotPriceStream, RandomFXSpotPriceStream>();
        services.AddSingleton<IFXMarketService, FXMarketService>();
        services.AddSingleton<PricingTasksChannel>();
        services.AddSingleton<FXTasksChannel>();
        services.TryAddScoped<IPricingTasksProcessor, PricingTasksProcessor>();

        services.AddOptions();        
        services.Configure<ApiClientOptions>(options => options.BaseAddress = Configuration.GetSection("ExternalServices")["ProjectXUrl"]);
        services.Configure<RandomFXSpotPriceStreamOptions>(options =>
        {
            options.RawSpreadInPips = Convert.ToDecimal(Configuration.GetSection("FX")["RawSpreadInPips"]);
            options.IntervalBetweenSends = Convert.ToInt32(Configuration.GetSection("FX")["IntervalBetweenSends"]);
        });

        services.AddHttpClient<IPricingResultsApiClient, PricingResultsApiClient>();
        services.AddHostedService<PricingTasksService>();
        services.AddHostedService<FXPricingService>();
        services.AddSignalR(config => config.EnableDetailedErrors = true)
                        .AddMessagePackProtocol();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();            
        }

        // Configure the HTTP request pipeline.

        app.UseHttpsRedirection();        

        app.UseAuthorization();

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.Map("/", () => "Hello there.");
            endpoints.MapControllers();
            endpoints.MapHub<StreamHub>("/streamHub");
        });

        var appLifetime = app.ApplicationServices.GetService<IHostApplicationLifetime>();
        appLifetime.ApplicationStopping.Register(() =>
        {
            var nativeApi = app.ApplicationServices.GetService<IAPI>();
            if (nativeApi != null)
            {
                Console.WriteLine("Releasing native libraries");
                nativeApi.Shutdown();
            }
        });        
    }
}