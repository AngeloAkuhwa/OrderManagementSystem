using System.Text.Json.Serialization;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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

		public class Handler(AppDbContext context, ILogger<Handler> logger) : IRequestHandler<Command, OrderStatusResult>
		{
			public async Task<OrderStatusResult> Handle(Command request, CancellationToken cancellationToken)
			{
				logger.LogInformation("Processing status update for OrderId: {OrderId} to {NewStatus}", request.OrderId, request.NewStatus);
				var order = await context.Orders.FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

				if (order is null)
				{
					logger.LogWarning("Order with ID {OrderId} not found.", request.OrderId);
					return OrderStatusResult.NotFound(request.OrderId);
				}

				if (!OrderStatusTransitionValidator.IsValidTransition(order.Status, request.NewStatus))
				{
					logger.LogWarning("Invalid transition from {OldStatus} to {NewStatus} for OrderId {OrderId}", order.Status, request.NewStatus, order.Id);
					return OrderStatusResult.InvalidTransition(order.Id, order.Status, request.NewStatus);
				}

				var oldStatus = order.Status;
				order.Status = request.NewStatus;

				if (request.NewStatus == OrderStatus.Delivered)
				{
					order.FulfilledAt = DateTime.UtcNow;
					logger.LogInformation("Order {OrderId} marked as Delivered at {FulfilledAt}", order.Id, order.FulfilledAt);
				}

				await context.SaveChangesAsync(cancellationToken);
				logger.LogInformation("Successfully updated OrderId {OrderId} status from {OldStatus} to {NewStatus}", order.Id, oldStatus, order.Status);

				return OrderStatusResult.SuccessUpdate(order.Id, oldStatus, order.Status);
			}
		}
	}
}
