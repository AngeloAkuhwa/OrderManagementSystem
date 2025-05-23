using System.Text.Json.Serialization;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Domain.Enums;
using OrderManagementSystem.Domain.Helpers;
using OrderManagementSystem.Infrastructure.Data;

namespace OrderManagementSystem.Application.Features.Orders.Commands
{
	public static class UpdateOrderStatus
	{
		public class Command : IRequest<OrderStatusResult>
		{
			[JsonIgnore]
			public Guid OrderId { get; set; }
			public OrderStatus NewStatus { get; init; }
		}

		public class OrderStatusResult
		{
			public Guid OrderId { get; set; }
			public OrderStatus OldStatus { get; set; }
			public OrderStatus NewStatus { get; set; }
			public string Message { get; set; } = string.Empty;
		}

		public class Handler(AppDbContext context) : IRequestHandler<Command, OrderStatusResult>
		{
			public async Task<OrderStatusResult> Handle(Command request, CancellationToken cancellationToken)
			{
				var order = await context.Orders.FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

				if (order is null)
					throw new KeyNotFoundException("Order not found.");

				if (!OrderStatusTransitionValidator.IsValidTransition(order.Status, request.NewStatus))
					throw new InvalidOperationException($"Cannot change order status from {order.Status} to {request.NewStatus}.");

				var oldStatus = order.Status;
				order.Status = request.NewStatus;

				if (request.NewStatus == OrderStatus.Delivered)
					order.FulfilledAt = DateTime.UtcNow;

				await context.SaveChangesAsync(cancellationToken);

				return new OrderStatusResult
				{
					OrderId = order.Id,
					OldStatus = oldStatus,
					NewStatus = order.Status,
					Message = $"Order status updated from {oldStatus} to {order.Status}."
				};
			}
		}
	}
}
