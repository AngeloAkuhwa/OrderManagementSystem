using FluentAssertions;

namespace OrderManagementSystem.Tests.IntegrationTests;

public class ApplyDiscountTests(CustomTestFactory factory) : IClassFixture<CustomTestFactory>
{
	private readonly HttpClient _client = factory.CreateClient();

	[Fact]
	public async Task Should_Return_Discounted_Amount_For_Order()
	{
		var orderId = GetSampleOrderIdForCustomerSegment("VIP");

		var response = await _client.PostAsync($"/api/v1/discount/{orderId}/apply-discount", null);
		response.EnsureSuccessStatusCode();

		var content = await response.Content.ReadAsStringAsync();
		content.Should().Contain("discountedAmount");
	}

	private Guid GetSampleOrderIdForCustomerSegment(string segment)
	{
		return segment switch
		{
			"VIP" => Guid.Parse("33333333-3333-3333-3333-333333333333"),
			"Loyal" => Guid.Parse("22222222-2222-2222-2222-222222222222"),
			"New" => Guid.Parse("11111111-1111-1111-1111-111111111111"),
			_ => throw new ArgumentException("Invalid segment name.")
		};
	}
}