using System.Collections.Generic;
using System.Linq;
using Greggs.Products.Api.Controllers;
using Greggs.Products.Api.Interfaces;
using Greggs.Products.Api.Models;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Greggs.Products.Api.DataAccess;

namespace Greggs.Products.UnitTests
{
    public class ProductControllerTests
    {
        private readonly Mock<ILogger<ProductController>> _mockLogger;
        private readonly Mock<IDataAccess<Product>> _mockProductAccess;
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly ProductController _controller;

        public ProductControllerTests()
        {
            _mockLogger = new Mock<ILogger<ProductController>>();
            _mockProductAccess = new Mock<IDataAccess<Product>>();
            _mockProductRepository = new Mock<IProductRepository>();

            // Initializing controller with mocked dependencies
            _controller = new ProductController(
                _mockLogger.Object,
                _mockProductAccess.Object,
                _mockProductRepository.Object
            );
        }

        [Fact]
        public void GetProducts_ReturnsOkResult_WhenProductsAreFound()
        {
            // Arrange
            var mockProducts = new List<Product>
            {
                new Product { Name = "Sausage Roll", PriceInPounds = 1m },
                new Product { Name = "Vegan Sausage Roll", PriceInPounds = 1.1m }
            };

            _mockProductAccess.Setup(p => p.List(It.IsAny<int?>(), It.IsAny<int?>()))
                .Returns(mockProducts);

            // Act
            var result = _controller.GetProducts(0, 2);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var products = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
            Assert.Equal(mockProducts.Count, products.Count());
        }

        [Fact]
        public void GetProducts_ReturnsNotFound_WhenNoProductsAreFound()
        {
            // Arrange
            _mockProductAccess.Setup(p => p.List(It.IsAny<int?>(), It.IsAny<int?>()))
                .Returns(new List<Product>());

            // Act
            var result = _controller.GetProducts(0, 2);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No products available.", notFoundResult.Value);
        }

        [Fact]
        public void GetProductsInEuros_ReturnsOkResult_WhenProductsAreFound()
        {
            // Arrange
            var mockProducts = new List<Product>
            {
                new Product { Name = "Sausage Roll", PriceInPounds = 1m },
                new Product { Name = "Vegan Sausage Roll", PriceInPounds = 1.1m }
            };

            // Setup synchronous List and ConvertToEuros
            _mockProductAccess.Setup(p => p.List(It.IsAny<int?>(), It.IsAny<int?>()))
                .Returns(mockProducts);
            _mockProductRepository.Setup(r => r.ConvertToEuros(It.IsAny<decimal>(), It.IsAny<decimal>()))
                .Returns((decimal pounds, decimal exchangeRate) => pounds * exchangeRate);

            // Act
            var result = _controller.GetProductsInEuros(0, 2);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var products = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
            var firstProduct = products.First();
            Assert.Equal(1m * 1.11m, firstProduct.PriceInEuros);  // Check price in Euros
        }

        [Fact]
        public void GetProductsInEuros_ReturnsNotFound_WhenNoProductsAreFound()
        {
            // Arrange
            _mockProductAccess.Setup(p => p.List(It.IsAny<int?>(), It.IsAny<int?>()))
                .Returns(new List<Product>());

            // Act
            var result = _controller.GetProductsInEuros(0, 2);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No products available.", notFoundResult.Value);
        }
    }
}
