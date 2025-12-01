using AutoFixture;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Shop.Domain;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Repositories;
using Xunit;

public class ProductRepositoryTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public async Task AddAndGet_Product_WorksCorrectly()
    {
        // Arrange
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<ShopDbContext>()
            .UseSqlite(connection)
            .Options;

        using var context = new ShopDbContext(options);
        context.Database.EnsureCreated();

        var repo = new ProductRepository(context);

        var product = new Product("TestProduct", "http://img.com/test.jpg");
        product.UpdateDetails(99.99m, "Test Description");
        product.UpdateStock(10);

        // Act
        await repo.AddAsync(product);
        await repo.SaveChangesAsync();

        var fetched = await repo.GetByIdAsync(product.Id);

        // Assert
        Assert.NotNull(fetched);
        Assert.Equal("TestProduct", fetched.Name);
        Assert.Equal(99.99m, fetched.Price);

        connection.Close();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllProducts()
    {
        // Arrange
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        //db context options creates 3 initial products
        var options = new DbContextOptionsBuilder<ShopDbContext>()
            .UseSqlite(connection)
            .Options;

        using var context = new ShopDbContext(options);
        context.Database.EnsureCreated();

        var repo = new ProductRepository(context);

        var products = new[]
        {
            new Product("A", "imgA"),
            new Product("B", "imgB"),
            new Product("C", "imgC")
        };

        products[0].UpdateDetails(1, "descA");
        products[0].UpdateStock(1);

        products[1].UpdateDetails(2, "descB");
        products[1].UpdateStock(2);

        products[2].UpdateDetails(3, "descC");
        products[2].UpdateStock(3);

        // Act
        foreach (var product in products)
        {
            await repo.AddAsync(product);
        }
        await repo.SaveChangesAsync();

        var allProducts = await repo.GetAllAsync();

        // Assert 
        Assert.Equal(6, allProducts.Count());

        connection.Close();
    }
}