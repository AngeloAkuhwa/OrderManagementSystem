using OrderManagementSystem.Domain.Enums;

namespace OrderManagementSystem.Domain.Helpers
{
	public static class OrderStatusTransitionValidator
	{
		private static readonly Dictionary<OrderStatus, OrderStatus[]> _validTransitions = new()
		{
			{ OrderStatus.Created, new[] { OrderStatus.Processing, OrderStatus.Cancelled } },
			{ OrderStatus.Processing, new[] { OrderStatus.Shipped, OrderStatus.Cancelled } },
			{ OrderStatus.Shipped, new[] { OrderStatus.Delivered } },
			{ OrderStatus.Delivered, Array.Empty<OrderStatus>() },
			{ OrderStatus.Cancelled, Array.Empty<OrderStatus>() }
		};

		public static bool IsValidTransition(OrderStatus current, OrderStatus next)
		{
			return _validTransitions.TryGetValue(current, out var validNextStates) && validNextStates.Contains(next);
		}
	}
}