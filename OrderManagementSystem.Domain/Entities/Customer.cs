using System.ComponentModel.DataAnnotations;
using OrderManagementSystem.Domain.Enums;

namespace OrderManagementSystem.Domain.Entities
{
	public class Customer
	{
		[Key]
		public Guid Id { get; set; }

		[Required]
		[StringLength(50)]
		public string FirstName { get; set; }

		[Required]
		[StringLength(50)]
		public string LastName { get; set; }

		[Required]
		[EmailAddress]
		public string Email { get; set; }

		public CustomerSegment Segment { get; set; }

		public List<Order> Orders { get; set; }	= new();
	}
}