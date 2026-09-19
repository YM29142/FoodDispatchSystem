using FoodDispatchSystem.Web.Services;
using FoodDispatchSystem.Web.ViewModels;
using Xunit;

namespace FoodDispatchSystem.Tests
{
    public class OrderItemServiceTests
    {
        [Fact]
        public void ConsolidateItems_WithDuplicateProducts_ShouldMergeQuantities()
        {
            // Arrange
            var service = new OrderItemService();

            var items = new List<OrderItemInputModel>
            {
                new OrderItemInputModel
                {
                    ProductId = 1,
                    Quantity = 1
                },
                new OrderItemInputModel
                {
                    ProductId = 1,
                    Quantity = 1
                }
            };

            // Act
            var result = service.ConsolidateItems(items);

            // Assert
            Assert.Single(result);

            Assert.Equal(
                1,
                result[0].ProductId);

            Assert.Equal(
                2,
                result[0].Quantity);
        }
        [Fact]
        public void ConsolidateItems_WithDifferentProducts_ShouldKeepSeparateItems()
        {
            // Arrange
            var service = new OrderItemService();

            var items = new List<OrderItemInputModel>
    {
        new OrderItemInputModel
        {
            ProductId = 1,
            Quantity = 2
        },
        new OrderItemInputModel
        {
            ProductId = 3,
            Quantity = 3
        }
    };

            // Act
            var result = service.ConsolidateItems(items);

            // Assert
            Assert.Equal(2, result.Count);

            Assert.Contains(
                result,
                item =>
                    item.ProductId == 1 &&
                    item.Quantity == 2);

            Assert.Contains(
                result,
                item =>
                    item.ProductId == 3 &&
                    item.Quantity == 3);
        }
        [Fact]
        public void ConsolidateItems_WithMultipleDuplicateProducts_ShouldMergeEachProduct()
        {
            // Arrange
            var service = new OrderItemService();

            var items = new List<OrderItemInputModel>
    {
        new OrderItemInputModel
        {
            ProductId = 1,
            Quantity = 1
        },
        new OrderItemInputModel
        {
            ProductId = 1,
            Quantity = 2
        },
        new OrderItemInputModel
        {
            ProductId = 3,
            Quantity = 3
        },
        new OrderItemInputModel
        {
            ProductId = 3,
            Quantity = 4
        }
    };

            // Act
            var result = service.ConsolidateItems(items);

            // Assert
            Assert.Equal(2, result.Count);

            Assert.Contains(
                result,
                item =>
                    item.ProductId == 1 &&
                    item.Quantity == 3);

            Assert.Contains(
                result,
                item =>
                    item.ProductId == 3 &&
                    item.Quantity == 7);
        }
        [Fact]
        public void ExceedsMaximumQuantity_WithCombinedQuantityAbove100_ShouldReturnTrue()
        {
            // Arrange
            var service = new OrderItemService();

            var items = new List<OrderItemInputModel>
    {
        new OrderItemInputModel
        {
            ProductId = 1,
            Quantity = 60
        },
        new OrderItemInputModel
        {
            ProductId = 1,
            Quantity = 60
        }
    };

            // Act
            var result = service.ExceedsMaximumQuantity(items);

            // Assert
            Assert.True(result);
        }
        [Fact]
        public void ExceedsMaximumQuantity_WithCombinedQuantityEqualTo100_ShouldReturnFalse()
        {
            // Arrange
            var service = new OrderItemService();

            var items = new List<OrderItemInputModel>
    {
        new OrderItemInputModel
        {
            ProductId = 1,
            Quantity = 60
        },
        new OrderItemInputModel
        {
            ProductId = 1,
            Quantity = 40
        }
    };

            // Act
            var result = service.ExceedsMaximumQuantity(items);

            // Assert
            Assert.False(result);
        }
        [Fact]
        public void ExceedsMaximumQuantity_WithDifferentProducts_ShouldReturnFalse()
        {
            // Arrange
            var service = new OrderItemService();

            var items = new List<OrderItemInputModel>
    {
        new OrderItemInputModel
        {
            ProductId = 1,
            Quantity = 60
        },
        new OrderItemInputModel
        {
            ProductId = 3,
            Quantity = 60
        }
    };

            // Act
            var result = service.ExceedsMaximumQuantity(items);

            // Assert
            Assert.False(result);
        }
    }
}