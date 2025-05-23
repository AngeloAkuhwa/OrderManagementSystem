using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Infrastructure.Data;

namespace OrderManagementSystem.Application.Features.Orders.Queries
{
	public static class GetOrderAnalytics
	{
		public class Query : IRequest<OrderAnalyticsResult> { }

		public class OrderAnalyticsResult
		{
			public OrderSummary Summary { get; set; }	 = new();
			public List<StatusCount> StatusBreakdown { get; set; } = new();
			public List<MonthlyCount> MonthlyOrderCounts { get; set; } = new();
			public List<RangeHistogram> ValueRangeHistogram { get; set; } = new();
			public List<RecentOrder> RecentOrders { get; set; } = new();
		}

		public class OrderSummary
		{
			public int TotalOrders { get; set; }
			public decimal AverageOrderValue { get; set; }
			public double FulfillmentRate { get; set; }
			public double AverageFulfillmentTimeInHours { get; set; }
		}

		public class StatusCount
		{
			public string Status { get; set; }
			public int Count { get; set; }
		}

		public class MonthlyCount
		{
			public string Month { get; set; }
			public int Count { get; set; }
		}

		public class RangeHistogram
		{
			public string Range { get; set; } = string.Empty;
			public int Count { get; set; }
		}

		public class RecentOrder
		{
			public Guid OrderId { get; set; }
			public decimal Amount { get; set; }
			public string Status { get; set; } = string.Empty;
			public DateTime Date { get; set; }
		}

		public class Handler(AppDbContext context) : IRequestHandler<Query, OrderAnalyticsResult>
		{
			public async Task<OrderAnalyticsResult> Handle(Query request, CancellationToken cancellationToken)
			{
				var now = DateTime.UtcNow;
				var orders = await context.Orders
						.AsNoTracking()
						.Include(o => o.Customer)
						.ToListAsync(cancellationToken);

				var totalOrders = orders.Count;
				var fulfilled = orders.Where(o => o.FulfilledAt.HasValue).ToList();

				var result = new OrderAnalyticsResult
				{
					Summary =
					{
						TotalOrders = totalOrders,
						AverageOrderValue = totalOrders > 0 ? Math.Round(orders.Average(o => o.TotalAmount), 2) : 0,
						FulfillmentRate = totalOrders > 0 ? Math.Round((double)fulfilled.Count / totalOrders * 100, 2) : 0,
						AverageFulfillmentTimeInHours = fulfilled.Any() ? 
							Math.Round(fulfilled.Average(o => (o.FulfilledAt!.Value - o.CreatedAt).TotalHours), 2) : 0
					},
					StatusBreakdown = orders
						.GroupBy(o => o.Status.ToString())
						.Select(g => new StatusCount
						{
							Status = g.Key,
							Count = g.Count()
						})
						.ToList(),
					MonthlyOrderCounts = orders
						.Where(o => o.CreatedAt > now.AddMonths(-6))
						.GroupBy(o => o.CreatedAt.ToString("yyyy-MM"))
						.OrderBy(g => g.Key)
						.Select(g => new MonthlyCount
						{
							Month = g.Key,
							Count = g.Count()
						})
						.ToList(),
					ValueRangeHistogram =
					[
						new RangeHistogram { Range = "0–100", Count = orders.Count(o => o.TotalAmount <= 100) },
						new RangeHistogram { Range = "101–300", Count = orders.Count(o => o.TotalAmount is > 100 and <= 300) },
						new RangeHistogram { Range = "301–600", Count = orders.Count(o => o.TotalAmount is > 300 and <= 600) },
						new RangeHistogram { Range = "601+", Count = orders.Count(o => o.TotalAmount > 600) }
					],
					RecentOrders = orders
						.OrderByDescending(o => o.CreatedAt)
						.Take(5)
						.Select(o => new RecentOrder
						{
							OrderId = o.Id,
							Amount = o.TotalAmount,
							Status = o.Status.ToString(),
							Date = o.CreatedAt
						})
						.ToList()
				};

				return result;
			}
		}
	}
}
