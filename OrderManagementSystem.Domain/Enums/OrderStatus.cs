namespace OrderManagementSystem.Domain.Enums
{
	public enum OrderStatus
	{
		None = 0,
		Created = 1,
		Processing = 2,
		Shipped = 3,
		Delivered = 4,
		Cancelled = 5
	}
}