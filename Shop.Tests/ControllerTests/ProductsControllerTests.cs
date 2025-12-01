using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shop.Api.Controllers;
using Shop.Application.DTOs;
using Shop.Application.Services;

public class ProductsControllerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IProductService> _serviceMock = new();
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _controller = new ProductsController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithProducts()
    {
        // Arrange
        var products = _fixture.CreateMany<ProductDto>(3);
        _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(products);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(products, okResult.Value);
    }

    [Fact]
    public async Task GetById_ProductExists_ReturnsOk()
    {
        // Arrange
        var product = _fixture.Create<ProductDto>();
        _serviceMock.Setup(s => s.GetByIdAsync(product.Id)).ReturnsAsync(product);

        // Act
        var result = await _controller.GetById(product.Id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(product, okResult.Value);
    }

    [Fact]
    public async Task GetById_ProductNotFound_ReturnsNotFound()
    {
        // Arrange
        int id = _fixture.Create<int>();
        _serviceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync((ProductDto)null);

        // Act
        var result = await _controller.GetById(id);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Contains("not found", notFoundResult.Value.ToString());
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction()
    {
        // Arrange
        var dto = _fixture.Create<CreateProductDto>();
        int newId = _fixture.Create<int>();
        _serviceMock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(newId);

        // Act
        var result = await _controller.Create(dto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(_controller.GetById), createdResult.ActionName);
        Assert.Equal(newId, createdResult.RouteValues["id"]);
    }

    [Fact]
    public async Task UpdateStock_Success_ReturnsNoContent()
    {
        // Arrange
        int id = _fixture.Create<int>();
        var dto = _fixture.Create<UpdateStockDto>();
        _serviceMock.Setup(s => s.UpdateStockAsync(id, dto.NewQuantity)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateStock(id, dto);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateStock_KeyNotFound_ReturnsNotFound()
    {
        // Arrange
        int id = _fixture.Create<int>();
        var dto = _fixture.Create<UpdateStockDto>();
        _serviceMock.Setup(s => s.UpdateStockAsync(id, dto.NewQuantity))
            .ThrowsAsync(new KeyNotFoundException("not found"));

        // Act
        var result = await _controller.UpdateStock(id, dto);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Contains("not found", notFoundResult.Value.ToString());
    }

    [Fact]
    public async Task UpdateStock_ArgumentException_ReturnsBadRequest()
    {
        // Arrange
        int id = _fixture.Create<int>();
        var dto = _fixture.Create<UpdateStockDto>();
        _serviceMock.Setup(s => s.UpdateStockAsync(id, dto.NewQuantity))
            .ThrowsAsync(new ArgumentException("invalid"));

        // Act
        var result = await _controller.UpdateStock(id, dto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("invalid", badRequestResult.Value.ToString());
    }
}