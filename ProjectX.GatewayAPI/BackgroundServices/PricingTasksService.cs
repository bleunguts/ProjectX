using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProjectX.GatewayAPI.Processors;
using System.Threading.Channels;

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
        try
        {
            await foreach (var message in _pricingTasksChannel.Reader.ReadAllAsync().WithCancellation(stoppingToken))
            {
                _logger.LogInformation("Read message {Id} to process from channel.", message);

                using var scope = _serviceProvider.CreateScope();
                var processor = scope.ServiceProvider.GetRequiredService<IPricingTasksProcessor>();

                await processor.Process(message);

                _logger.LogInformation("finished processing message {Id} from channel.", message);
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

public class PricingTasksChannel
{
    private readonly ILogger<PricingTasksChannel> _logger;
    private Channel<string> _channel;    

    public PricingTasksChannel(ILogger<PricingTasksChannel> logger)
    {
        _logger = logger;
        _channel = Channel.CreateBounded<string>(new BoundedChannelOptions(2000)
        {
            SingleWriter = true,
            SingleReader = true,
        });
    }

    public ChannelReader<string> Reader => _channel.Reader;
}