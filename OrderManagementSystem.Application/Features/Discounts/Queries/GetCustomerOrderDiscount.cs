using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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

		public class Handler(AppDbContext context, IDiscountService discountService, ILogger<Handler> logger) : IRequestHandler<Query, OrderDiscountResult>
		{
			public async Task<OrderDiscountResult> Handle(Query request, CancellationToken cancellationToken)
			{
				logger.LogInformation("Applying discount for OrderId: {OrderId}", request.OrderId);

				var order = await context.Orders
					.Include(o => o.Customer)
					.FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

				if (order?.Customer is null)
				{
					logger.LogWarning("Order or Customer not found for OrderId: {OrderId}", request.OrderId);
					return new OrderDiscountResult();
				}

				var discounted = discountService.ApplyDiscount(order.Customer, order);

				logger.LogInformation("Discount applied. Original: {Original}, Discounted: {Discounted}", order.TotalAmount, discounted);

				return new OrderDiscountResult
				{
					OriginalAmount = order.TotalAmount,
					DiscountedAmount = discounted
				};
			}
		}
	}
}