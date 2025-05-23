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
			public bool Success { get; set; }
			public Guid OrderId { get; set; }
			public OrderStatus? OldStatus { get; set; }
			public OrderStatus? NewStatus { get; set; }
			public string Message { get; set; } = string.Empty;

			public static OrderStatusResult NotFound(Guid orderId) => new()
			{
				Success = false,
				OrderId = orderId,
				Message = "Order not found."
			};

			public static OrderStatusResult InvalidTransition(Guid orderId, OrderStatus current, OrderStatus attempted) => new()
			{
				Success = false,
				OrderId = orderId,
				OldStatus = current,
				NewStatus = attempted,
				Message = $"Invalid transition: Cannot change status from {current} to {attempted}."
			};

			public static OrderStatusResult SuccessUpdate(Guid orderId, OrderStatus oldStatus, OrderStatus newStatus) => new()
			{
				Success = true,
				OrderId = orderId,
				OldStatus = oldStatus,
				NewStatus = newStatus,
				Message = $"Order status updated from {oldStatus} to {newStatus}."
			};
		}

		public class Handler(AppDbContext context) : IRequestHandler<Command, OrderStatusResult>
		{
			public async Task<OrderStatusResult> Handle(Command request, CancellationToken cancellationToken)
			{
				var order = await context.Orders.FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

				if (order is null)
				{
					return OrderStatusResult.NotFound(request.OrderId);
				}

				if (!OrderStatusTransitionValidator.IsValidTransition(order.Status, request.NewStatus))
				{
					return OrderStatusResult.InvalidTransition(order.Id, order.Status, request.NewStatus);
				}

				var oldStatus = order.Status;
				order.Status = request.NewStatus;

				if (request.NewStatus == OrderStatus.Delivered)
				{
					order.FulfilledAt = DateTime.UtcNow;
				}

				await context.SaveChangesAsync(cancellationToken);

				return OrderStatusResult.SuccessUpdate(order.Id, oldStatus, order.Status);
			}
		}
	}
}
