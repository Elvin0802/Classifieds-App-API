using ClassifiedsApp.Application.Interfaces.Services.RateLimit;
using Microsoft.AspNetCore.RateLimiting;
using System.Globalization;
using System.Security.Claims;

public class RateLimitingMiddleware
{
	private readonly RequestDelegate _next;

	public RateLimitingMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	public async Task InvokeAsync(HttpContext context, IRateLimitService rateLimitService)
	{
		var endpoint = context.GetEndpoint();
		if (endpoint?.Metadata.GetMetadata<DisableRateLimitingAttribute>() != null)
		{
			await _next(context);
			return;
		}

		var user = context.User;
		bool isAdmin = user.Identity?.IsAuthenticated == true &&
					   user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value == "Admin";
		if (isAdmin)
		{
			await _next(context);
			return;
		}

		string userKey = user.Identity?.IsAuthenticated == true ? user?.FindFirstValue(ClaimTypes.NameIdentifier)! : "anonymous";

		if (rateLimitService.IsLimitExceeded(userKey, out TimeSpan retryAfter))
		{
			context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
			context.Response.Headers["Retry-After"] = retryAfter.TotalSeconds.ToString("F0", CultureInfo.InvariantCulture);
			await context.Response.WriteAsync("Rate limit exceeded. Please try again later.");
			return;
		}

		await _next(context);
	}

}
