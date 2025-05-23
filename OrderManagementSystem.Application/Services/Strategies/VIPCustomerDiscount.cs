using OrderManagementSystem.Application.Interfaces;
using OrderManagementSystem.Domain.Entities;
using OrderManagementSystem.Domain.Enums;
using OrderManagementSystem.Infrastructure.Data;

namespace OrderManagementSystem.Application.Services.Strategies
{
	public class VIPCustomerDiscount(AppDbContext context) : IDiscountStrategy
	{
		public decimal ApplyDiscount(Customer customer, Order order)
		{
			var config = context.CustomerSegmentDiscounts
				.FirstOrDefault(d => d.Segment == CustomerSegment.VIP);

			if (config == null)
			{
				return order.TotalAmount;
			}

			var totalSpent = customer.Orders.Sum(o => o.TotalAmount);
			var eligible = totalSpent > (config.ThresholdAmount ?? 0);
			return eligible ? order.TotalAmount * (1 - config.DiscountRate) : order.TotalAmount;
		}
	}
}