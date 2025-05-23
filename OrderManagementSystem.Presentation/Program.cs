using MediatR;

using Microsoft.EntityFrameworkCore;

using OrderManagementSystem.Application;
using OrderManagementSystem.Application.Interfaces;
using OrderManagementSystem.Application.Services.DiscountServices;
using OrderManagementSystem.Infrastructure.Data;
using OrderManagementSystem.Presentation.Middleware;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
	.WriteTo.Console()
	.WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
	.Enrich.FromLogContext()
	.MinimumLevel.Debug()
	.CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();

if (builder.Environment.IsEnvironment("Testing"))
{
	builder.Services.AddDbContext<AppDbContext>(options =>
		options.UseInMemoryDatabase("TestDb"));
}
else
{
	builder.Services.AddDbContext<AppDbContext>(options =>
		options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}

builder.Services.AddMediatR(typeof(AssemblyMarker).Assembly);
builder.Services.AddScoped<IDiscountService, DiscountService>();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
	var xmlFile = $"{typeof(Program).Assembly.GetName().Name}.xml";
	var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
	c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	DbSeeder.Seed(db);
}

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

try
{
	Log.Information("Starting web host");
	app.Run();
}
catch (Exception ex)
{
	Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
	Log.CloseAndFlush();
}

public partial class Program { }