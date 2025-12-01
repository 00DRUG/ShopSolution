using AutoFixture;
using Shop.Domain;
using Xunit;

namespace Shop.Tests.DomainTests
{
    public class ProductTests
    {
        private readonly Fixture _fixture;

        public ProductTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenNameIsEmpty()
        {
            var validUrl = _fixture.Create<string>();

            Assert.Throws<ArgumentException>(() => new Product("", validUrl));
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenImgUrlIsEmpty()
        {

            var validName = _fixture.Create<string>();

            Assert.Throws<ArgumentException>(() => new Product(validName, ""));
        }

        [Fact]
        public void UpdateStock_ShouldUpdateQuantity_WhenValueIsValid()
        {
            var product = _fixture.Create<Product>();
            var newStock = _fixture.Create<int>();

            var validStock = Math.Abs(newStock);

            product.UpdateStock(validStock);

            Assert.Equal(validStock, product.StockQuantity);
        }

        [Fact]
        public void UpdateStock_ShouldThrow_WhenValueIsNegative()
        {
            var product = _fixture.Create<Product>();

            Assert.Throws<ArgumentException>(() => product.UpdateStock(-1));
        }
    }
}