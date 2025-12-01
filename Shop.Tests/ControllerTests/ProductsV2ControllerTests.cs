using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Mvc;
using Moq;
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
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            // Freeze mocks
            _queueMock = _fixture.Freeze<Mock<IStockQueue>>();
            _serviceMock = _fixture.Freeze<Mock<IProductService>>();

            _controller = _fixture.Create<ProductsV2Controller>();
        }

        [Fact]
        public async Task UpdateStockAsync_ShouldReturnAccepted_AndQueueMessage()
        {
            var id = _fixture.Create<int>();
            var dto = _fixture.Create<UpdateStockDto>();


            var result = await _controller.UpdateStockAsync(id, dto);

            Assert.IsType<AcceptedResult>(result);

            _queueMock.Verify(x => x.QueueBackgroundWorkItemAsync(
                It.Is<StockUpdateMessage>(m => m.ProductId == id && m.NewQuantity == dto.NewQuantity)
            ), Times.Once);

            // Verify Service was NOT called (ASYNC)
            _serviceMock.Verify(x => x.UpdateStockAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }
    }
}