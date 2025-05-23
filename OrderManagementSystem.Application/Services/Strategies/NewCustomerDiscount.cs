using OrderManagementSystem.Domain.Entities;
using OrderManagementSystem.Application.Interfaces;

namespace OrderManagementSystem.Application.Services.Strategies
{
	public class NewCustomerDiscount : IDiscountStrategy
	{
		public decimal ApplyDiscount(Customer customer, Order order)
		{
			return order.TotalAmount * 0.90m; // 10% discount
		}
	}
}