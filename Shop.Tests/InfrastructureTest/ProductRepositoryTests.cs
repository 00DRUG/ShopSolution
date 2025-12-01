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

        var product = _fixture.Build<Product>()
            .With(p => p.Name, "TestProduct")
            .With(p => p.ImgUrl, "http://img.com/test.jpg")
            .With(p => p.Price, 99.99m)
            .With(p => p.Description, "Test Description")
            .With(p => p.StockQuantity, 10)
            .Without(p => p.Id)
            .Create();
        
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

        var options = new DbContextOptionsBuilder<ShopDbContext>()
            .UseSqlite(connection)
            .Options;

        using var context = new ShopDbContext(options);
        context.Database.EnsureCreated();

        var repo = new ProductRepository(context);

        var products = _fixture.Build<Product>()
            .With(p => p.ImgUrl, "http://img.com/test.jpg")
            .With(p => p.Price, 10.0m)
            .With(p => p.Description, "desc")
            .With(p => p.StockQuantity, 5)
            .Without(p => p.Id)
            .CreateMany(3);

        // Act
        foreach (var product in products)
        {
            await repo.AddAsync(product);
        }
        await repo.SaveChangesAsync();

        var allProducts = await repo.GetAllAsync();

        // Assert
        Assert.Equal(3, allProducts.Count());

        connection.Close();
    }
}