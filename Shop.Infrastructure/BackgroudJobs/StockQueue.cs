using System.Threading.Channels;
using System.Threading.Tasks;
using System.Threading;
using Shop.Application.DTOs;

namespace Shop.Infrastructure.BackgroudJobs
{
    public interface IStockQueue
    {
        ValueTask QueueBackgroundWorkItemAsync(StockUpdateMessage workItem);
        ValueTask<StockUpdateMessage> DequeueAsync(CancellationToken cancellationToken);
    }
    public class StockQueue : IStockQueue
    {
        private readonly Channel<StockUpdateMessage> _channel;
        public StockQueue()
        {
            var options = new BoundedChannelOptions(100)
            {
                FullMode = BoundedChannelFullMode.Wait
            };
            _channel = Channel.CreateBounded<StockUpdateMessage>(options);
        }
        public async ValueTask QueueBackgroundWorkItemAsync(StockUpdateMessage workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }
            await _channel.Writer.WriteAsync(workItem);
        }
        public async ValueTask<StockUpdateMessage> DequeueAsync(CancellationToken cancellationToken)
        {
            return await _channel.Reader.ReadAsync(cancellationToken);
        }
    }
}
