using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using OrderManagementSystem.Application.Features.Orders.Queries;
using OrderManagementSystem.Domain.Entities;
using OrderManagementSystem.Domain.Enums;
using OrderManagementSystem.Infrastructure.Data;

namespace OrderManagementSystem.Tests.UnitTests
{
	public class GetOrderAnalyticsHandlerTests
	{
		private AppDbContext GetDbContextWithData()
		{
			var options = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;

			var context = new AppDbContext(options);

			var customer = new Customer
			{
				Id = Guid.NewGuid(),
				FirstName = "Test",
				LastName = "User",
				Email = "testUser@example.com",
				Segment = CustomerSegment.New,
				Orders = []
			};

			var orders = new List<Order>
			{
				new() { Id = Guid.NewGuid(), Customer = customer, TotalAmount = 100, CreatedAt = DateTime.UtcNow.AddDays(-5), Status = OrderStatus.Created },
				new() { Id = Guid.NewGuid(), Customer = customer, TotalAmount = 250, CreatedAt = DateTime.UtcNow.AddDays(-20), Status = OrderStatus.Delivered, FulfilledAt = DateTime.UtcNow.AddDays(-15) },
				new() { Id = Guid.NewGuid(), Customer = customer, TotalAmount = 500, CreatedAt = DateTime.UtcNow.AddDays(-10), Status = OrderStatus.Delivered, FulfilledAt = DateTime.UtcNow.AddDays(-7) },
				new() { Id = Guid.NewGuid(), Customer = customer, TotalAmount = 700, CreatedAt = DateTime.UtcNow.AddMonths(-5), Status = OrderStatus.Processing },
				new() { Id = Guid.NewGuid(), Customer = customer, TotalAmount = 120, CreatedAt = DateTime.UtcNow.AddDays(-1), Status = OrderStatus.Created }
			};

			context.Customers.Add(customer);
			context.Orders.AddRange(orders);
			context.SaveChanges();

			return context;
		}

		[Fact]
		public async Task Should_Correctly_Calculate_Order_Analytics()
		{
			// Arrange
			await using var context = GetDbContextWithData();
			var handler = new GetOrderAnalytics.Handler(context);

			// Act
			var result = await handler.Handle(new GetOrderAnalytics.Query(), default);

			// Assert
			result.Summary.TotalOrders.Should().Be(5);
			result.Summary.AverageOrderValue.Should().Be(334);
			result.Summary.FulfillmentRate.Should().BeApproximately(40.0, 0.5);
			result.Summary.AverageFulfillmentTimeInHours.Should().BeGreaterThan(0);
			result.StatusBreakdown.Should().Contain(x => x.Status == "Created" && x.Count == 2);
			result.ValueRangeHistogram.Should().Contain(x => x.Range == "101–300" && x.Count == 2);
			result.MonthlyOrderCounts.Should().NotBeEmpty();
			result.RecentOrders.Count.Should().BeLessThanOrEqualTo(5);
		}
	}
}
