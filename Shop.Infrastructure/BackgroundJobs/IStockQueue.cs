using Shop.Application.DTOs;

namespace Shop.Infrastructure.BackgroundJobs
{
    public interface IStockQueue
    {
        ValueTask QueueBackgroundWorkItemAsync(StockUpdateMessage workItem);

        ValueTask<StockUpdateMessage> DequeueAsync(CancellationToken cancellationToken);
    }
}
