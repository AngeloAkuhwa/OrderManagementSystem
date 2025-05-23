using OrderManagementSystem.Domain.Entities;

namespace OrderManagementSystem.Application.Interfaces
{
	public interface IDiscountService
	{
		decimal ApplyDiscount(Customer customer, Order order);
	}
}
