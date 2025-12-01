using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure.Data;

public class ShopDbContextTests
{
    [Fact]
    public async Task SeedsData_Correctly()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<ShopDbContext>()
            .UseSqlite(connection)
            .Options;

        using (var context = new ShopDbContext(options))
        {
            context.Database.EnsureCreated();

            var products = await context.Products.ToListAsync();
            Assert.True(products.Count >= 3);
            Assert.Contains(products, p => p.Name == "Gaming Mouse");
            Assert.Contains(products, p => p.Name == "Mechanical Keyboard");
            Assert.Contains(products, p => p.Name == "Monitor 24");
        }

        connection.Close();
    }
}