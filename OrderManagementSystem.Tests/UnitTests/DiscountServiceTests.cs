using FluentAssertions;
using Moq;
using OrderManagementSystem.Application.Interfaces;
using OrderManagementSystem.Domain.Entities;
using OrderManagementSystem.Domain.Enums;
using Xunit;

namespace OrderManagementSystem.Tests.UnitTests
{
	public class DiscountServiceTests
	{
		private readonly Mock<IDiscountService> _mockDiscountService = new();

		[Fact]
		public void Should_Apply_10Percent_Discount_For_NewCustomer()
		{
			var customer = new Customer { Segment = CustomerSegment.New };
			var order = new Order { TotalAmount = 100 };

			_mockDiscountService.Setup(x => x.ApplyDiscount(customer, order)).Returns(90);

			var discounted = _mockDiscountService.Object.ApplyDiscount(customer, order);
			discounted.Should().Be(90);
		}

		[Fact]
		public void Should_Apply_15Percent_Discount_For_LoyalCustomer_With_5_Orders()
		{
			var customer = new Customer
			{
				Segment = CustomerSegment.Loyal,
				Orders = [new Order(), new Order(), new Order(), new Order(), new Order()]
			};

			var order = new Order { TotalAmount = 200 };

			_mockDiscountService.Setup(x => x.ApplyDiscount(customer, order)).Returns(170);

			var discounted = _mockDiscountService.Object.ApplyDiscount(customer, order);
			discounted.Should().Be(170);
		}

		[Fact]
		public void Should_Not_Apply_Loyal_Discount_If_Not_Enough_Orders()
		{
			var customer = new Customer
			{
				Segment = CustomerSegment.Loyal,
				Orders = [new Order(), new Order()]
			};

			var order = new Order { TotalAmount = 200 };

			_mockDiscountService.Setup(x => x.ApplyDiscount(customer, order)).Returns(200);

			var discounted = _mockDiscountService.Object.ApplyDiscount(customer, order);
			discounted.Should().Be(200);
		}

		[Fact]
		public void Should_Apply_20Percent_Discount_For_VIPCustomer_If_TotalSpent_Over_5000()
		{
			var customer = new Customer
			{
				Segment = CustomerSegment.VIP,
				Orders = Enumerable.Range(0, 10).Select(i => new Order { TotalAmount = 600 }).ToList()
			};

			var order = new Order { TotalAmount = 1000 };

			_mockDiscountService.Setup(x => x.ApplyDiscount(customer, order)).Returns(800);

			var discounted = _mockDiscountService.Object.ApplyDiscount(customer, order);
			discounted.Should().Be(800);
		}

		[Fact]
		public void Should_Not_Apply_VIP_Discount_If_TotalSpent_Under_5000()
		{
			var customer = new Customer
			{
				Segment = CustomerSegment.VIP,
				Orders = Enumerable.Range(0, 2).Select(i => new Order { TotalAmount = 1000 }).ToList()
			};

			var order = new Order { TotalAmount = 800 };

			_mockDiscountService.Setup(x => x.ApplyDiscount(customer, order)).Returns(800);

			var discounted = _mockDiscountService.Object.ApplyDiscount(customer, order);
			discounted.Should().Be(800);
		}

		[Fact]
		public void Should_Throw_For_Unknown_Segment()
		{
			var customer = new Customer();
			var order = new Order { TotalAmount = 100 };

			_mockDiscountService.Setup(x => x.ApplyDiscount(customer, order)).Throws<InvalidOperationException>();

			Action act = () => _mockDiscountService.Object.ApplyDiscount(customer, order);
			act.Should().Throw<InvalidOperationException>();
		}
	}
}
