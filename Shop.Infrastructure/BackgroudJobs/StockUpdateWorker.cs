using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shop.Application.Services;

namespace Shop.Infrastructure.BackgroudJobs
{
    public class StockUpdateWorker : BackgroundService
    {
        private readonly IStockQueue _stockQueue;
        private readonly IServiceScopeFactory _serviceScopeFactory; // Factory temp to create scopes
        private readonly ILogger<StockUpdateWorker> _logger;
        public StockUpdateWorker(IStockQueue stockQueue,
            IServiceScopeFactory serviceScopeFactory,
            ILogger<StockUpdateWorker> logger)
        {
            _stockQueue = stockQueue;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Stock Update Worker running.");
            while (!stoppingToken.IsCancellationRequested)
            {
                var stockUpdateMessage = await _stockQueue.DequeueAsync(stoppingToken);
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
                    try
                    {
                        await productService.UpdateStockAsync(stockUpdateMessage.ProductId, stockUpdateMessage.NewQuantity);
                        _logger.LogInformation($"Updated stock for ProductId: {stockUpdateMessage.ProductId} to NewQuantity: {stockUpdateMessage.NewQuantity}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error updating stock for ProductId: {stockUpdateMessage.ProductId}");
                    }
                }
            }
        }
    }
}
