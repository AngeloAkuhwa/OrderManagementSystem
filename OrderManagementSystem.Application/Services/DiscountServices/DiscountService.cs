using OrderManagementSystem.Application.Interfaces;
using OrderManagementSystem.Application.Services.Strategies;
using OrderManagementSystem.Domain.Entities;
using OrderManagementSystem.Domain.Enums;

namespace OrderManagementSystem.Application.Services.DiscountServices
{
	public class DiscountService : IDiscountService
	{
		public decimal ApplyDiscount(Customer customer, Order order)
		{
			IDiscountStrategy strategy = customer.Segment switch
			{
				CustomerSegment.New => new NewCustomerDiscount(),
				CustomerSegment.Loyal => new LoyalCustomerDiscount(),
				CustomerSegment.VIP => new VIPCustomerDiscount(),
				_ => throw new InvalidOperationException("Unknown customer segment.")
			};

			return strategy.ApplyDiscount(customer, order);
		}
	}
}
