using ProjectX.Core.Requests;
using System.Threading.Channels;

namespace ProjectX.GatewayAPI.BackgroundServices;

public class PricingTasksChannel
{
    private const int MaxMessagesInChannel = 200;
    private readonly ILogger<PricingTasksChannel> _logger;
    private Channel<MultipleTimeslicesOptionsPricingRequest> _channel;    
    public PricingTasksChannel(ILogger<PricingTasksChannel> logger)
    {
        _logger = logger;
        _channel = Channel.CreateBounded<MultipleTimeslicesOptionsPricingRequest>(new BoundedChannelOptions(MaxMessagesInChannel)
        {
            SingleWriter = true,
            SingleReader = true
        });
    }
    public IAsyncEnumerable<MultipleTimeslicesOptionsPricingRequest> ReadAllAsync(CancellationToken ct = default) => _channel.Reader.ReadAllAsync(ct);
    public ChannelReader<MultipleTimeslicesOptionsPricingRequest> Reader => _channel.Reader;

    public async Task<bool> SendRequestAsync(MultipleTimeslicesOptionsPricingRequest request)
    {
        while (await _channel.Writer.WaitToWriteAsync())
        {
            if (_channel.Writer.TryWrite(request))
            {
                _logger.LogInformation($"Pricing Task Request written to channel successfully. Request:{request}");
                return true;
            }
        }

        return false;
    } 

    public void CompleteWriter(Exception? ex = null) => _channel.Writer.Complete(ex);
}