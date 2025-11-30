namespace Shop.Application.DTOs
{
    public record StockUpdateMessage(int ProductId, int NewQuantity);
}
