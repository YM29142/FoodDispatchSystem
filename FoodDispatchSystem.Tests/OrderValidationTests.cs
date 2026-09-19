using FoodDispatchSystem.Web.ViewModels;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace FoodDispatchSystem.Tests
{
    public class OrderValidationTests
    {
        [Fact]
        public void Order_WithNoItems_ShouldBeInvalid()
        {
            // Arrange
            var model = new OrderCreateViewModel
            {
                Items = new List<OrderItemInputModel>()
            };

            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(
                model,
                context,
                results,
                validateAllProperties: true);

            // Assert
            Assert.False(isValid);

            Assert.Contains(
                results,
                result => result.MemberNames.Contains(
                    nameof(OrderCreateViewModel.Items)));
        }
        [Fact]
        public void Order_WithValidItem_ShouldBeValid()
        {
            // Arrange
            var model = new OrderCreateViewModel
            {
                Items = new List<OrderItemInputModel>
        {
            new OrderItemInputModel
            {
                ProductId = 1,
                Quantity = 2
            }
        }
            };

            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(
                model,
                context,
                results,
                validateAllProperties: true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(results);
        }
    }
}