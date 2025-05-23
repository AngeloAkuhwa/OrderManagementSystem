using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.Application.Features.Orders.Commands;
using OrderManagementSystem.Application.Features.Orders.Queries;

namespace OrderManagementSystem.Presentation.Controllers
{
	/// <summary>
	/// Handles operations related to orders.
	/// </summary>
	[ApiController]
	[Route("api/v1/[controller]")]
	public class OrdersController(IMediator mediator) : ControllerBase
	{
		/// <summary>
		/// Updates the status of an order.
		/// </summary>
		/// <param name="id">The ID of the order to update.</param>
		/// <param name="request">The status update request payload.</param>
		/// <returns>Updated order status information.</returns>
		/// <response code="200">Order status updated</response>
		/// <response code="400">Invalid transition or input</response>
		[HttpPatch("{id}/update-status")]
		[ProducesResponseType(typeof(UpdateOrderStatus.OrderStatusResult), 200)]
		[ProducesResponseType(400)]
		public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateOrderStatus.Command request)
		{
			request.OrderId = id;
			var result = await mediator.Send(request);
			return Ok(result);
		}

		/// <summary>
		/// Gets overall order analytics including trends and breakdowns.
		/// </summary>
		/// <returns>Analytics data such as totals, averages, and recent orders.</returns>
		/// <response code="200">Analytics fetched successfully</response>
		[HttpGet("analytics")]
		[ProducesResponseType(typeof(GetOrderAnalytics.OrderAnalyticsResult), 200)]
		public async Task<IActionResult> GetAnalytics()
		{
			var result = await mediator.Send(new GetOrderAnalytics.Query());
			return Ok(result);
		}
	}
}