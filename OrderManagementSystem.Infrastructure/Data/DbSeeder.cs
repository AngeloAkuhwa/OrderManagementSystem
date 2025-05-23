using OrderManagementSystem.Domain.Entities;
using OrderManagementSystem.Domain.Enums;

namespace OrderManagementSystem.Infrastructure.Data
{
	public static class DbSeeder
	{
		public static void Seed(AppDbContext context)
		{
			if (context.Customers.Any()) return;

			var customer1 = new Customer
			{
				Id = Guid.NewGuid(),
				FirstName = "John",
				LastName = "Doe",
				Email = "john.doe@example.com",
				Segment = CustomerSegment.New,
			};

			var customer2 = new Customer
			{
				Id = Guid.NewGuid(),
				FirstName = "Samantha",
				LastName = "Loyal",
				Email = "samantha.loyal@example.com",
				Segment = CustomerSegment.Loyal,
			};

			var customer3 = new Customer
			{
				Id = Guid.NewGuid(),
				FirstName = "Angelo",
				LastName = "VIP",
				Email = "angelo.vip@example.com",
				Segment = CustomerSegment.VIP,
			};


			customer1.Orders.Add(new Order
			{
				Id = Guid.NewGuid(),
				CreatedAt = DateTime.UtcNow.AddDays(-2),
				Status = OrderStatus.Created,
				TotalAmount = 120,
			});

			customer2.Orders.AddRange(new[]
			{
				new Order
				{
					Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow.AddDays(-20), TotalAmount = 100,
					Status = OrderStatus.Delivered, FulfilledAt = DateTime.UtcNow.AddDays(-15)
				},
				new Order
				{
					Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow.AddDays(-10), TotalAmount = 150,
					Status = OrderStatus.Delivered, FulfilledAt = DateTime.UtcNow.AddDays(-5)
				},
				new Order
				{
					Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow.AddDays(-5), TotalAmount = 200,
					Status = OrderStatus.Delivered, FulfilledAt = DateTime.UtcNow.AddDays(-2)
				},
				new Order
				{
					Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow.AddDays(-3), TotalAmount = 180,
					Status = OrderStatus.Processing
				},
				new Order { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, TotalAmount = 220, Status = OrderStatus.Created }
			});

			customer3.Orders.AddRange(Enumerable.Range(1, 10).Select(i => new Order
			{
				Id = Guid.NewGuid(),
				CreatedAt = DateTime.UtcNow.AddDays(-i * 3),
				TotalAmount = 600,
				Status = OrderStatus.Delivered,
				FulfilledAt = DateTime.UtcNow.AddDays(-(i * 3 - 1))
			}));

			context.Customers.AddRange(customer1, customer2, customer3);
			context.SaveChanges();
		}
	}
}
