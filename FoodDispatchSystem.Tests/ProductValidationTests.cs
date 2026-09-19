using FoodDispatchSystem.Web.Models;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace FoodDispatchSystem.Tests
{
    public class ProductValidationTests
    {
        [Fact]
        public void Product_WithZeroPrice_ShouldBeInvalid()
        {
            // Arrange
            var product = new Product
            {
                Name = "Producto de prueba",
                Price = 0,
                CategoryId = 1
            };

            var context = new ValidationContext(product);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(
                product,
                context,
                results,
                validateAllProperties: true);

            // Assert
            Assert.False(isValid);

            Assert.Contains(
                results,
                result => result.MemberNames.Contains(
                    nameof(Product.Price)));
        }
        [Fact]
        public void Product_WithValidPrice_ShouldBeValid()
        {
            // Arrange
            var product = new Product
            {
                Name = "Hamburguesa de prueba",
                Price = 4.75m,
                CategoryId = 1
            };

            var context = new ValidationContext(product);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(
                product,
                context,
                results,
                validateAllProperties: true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(results);
        }
        [Fact]
        public void Product_WithoutName_ShouldBeInvalid()
        {
            // Arrange
            var product = new Product
            {
                Name = string.Empty,
                Price = 4.75m,
                CategoryId = 1
            };

            var context = new ValidationContext(product);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(
                product,
                context,
                results,
                validateAllProperties: true);

            // Assert
            Assert.False(isValid);

            Assert.Contains(
                results,
                result => result.MemberNames.Contains(
                    nameof(Product.Name)));
        }
        [Fact]
        public void Product_WithPriceAboveMaximum_ShouldBeInvalid()
        {
            // Arrange
            var product = new Product
            {
                Name = "Producto de prueba",
                Price = 100000m,
                CategoryId = 1
            };

            var context = new ValidationContext(product);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(
                product,
                context,
                results,
                validateAllProperties: true);

            // Assert
            Assert.False(isValid);

            Assert.Contains(
                results,
                result => result.MemberNames.Contains(
                    nameof(Product.Price)));
        }
        [Fact]
        public void Product_WithMaximumPrice_ShouldBeValid()
        {
            // Arrange
            var product = new Product
            {
                Name = "Producto de prueba",
                Price = 99999.99m,
                CategoryId = 1
            };

            var context = new ValidationContext(product);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(
                product,
                context,
                results,
                validateAllProperties: true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(results);
        }
    }
}