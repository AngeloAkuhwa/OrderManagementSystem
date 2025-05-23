using FluentAssertions;
using OrderManagementSystem.Domain.Enums;
using OrderManagementSystem.Domain.Helpers;

namespace OrderManagementSystem.Tests.UnitTests
{
	public class OrderStatusTransitionValidatorTests
	{
		[Theory]
		[InlineData(OrderStatus.Created, OrderStatus.Processing, true)]
		[InlineData(OrderStatus.Processing, OrderStatus.Shipped, true)]
		[InlineData(OrderStatus.Shipped, OrderStatus.Delivered, true)]
		[InlineData(OrderStatus.Created, OrderStatus.Shipped, false)]
		[InlineData(OrderStatus.Delivered, OrderStatus.Created, false)]
		[InlineData(OrderStatus.Cancelled, OrderStatus.Created, false)]
		public void Should_Validate_Status_Transitions(OrderStatus current, OrderStatus next, bool expected)
		{
			var result = OrderStatusTransitionValidator.IsValidTransition(current, next);
			result.Should().Be(expected);
		}
	}
}
