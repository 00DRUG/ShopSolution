using Shop.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Infrastructure.BackgroundJobs
{
    public interface IStockQueue
    {
        ValueTask QueueBackgroundWorkItemAsync(StockUpdateMessage workItem);

        ValueTask<StockUpdateMessage> DequeueAsync(CancellationToken cancellationToken);
    }
}
