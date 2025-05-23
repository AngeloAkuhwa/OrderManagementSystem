using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Application.Interfaces;
using OrderManagementSystem.Infrastructure.Data;

namespace OrderManagementSystem.Application.Features.Discounts.Queries
{
	public static class GetCustomerOrderDiscount
	{
		public class Query : IRequest<OrderDiscountResult>
		{
			public Guid OrderId { get; init; }
		}

		public class OrderDiscountResult
		{
			public decimal OriginalAmount { get; set; }
			public decimal DiscountedAmount { get; set; }
			public decimal Discount => OriginalAmount - DiscountedAmount;
		}

		public class Handler(AppDbContext context, IDiscountService discountService) : IRequestHandler<Query, OrderDiscountResult>
		{
			public async Task<OrderDiscountResult> Handle(Query request, CancellationToken cancellationToken)
			{
				var order = await context.Orders
						.Include(o => o.Customer)
						.FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

				if (order == null || order.Customer == null)
					throw new KeyNotFoundException("Order or customer not found.");

				var discounted = discountService.ApplyDiscount(order.Customer, order);

				return new OrderDiscountResult
				{
					OriginalAmount = order.TotalAmount,
					DiscountedAmount = discounted
				};
			}
		}
	}
}