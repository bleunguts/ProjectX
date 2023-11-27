using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProjectX.GatewayAPI.Processors;

namespace ProjectX.GatewayAPI.BackgroundServices;

public class PricingTasksService : BackgroundService
{
    private readonly ILogger<PricingTasksService> _logger;
    private readonly PricingTasksChannel _pricingTasksChannel;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;

    public PricingTasksService(
        ILogger<PricingTasksService> logger,        
        PricingTasksChannel pricingTasksChannel,
        IServiceProvider serviceProvider,
        IHostApplicationLifetime hostApplicationLifetime)
    {
        this._logger = logger;
        this._pricingTasksChannel = pricingTasksChannel;
        this._serviceProvider = serviceProvider;
        this._hostApplicationLifetime = hostApplicationLifetime;        
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        try
        {
            await foreach (var request in _pricingTasksChannel.ReadAllAsync(stoppingToken))
            {
                _logger.LogInformation($"Read message {request.Id} to process from channel. Request={request}");

                using var scope = _serviceProvider.CreateScope();
                var processor = scope.ServiceProvider.GetRequiredService<IPricingTasksProcessor>();

                await processor.Process(request, stoppingToken);

                _logger.LogInformation("finished processing message {Id} from channel.", request);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.Log(LogLevel.Information, "A task/operation cancelled exception was caught.");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "An unhandled exception was thrown.  Triggering app shutdown.");
        }
        finally
        {
            _hostApplicationLifetime.StopApplication();
        }
    }
}
