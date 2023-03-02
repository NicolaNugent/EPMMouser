using EPM.Mouser.Interview.Data;
using EPM.Mouser.Interview.Models;
using EPM.Mouser.Interview.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace EPM.Mouser.Interview.Tests.ControllerTests
{
    [TestClass]
    public class HomeControllerTests
    {
        private readonly Mock<IWarehouseRepository> _mockWarehouseRepo;

        public HomeControllerTests()
        {           
            this._mockWarehouseRepo = new Mock<IWarehouseRepository>();
        }

        [TestMethod]
        public async Task Index()
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
            var result = await sut.Index();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IActionResult));
            Assert.IsInstanceOfType((result as ViewResult)?.ViewData.Model, typeof(List<Product>));
            var model = (List<Product>?)(result as ViewResult)?.ViewData.Model;
            Assert.AreEqual(model?.Count(), 1);
            Assert.AreEqual(model?[0].Name, "Test");
        }

        [TestMethod]
        public async Task ProductPage()
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
            var result = await sut.ProductPage(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IActionResult));
            Assert.IsInstanceOfType((result as ViewResult)?.ViewData.Model, typeof(Product));
            var model = (Product?)(result as ViewResult)?.ViewData.Model;
            Assert.AreEqual(model?.Name, "Test");
        }

        private HomeController CreateController()
        {
            return new HomeController(
                _mockWarehouseRepo.Object)
            {
            };
        }
    }
}