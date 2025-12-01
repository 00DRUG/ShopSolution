using AutoFixture;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Shop.Api.Controllers;
using Shop.Application.DTOs;
using Shop.Application.Services;
using Shop.Infrastructure.BackgroundJobs;
using Xunit;

namespace Shop.Tests.ControllerTests
{
    public class ProductsV2ControllerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IStockQueue> _queueMock;
        private readonly Mock<IProductService> _serviceMock;
        private readonly ProductsV2Controller _controller;

        public ProductsV2ControllerTests()
        {
            _queueMock = new Mock<IStockQueue>();
            _serviceMock = new Mock<IProductService>();
            _controller = new ProductsV2Controller(_serviceMock.Object, _queueMock.Object);
        }

        [Fact]
        public async Task UpdateStockAsync_ShouldReturnAccepted_AndQueueMessage()
        {
            // Arrange
            var id = 42;
            var dto = new UpdateStockDto(99);

            // Act
            var result = await _controller.UpdateStockAsync(id, dto);

            // Assert
            Assert.IsType<AcceptedResult>(result);
            _queueMock.Verify(x => x.QueueBackgroundWorkItemAsync(
                It.Is<StockUpdateMessage>(m => m.ProductId == id && m.NewQuantity == dto.NewQuantity)
            ), Times.Once);
            _serviceMock.Verify(x => x.UpdateStockAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);

        }
    }
}