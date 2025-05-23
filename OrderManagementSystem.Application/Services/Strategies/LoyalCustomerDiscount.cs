using OrderManagementSystem.Domain.Entities;
using OrderManagementSystem.Application.Interfaces;

namespace OrderManagementSystem.Application.Services.Strategies
{
	public class LoyalCustomerDiscount : IDiscountStrategy
	{
		public decimal ApplyDiscount(Customer customer, Order order)
		{
			return customer.Orders.Count >= 5 ? order.TotalAmount * 0.85m : order.TotalAmount;
		}
	}
}