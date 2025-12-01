namespace Shop.Domain;

public class Product
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string ImgUrl { get; private set; }
    public decimal Price { get; private set; }
    public string? Description { get; private set; }

    public int StockQuantity { get; private set; }
    // Constructor that enforces name and url only
    public Product (string name, string imgUrl)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(imgUrl))
            throw new ArgumentException("Product image URL cannot be empty.", nameof(imgUrl));

        Name = name;
        ImgUrl = imgUrl;

        //Defaults
        Price = 0;
        StockQuantity = 0;
    }
    public void UpdateDetails(decimal price, string? description)
    {
        if (price < 0)
            throw new ArgumentException("Price cannot be negative.", nameof(price));
        Price = price;
        Description = description;
    }
    public void UpdateStock(int newQuantity)
    {
        if(newQuantity<0)
            throw new ArgumentException("Quantity cannot be negative.",nameof(newQuantity));
        StockQuantity = newQuantity;
    }
}
