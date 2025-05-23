using OrderManagementSystem.Domain.Entities;
using OrderManagementSystem.Application.Interfaces;

namespace OrderManagementSystem.Application.Services.Strategies
{
	public class VIPCustomerDiscount : IDiscountStrategy
	{
		public decimal ApplyDiscount(Customer customer, Order order)
		{
			var totalSpent = customer.Orders.Sum(o => o.TotalAmount);
			return totalSpent > 5000 ? order.TotalAmount * 0.80m : order.TotalAmount;
		}
	}
}