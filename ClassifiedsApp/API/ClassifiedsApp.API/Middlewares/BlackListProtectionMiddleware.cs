using ClassifiedsApp.Application.Interfaces.Services.BlackList;

namespace ClassifiedsApp.API.Middlewares;

public class BlackListProtectionMiddleware
{
	private readonly RequestDelegate _next;
	private readonly IServiceScopeFactory _serviceScopeFactory;

	public BlackListProtectionMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory)
	{
		_next = next;
		_serviceScopeFactory = serviceScopeFactory;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		if (context.Request.Path.StartsWithSegments("/api/auth") || !context.User.Identity.IsAuthenticated)
		{
			await _next(context);
			return;
		}

		using var scope = _serviceScopeFactory.CreateScope();
		var blacklistService = scope.ServiceProvider.GetRequiredService<IBlacklistService>();

		var userIdClaim = context.User.FindFirst("UserId")?.Value;
		if (userIdClaim != null && Guid.TryParse(userIdClaim, out var userId))
		{
			if (await blacklistService.IsUserBlacklistedAsync(userId))
			{
				context.Response.StatusCode = 403; // 403 Forbidden.
				await context.Response.WriteAsync("User is blocked.");
				return;
			}
		}

		await _next(context);
	}
}
