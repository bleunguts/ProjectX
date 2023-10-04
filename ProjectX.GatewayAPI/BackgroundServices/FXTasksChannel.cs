using ProjectX.Core.Requests;
using System.Threading.Channels;

namespace ProjectX.GatewayAPI.BackgroundServices
{
    public class FXTasksChannel 
    {
        private const int MaxMessagesInChannel = 200;
        private ILogger<PricingTasksChannel> _logger;
        private Channel<SpotPriceRequest> _channel;
        public FXTasksChannel(ILogger<PricingTasksChannel> logger)
        {
            _logger = logger;
            _channel = Channel.CreateBounded<SpotPriceRequest>(new BoundedChannelOptions(MaxMessagesInChannel)
            {
                SingleWriter = true,
                SingleReader = true
            });
        }

        public IAsyncEnumerable<SpotPriceRequest> ReadAllAsync(CancellationToken ct = default) => _channel.Reader.ReadAllAsync(ct);
        public ChannelReader<SpotPriceRequest> Reader => _channel.Reader;
        public async Task<bool> SendRequestAsync(SpotPriceRequest request)
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
}
