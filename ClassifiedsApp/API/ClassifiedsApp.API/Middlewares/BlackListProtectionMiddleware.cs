using ClassifiedsApp.Application.Interfaces.Services.BlackList;

namespace ClassifiedsApp.API.Middlewares;

public class BlackListProtectionMiddleware
{
	private readonly RequestDelegate _next;
	private readonly IServiceScopeFactory _serviceScopeFactory;
	readonly ILogger<BlackListProtectionMiddleware> _logger;

	public BlackListProtectionMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory,
											ILogger<BlackListProtectionMiddleware> logger)
	{
		_next = next;
		_serviceScopeFactory = serviceScopeFactory;
		_logger = logger;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		using var scope = _serviceScopeFactory.CreateScope();
		var blacklistService = scope.ServiceProvider.GetRequiredService<IBlacklistService>();

		var userIdClaim = context.User.FindFirst("UserId")?.Value;
		if (userIdClaim != null && Guid.TryParse(userIdClaim, out var userId))
		{
			if (await blacklistService.IsUserBlacklistedAsync(userId))
			{
				context.Response.StatusCode = 403; // 403 Forbidden.
				await context.Response.WriteAsync("User is blocked.");

				_logger.LogError($"failed for : {userId}");
				return;
			}
		}

		await _next(context);
	}
}
