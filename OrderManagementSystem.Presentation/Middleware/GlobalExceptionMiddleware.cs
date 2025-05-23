using System.Net;
using System.Text.Json;

namespace OrderManagementSystem.Presentation.Middleware
{
	public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
	{
		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await next(context);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "An unhandled exception occurred");
				await HandleExceptionAsync(context, ex);
			}
		}

		private static Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			context.Response.ContentType = "application/json";
			context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

			var trace = new System.Diagnostics.StackTrace(exception, true);
			var frame = trace.GetFrames()?.FirstOrDefault(f => f.GetFileLineNumber() > 0);
			var method = frame?.GetMethod();
			var declaringType = method?.DeclaringType?.FullName ?? "Unknown";
			var fileName = frame?.GetFileName() ?? "N/A";
			var lineNumber = frame?.GetFileLineNumber() ?? 0;

			var errorResponse = new
			{
				StatusCode = context.Response.StatusCode,
				Message = "An unexpected error occurred. Please try again later.",
				Error = exception.Message,
				ExceptionType = exception.GetType().Name,
				Source = declaringType,
				Method = method?.Name ?? "Unknown",
				File = fileName,
				Line = lineNumber,
				StackTrace = exception.StackTrace
			};

			var json = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
			{
				WriteIndented = true
			});

			return context.Response.WriteAsync(json);
		}

	}
}