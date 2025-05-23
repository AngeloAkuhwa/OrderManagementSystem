using OrderManagementSystem.Application.Interfaces;
using OrderManagementSystem.Domain.Entities;
using OrderManagementSystem.Domain.Enums;
using OrderManagementSystem.Infrastructure.Data;

namespace OrderManagementSystem.Application.Services.Strategies
{
	public class NewCustomerDiscount(AppDbContext context) : IDiscountStrategy
	{
		public decimal ApplyDiscount(Customer customer, Order order)
		{
			var config = context.CustomerSegmentDiscounts
				.FirstOrDefault(d => d.Segment == CustomerSegment.New);

			if (config is null)
			{
				return order.TotalAmount;
			}

			return order.TotalAmount * (1 - config.DiscountRate);
		}
	}
}