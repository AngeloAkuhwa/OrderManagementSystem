using System.ComponentModel.DataAnnotations;
using OrderManagementSystem.Domain.Enums;

namespace OrderManagementSystem.Domain.Entities
{
	public class Order
	{
		[Key]
		public Guid Id { get; set; }

		[Required]
		public Guid CustomerId { get; set; }

		[Required]
		[Range(0.01, double.MaxValue, ErrorMessage = "TotalAmount must not be less than zero.")]
		public decimal TotalAmount { get; set; }

		public OrderStatus Status { get; set; }

		[Required]
		public DateTime CreatedAt { get; set; }

		public DateTime? FulfilledAt { get; set; }

		public Customer Customer { get; set; } = null!;
	}
}