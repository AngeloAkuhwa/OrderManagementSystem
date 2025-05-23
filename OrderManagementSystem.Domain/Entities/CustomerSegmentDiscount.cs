using System.ComponentModel.DataAnnotations;

using OrderManagementSystem.Domain.Enums;

namespace OrderManagementSystem.Domain.Entities;
public class CustomerSegmentDiscount
{
	public Guid Id { get; set; }
	public CustomerSegment Segment { get; set; }
	public decimal DiscountRate { get; set; }
	public decimal? ThresholdAmount { get; set; }
	public int? MinOrders { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime ModifiedOn { get; set; }
}