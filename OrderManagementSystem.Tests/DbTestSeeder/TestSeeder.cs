using OrderManagementSystem.Domain.Entities;
using OrderManagementSystem.Domain.Enums;
using OrderManagementSystem.Infrastructure.Data;

namespace OrderManagementSystem.Tests.DbTestSeeder
{
	public static class TestSeeder
	{
		public static void Seed(AppDbContext context)
		{
			if (context.Customers.Any()) return;

			var customerNew = new Customer
			{
				Id = Guid.NewGuid(),
				FirstName = "Test",
				LastName = "New",
				Email = "new@test.com",
				Segment = CustomerSegment.New,
				Orders =
				[
					new Order
					{
						Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
						TotalAmount = 100,
						Status = OrderStatus.Created,
						CreatedAt = DateTime.UtcNow.AddDays(-1)
					}
				]
			};

			var customerLoyal = new Customer
			{
				Id = Guid.NewGuid(),
				FirstName = "Test",
				LastName = "Loyal",
				Email = "loyal@test.com",
				Segment = CustomerSegment.Loyal,
				Orders =
				[
					new Order
					{
						Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
						TotalAmount = 200,
						Status = OrderStatus.Processing,
						CreatedAt = DateTime.UtcNow.AddDays(-5),
						FulfilledAt = DateTime.UtcNow.AddDays(-2)
					}
				]
			};

			var customerVip = new Customer
			{
				Id = Guid.NewGuid(),
				FirstName = "Test",
				LastName = "VIP",
				Email = "vip@test.com",
				Segment = CustomerSegment.VIP,
				Orders =
				[
					new Order
					{
						Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
						TotalAmount = 1000,
						Status = OrderStatus.Delivered,
						CreatedAt = DateTime.UtcNow.AddDays(-10),
						FulfilledAt = DateTime.UtcNow.AddDays(-7)
					}
				]
			};

			context.Customers.AddRange(customerNew, customerLoyal, customerVip);
			context.SaveChanges();
		}
	}
}
