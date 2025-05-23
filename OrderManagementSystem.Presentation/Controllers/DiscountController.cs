using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.Application.Features.Discounts.Queries;

namespace OrderManagementSystem.Presentation.Controllers
{
	/// <summary>
	/// Handles discount-related operations.
	/// </summary>
	[ApiController]
	[Route("api/v1/[controller]")]
	public class DiscountController(IMediator mediator) : ControllerBase
	{
		/// <summary>
		/// Applies a customer-segment-based discount to an order.
		/// </summary>
		/// <param name="orderId">The ID of the order to discount.</param>
		/// <returns>Discount summary including original and discounted amounts.</returns>
		/// <response code="200">Discount applied successfully</response>
		/// <response code="404">Order or customer not found</response>
		[HttpPost("{orderId}/apply-discount")]
		[ProducesResponseType(typeof(GetCustomerOrderDiscount.OrderDiscountResult), 200)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> ApplyDiscount(Guid orderId)
		{
			var result = await mediator.Send(new GetCustomerOrderDiscount.Query { OrderId = orderId });
			return Ok(result);
		}
	}
}