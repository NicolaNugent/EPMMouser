using EPM.Mouser.Interview.Data;
using EPM.Mouser.Interview.Models;
using EPM.Mouser.Interview.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace EPM.Mouser.Interview.Tests.ControllerTests
{
    [TestClass]
    public class WarehouseAPITests
    {
        private readonly Mock<IWarehouseRepository> _mockWarehouseRepo;

        public WarehouseAPITests()
        {           
            this._mockWarehouseRepo = new Mock<IWarehouseRepository>();
        }

        [TestMethod]
        public async Task GetProduct()
        {
            // Arrange
            var sut = this.CreateController();
            Product product = new Product
            {
                Id = 1,
                Name = "Test",
            };
            this._mockWarehouseRepo.Setup(c => c.Get(1)).ReturnsAsync(product);

            // Act
            var result = await sut.GetProduct(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(Product));
        }

        [TestMethod]
        public async Task GetPublicInStockProducts()
        {
            // Arrange
            var sut = this.CreateController();
            List<Product> products = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "Test",
                }
            };
            this._mockWarehouseRepo.Setup(c => c.List()).ReturnsAsync(products);

            // Act
            var result = await sut.GetPublicInStockProducts();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<Product>));
        }

        [TestMethod]
        public async Task OrderItem_ReturnSuccess()
        {
            // Arrange
            var sut = this.CreateController();
            Product product = new Product
            {
                Id = 1,
                Name = "Test",
                InStockQuantity = 5,
            };
            UpdateQuantityRequest updateQuantityRequest = new UpdateQuantityRequest
            {
                Id = 1,
                Quantity = 1,
            };
            this._mockWarehouseRepo.Setup(c => c.Get(1)).ReturnsAsync(product);

            // Act
            var result = await sut.OrderItem(updateQuantityRequest);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(UpdateResponse));
            Assert.AreEqual(result.Success, true);
        }

        [TestMethod]
        public async Task OrderItem_ReturnError_InvalidRequest()
        {
            // Arrange
            var sut = this.CreateController();
            Product? product = null;
            UpdateQuantityRequest updateQuantityRequest = new UpdateQuantityRequest
            {
                Id = 1,
                Quantity = -1,
            };
            this._mockWarehouseRepo.Setup(c => c.Get(1)).ReturnsAsync(product);

            // Act
            var result = await sut.OrderItem(updateQuantityRequest);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(UpdateResponse));
            Assert.AreEqual(result.Success, false);
            Assert.AreEqual(result.ErrorReason, ErrorReason.InvalidRequest);
        }

        [TestMethod]
        public async Task OrderItem_ReturnError_QuantityInvalid()
        {
            // Arrange
            var sut = this.CreateController();
            Product product = new Product
            {
                Id = 1,
                Name = "Test",
                InStockQuantity = 5,
            };
            UpdateQuantityRequest updateQuantityRequest = new UpdateQuantityRequest
            {
                Id = 1,
                Quantity = -1,
            };
            this._mockWarehouseRepo.Setup(c => c.Get(1)).ReturnsAsync(product);

            // Act
            var result = await sut.OrderItem(updateQuantityRequest);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(UpdateResponse));
            Assert.AreEqual(result.Success, false);
            Assert.AreEqual(result.ErrorReason, ErrorReason.QuantityInvalid);
        }

        [TestMethod]
        public async Task OrderItem_ReturnError_NotEnoughQuantity()
        {
            // Arrange
            var sut = this.CreateController();
            Product product = new Product
            {
                Id = 1,
                Name = "Test",
            };
            UpdateQuantityRequest updateQuantityRequest = new UpdateQuantityRequest
            {
                Id = 1,
                Quantity = 1,
            };
            this._mockWarehouseRepo.Setup(c => c.Get(1)).ReturnsAsync(product);

            // Act
            var result = await sut.OrderItem(updateQuantityRequest);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(UpdateResponse));
            Assert.AreEqual(result.Success, false);
            Assert.AreEqual(result.ErrorReason, ErrorReason.NotEnoughQuantity);
        }

        private WarehouseApi CreateController()
        {
            return new WarehouseApi(
                _mockWarehouseRepo.Object)
            {
            };
        }
    }
}