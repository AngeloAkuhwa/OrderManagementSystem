using OrderManagementSystem.Application.Interfaces;
using OrderManagementSystem.Application.Services.Strategies;
using OrderManagementSystem.Domain.Entities;
using OrderManagementSystem.Domain.Enums;
using OrderManagementSystem.Infrastructure.Data;

namespace OrderManagementSystem.Application.Services.DiscountServices
{
	public class DiscountService(AppDbContext context): IDiscountService
	{
		public decimal ApplyDiscount(Customer customer, Order order)
		{
			IDiscountStrategy strategy = customer.Segment switch
			{
				CustomerSegment.New => new NewCustomerDiscount(context),
				CustomerSegment.Loyal => new LoyalCustomerDiscount(context),
				CustomerSegment.VIP => new VIPCustomerDiscount(context),
				_ => throw new InvalidOperationException("Unknown customer segment.")
			};

			return strategy.ApplyDiscount(customer, order);
		}
	}
}