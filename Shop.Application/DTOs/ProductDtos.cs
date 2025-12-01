namespace Shop.Application.DTOs
{
    //data struct for the user requesting 
    public record ProductDto(
        int Id,
        string Name,
        string ImgUrl,
        decimal Price,
        string? Description,
        int StockQuantity);
                
    //requirement of creating the product from the task
    public record CreateProductDto(
        string Name, // Required
        string ImgUrl, // Required
        decimal? Price = null, // Optional(Nullable)
        string? Description = null // Optional(Nullable)
        );
    public record UpdateStockDto(
        int NewQuantity);
}
