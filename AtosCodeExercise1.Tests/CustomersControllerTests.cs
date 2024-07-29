using AtosCodeExercise1.Controllers;
using AtosCodeExercise1.Data;
using AtosCodeExercise1.Interfaces;
using AtosCodeExercise1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;


namespace AtosCodeExercise1.Tests
{
    public class CustomersControllerTests
    {
        private readonly Mock<ICustomerService> _mockCustomerService;
        private readonly Mock<ILogger<CustomersController>> _mockLogger;
        private readonly CustomersController _controller;
        private readonly DbContextOptions<CustomerDbContext> _dbContextOptions;

        public CustomersControllerTests()
        {
            var databaseName = $"TestDataBase_{Guid.NewGuid()}";
            _dbContextOptions = new DbContextOptionsBuilder<CustomerDbContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;

            var context = new CustomerDbContext(_dbContextOptions);
            _mockCustomerService = new Mock<ICustomerService>();
            _mockLogger = new Mock<ILogger<CustomersController>>();
            _controller = new CustomersController(context, _mockCustomerService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task PostCustomer_ReturnsConflict_WhenCustomerExists()
        {
            // Arrange
            var existingCustomer = new Customer { Id = 1, FirstName = "John", LastName = "Smith" };
            var newCustomer = new Customer { Id = 1, FirstName = "Alice", LastName = "Cooper" };

            using (var context = new CustomerDbContext(_dbContextOptions))
            {
                context.Customers.Add(existingCustomer);
                await context.SaveChangesAsync();
            }

            _mockCustomerService.Setup(s => s.CustomerExists(newCustomer.Id)).Returns(true);

            // Act
            var result = await _controller.PostCustomer(newCustomer);

            // Assert
            var actionResult = Assert.IsType<ConflictObjectResult>(result.Result);
        }

        [Fact]
        public async Task PostCustomer_CreatesCustomer_WhenCustomerDoesNotExist()
        {
            // Arrange
            var customer = new Customer { Id = 1, FirstName = "John", LastName = "Smith" };

            // Act
            var result = await _controller.PostCustomer(customer);

            // Assert
            var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal("PostCustomer", actionResult.ActionName);
            Assert.Equal(customer.Id, ((Customer)actionResult.Value).Id);

            using (var context = new CustomerDbContext(_dbContextOptions))
            {
                Assert.True(context.Customers.Any(c => c.Id == customer.Id));
            }

        }

        [Fact]
        public async Task PostCustomer_ReturnsCreatedAtAction_WhenIdIsMoreThanFourMillion()
        {
            // Arrange
            var customer = new Customer { Id = -1, FirstName = "John", LastName = "Smith" };
            
            // Act
            var result = await _controller.PostCustomer(customer);

            // Assert
            var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal("PostCustomer", actionResult.ActionName);
            Assert.Equal(customer.Id, ((Customer)actionResult.Value).Id);

            using (var context = new CustomerDbContext(_dbContextOptions))
            {
                Assert.True(context.Customers.Any(c => c.Id == customer.Id));
            }

        }

        // Unit Tests for GetCustomers() method

        [Fact]
        public async Task GetCustomers_ReturnsNoContent_WhenNoCustomersExist()
        {
            // Arrange
            var context = new CustomerDbContext(_dbContextOptions);

            // Act
            var result = await _controller.GetCustomers();

            // Assert
            Assert.IsType<NoContentResult>(result.Result);
        }

        [Fact]
        public async Task GetCustomers_ReturnsAllCustomers_WhenMillionsOfCustomersExist()
        {
            // Arrange
            var context = new CustomerDbContext(_dbContextOptions);
            List<Customer> customers = [];

            for (long i = 1; i <= 1000000; i++)
            {
                customers.Add(new Customer { Id = i, FirstName = $"FistName{i}", LastName = $"LastName{i}" });
            }

            context.AddRange(customers);
            await context.SaveChangesAsync();
            
            //Act
            ActionResult<IEnumerable<Customer>> result = await _controller.GetCustomers();

            //Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Customer>>>(result);
            var customersList = Assert.IsType<List<Customer>>(actionResult.Value);
            Assert.Equal(1000000, customersList.Count);
            Assert.Equal("FistName1000", customersList[999].FirstName);
        }

        [Fact]
        public async Task DeleteCustomer_ReturnsNotFound_WhenCustomerDoesNotExist()
        {
            // Arrange
            var context = new CustomerDbContext(_dbContextOptions);
            List<Customer> customers = [new Customer { Id = 999, FirstName = "XYZ", LastName = "QWE" }];
            long nonExistingId = 999;

            // Act
            var result = await _controller.DeleteCustomer(nonExistingId);

            // Assert
            var actionResult = Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteCustomer_RemovesCustomer_WhenCustomerExists()
        {
            // Arrange
            var customer = new Customer { Id = 1, FirstName = "TestCustomer",  LastName = "TestCustomerLastName"};
            var context = new CustomerDbContext(_dbContextOptions);
            context.Customers.Add(customer);
            await context.SaveChangesAsync();

            // Act
            var result = await _controller.DeleteCustomer(customer.Id);

            // Assert
            var actionResult = Assert.IsType<NoContentResult>(result);
            Assert.False(context.Customers.Any(c => c.Id == customer.Id));
        }
    }
}