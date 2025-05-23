using FluentAssertions;

namespace OrderManagementSystem.Tests.IntegrationTests;

public class OrderAnalyticsTests(CustomTestFactory factory) : IClassFixture<CustomTestFactory>
{
	private readonly HttpClient _client = factory.CreateClient();

	[Fact]
	public async Task Should_Return_Order_Analytics()
	{
		var response = await _client.GetAsync("/api/v1/orders/analytics");
		response.EnsureSuccessStatusCode();

		var content = await response.Content.ReadAsStringAsync();
		content.Should().Contain("summary").And.Contain("monthlyOrderCounts");
	}
}