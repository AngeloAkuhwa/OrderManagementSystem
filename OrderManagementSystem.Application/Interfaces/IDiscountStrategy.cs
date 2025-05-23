using OrderManagementSystem.Domain.Entities;

namespace OrderManagementSystem.Application.Interfaces
{
	public interface IDiscountStrategy
	{
		decimal ApplyDiscount(Customer customer, Order order);
	}
}