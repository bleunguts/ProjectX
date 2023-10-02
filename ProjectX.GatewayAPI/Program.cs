using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using ProjectX.Core.Services;
using ProjectX.GatewayAPI.BackgroundServices;
using ProjectX.GatewayAPI.ExternalServices;
using ProjectX.GatewayAPI.Processors;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.TryAddScoped<IPricingTasksProcessor, PricingTasksProcessor>();
builder.Services.TryAddScoped<IBlackScholesOptionsPricingModel, BlackScholesOptionsPricingModel>();
builder.Services.AddSingleton<PricingTasksChannel>();

builder.Services.AddOptions();
IConfiguration config = builder.Configuration;
builder.Services.Configure<ProjectXApiClientOptions>(options => options.BaseAddress = config.GetSection("ExternalServices")["ProjectXUrl"]);
//builder.Services.Configure<ProjectXApiClientOptions>(options => options.BaseAddress = "https://localhost:7029");

builder.Services.AddHttpClient<IPricingResultsApiClient, PricingResultsApiClient>();
builder.Services.AddHostedService<PricingTasksService>();


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
