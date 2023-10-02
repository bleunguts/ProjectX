using ProjectX.Core.Requests;
using System.Threading.Channels;

namespace ProjectX.GatewayAPI.BackgroundServices;

public class PricingTasksChannel
{
    private readonly ILogger<PricingTasksChannel> _logger;
    private Channel<MultipleTimeslicesOptionsPricingRequest> _channel;    

    public PricingTasksChannel(ILogger<PricingTasksChannel> logger)
    {
        _logger = logger;
        _channel = Channel.CreateBounded<MultipleTimeslicesOptionsPricingRequest>(new BoundedChannelOptions(2000)
        {
            SingleWriter = true,
            SingleReader = true,
        });
    }

    public ChannelReader<MultipleTimeslicesOptionsPricingRequest> Reader => _channel.Reader;

    public async Task SendRequestAsync(MultipleTimeslicesOptionsPricingRequest request)
    {
        while (await _channel.Writer.WaitToWriteAsync())
        {
            if (_channel.Writer.TryWrite(request))
            {
                _logger.LogInformation($"Pricing Task Request written to channel successfully. Request:{request}");
            }
        }
    }
}