using OrderManagementSystem.Application.Interfaces;
using OrderManagementSystem.Domain.Entities;
using OrderManagementSystem.Domain.Enums;
using OrderManagementSystem.Infrastructure.Data;

namespace OrderManagementSystem.Application.Services.Strategies
{
	public class LoyalCustomerDiscount(AppDbContext context) : IDiscountStrategy
	{
		public decimal ApplyDiscount(Customer customer, Order order)
		{
			var config = context.CustomerSegmentDiscounts
				.FirstOrDefault(d => d.Segment == CustomerSegment.Loyal);

			if (config == null)
			{
				return order.TotalAmount;
			}

			var eligible = customer.Orders.Count >= (config.MinOrders ?? 0);
			return eligible ? order.TotalAmount * (1 - config.DiscountRate) : order.TotalAmount;
		}
	}
}