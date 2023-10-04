using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using ProjectX.AnalyticsLib;
using ProjectX.Core;
using ProjectX.Core.MarketData;
using ProjectX.Core.Services;
using ProjectX.GatewayAPI.BackgroundServices;
using ProjectX.GatewayAPI.ExternalServices;
using ProjectX.GatewayAPI.Processors;
using System.Runtime.CompilerServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new Array2DConverter());
});
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new Array2DConverter());
});

builder.Services.TryAddScoped<IPricingTasksProcessor, PricingTasksProcessor>();
builder.Services.TryAddScoped<IBlackScholesOptionsPricingModel, BlackScholesOptionsPricer>();
builder.Services.AddSingleton<IFXSpotPricer,FXSpotPricer>();
builder.Services.AddSingleton<IFXSpotPriceStream, RandomFXSpotPriceStream>();
builder.Services.AddSingleton<IFXMarketService, FXMarketService>();
builder.Services.AddSingleton<PricingTasksChannel>();
builder.Services.AddSingleton<FXTasksChannel>();

builder.Services.AddOptions();
IConfiguration config = builder.Configuration;
builder.Services.Configure<ApiClientOptions>(options => options.BaseAddress = config.GetSection("ExternalServices")["ProjectXUrl"]);
builder.Services.Configure<RandomFXSpotPriceStreamOptions>(options =>
{
    options.RawSpreadInPips = Convert.ToDecimal(config.GetSection("FX")["RawSpreadInPips"]);
    options.IntervalBetweenSends = Convert.ToInt32(config.GetSection("FX")["IntervalBetweenSends"]);
});

builder.Services.AddHttpClient<IPricingResultsApiClient, PricingResultsApiClient>();
builder.Services.AddHostedService<PricingTasksService>();
builder.Services.AddHostedService<FXPricingService>();
builder.Services.AddSignalR(config => config.EnableDetailedErrors = true)
                .AddMessagePackProtocol();              

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseRouting();

app.UseEndpoints(endpoints => endpoints.MapHub<StreamHub>("/streamHub"));

app.Run();
