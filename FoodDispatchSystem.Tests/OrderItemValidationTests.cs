using FoodDispatchSystem.Web.ViewModels;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace FoodDispatchSystem.Tests
{
    public class OrderItemValidationTests
    {
        [Fact]
        public void OrderItem_WithZeroQuantity_ShouldBeInvalid()
        {
            // Arrange
            var item = new OrderItemInputModel
            {
                ProductId = 1,
                Quantity = 0
            };

            var context = new ValidationContext(item);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(
                item,
                context,
                results,
                validateAllProperties: true);

            // Assert
            Assert.False(isValid);

            Assert.Contains(
                results,
                result => result.MemberNames.Contains(
                    nameof(OrderItemInputModel.Quantity)));
        }
        [Fact]
        public void OrderItem_WithQuantityAboveMaximum_ShouldBeInvalid()
        {
            // Arrange
            var item = new OrderItemInputModel
            {
                ProductId = 1,
                Quantity = 101
            };

            var context = new ValidationContext(item);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(
                item,
                context,
                results,
                validateAllProperties: true);

            // Assert
            Assert.False(isValid);

            Assert.Contains(
                results,
                result => result.MemberNames.Contains(
                    nameof(OrderItemInputModel.Quantity)));
        }
        [Fact]
        public void OrderItem_WithMaximumQuantity_ShouldBeValid()
        {
            // Arrange
            var item = new OrderItemInputModel
            {
                ProductId = 1,
                Quantity = 100
            };

            var context = new ValidationContext(item);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(
                item,
                context,
                results,
                validateAllProperties: true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(results);
        }
        [Fact]
        public void OrderItem_WithMinimumQuantity_ShouldBeValid()
        {
            // Arrange
            var item = new OrderItemInputModel
            {
                ProductId = 1,
                Quantity = 1
            };

            var context = new ValidationContext(item);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(
                item,
                context,
                results,
                validateAllProperties: true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(results);
        }
        [Fact]
        public void OrderItem_WithoutProduct_ShouldBeInvalid()
        {
            // Arrange
            var item = new OrderItemInputModel
            {
                ProductId = null,
                Quantity = 1
            };

            var context = new ValidationContext(item);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(
                item,
                context,
                results,
                validateAllProperties: true);

            // Assert
            Assert.False(isValid);

            Assert.Contains(
                results,
                result => result.MemberNames.Contains(
                    nameof(OrderItemInputModel.ProductId)));
        }
        [Fact]
        public void OrderItem_WithInvalidProductId_ShouldBeInvalid()
        {
            // Arrange
            var item = new OrderItemInputModel
            {
                ProductId = 0,
                Quantity = 1
            };

            var context = new ValidationContext(item);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(
                item,
                context,
                results,
                validateAllProperties: true);

            // Assert
            Assert.False(isValid);

            Assert.Contains(
                results,
                result => result.MemberNames.Contains(
                    nameof(OrderItemInputModel.ProductId)));
        }
        [Fact]
        public void OrderItem_WithValidData_ShouldBeValid()
        {
            // Arrange
            var item = new OrderItemInputModel
            {
                ProductId = 1,
                Quantity = 5
            };

            var context = new ValidationContext(item);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(
                item,
                context,
                results,
                validateAllProperties: true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(results);
        }


    }


}