using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Domain.Entities;

namespace OrderManagementSystem.Infrastructure.Data
{
	public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
	{
		public DbSet<Customer> Customers => Set<Customer>();
		public DbSet<Order> Orders => Set<Order>();

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Customer>(entity =>
			{
				entity.HasKey(c => c.Id);
				entity.Property(c => c.FirstName).IsRequired().HasMaxLength(50);
				entity.Property(c => c.LastName).IsRequired().HasMaxLength(50);
				entity.Property(c => c.Email).IsRequired().HasMaxLength(50);
				entity.HasMany(c => c.Orders)
					.WithOne(o => o.Customer)
					.HasForeignKey(o => o.CustomerId);
			});

			modelBuilder.Entity<Order>(entity =>
			{
				entity.HasKey(o => o.Id);
				entity.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");
				entity.Property(o => o.CreatedAt).IsRequired();
				entity.Property(o => o.Status).IsRequired();
			});
		}
	}
}