using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderManagementSystem.Infrastructure.Data;
using OrderManagementSystem.Tests.DbTestSeeder;

namespace OrderManagementSystem.Tests.IntegrationTests
{
	public class CustomTestFactory : WebApplicationFactory<Program>
	{
		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			builder.UseEnvironment("Testing");

			builder.ConfigureServices(services =>
			{
				var dbContextDescriptor = services.SingleOrDefault(
					d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

				if (dbContextDescriptor != null)
					services.Remove(dbContextDescriptor);

				var dbContextImplementation = services.SingleOrDefault(
					d => d.ImplementationType == typeof(AppDbContext));

				if (dbContextImplementation != null)
					services.Remove(dbContextImplementation);

				services.AddDbContext<AppDbContext>(options =>
				{
					options.UseInMemoryDatabase("TestDb");
				});

				var sp = services.BuildServiceProvider();

				using var scope = sp.CreateScope();
				var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

				context.Database.EnsureCreated();
				TestSeeder.Seed(context);
			});
		}
	}
}