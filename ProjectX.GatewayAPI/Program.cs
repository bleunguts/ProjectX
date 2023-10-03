using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using ProjectX.Core;
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
builder.Services.TryAddScoped<IBlackScholesOptionsPricingModel, BlackScholesOptionsPricingModel>();
builder.Services.AddSingleton<PricingTasksChannel>();

builder.Services.AddOptions();
IConfiguration config = builder.Configuration;
builder.Services.Configure<ApiClientOptions>(options => options.BaseAddress = config.GetSection("ExternalServices")["ProjectXUrl"]);

builder.Services.AddHttpClient<IPricingResultsApiClient, PricingResultsApiClient>();
builder.Services.AddHostedService<PricingTasksService>();
builder.Services.AddSignalR(config => config.EnableDetailedErrors = true)
                .AddJsonProtocol(options =>
                {
                    options.PayloadSerializerOptions.Converters.Add(new Array2DConverter());
                });


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseRouting();

app.UseEndpoints(endpoints => endpoints.MapHub<StreamHub>("/streamHub"));

app.Run();
