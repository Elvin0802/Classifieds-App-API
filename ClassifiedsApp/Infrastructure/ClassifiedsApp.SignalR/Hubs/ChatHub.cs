using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace ClassifiedsApp.SignalR.Hubs;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,User")]
public class ChatHub : Hub
{
	readonly ILogger<ChatHub> _logger;

	public ChatHub(ILogger<ChatHub> logger)
	{
		_logger = logger;
	}

	public override async Task OnConnectedAsync()
	{
		var userId = Context.User!.FindFirstValue(ClaimTypes.NameIdentifier);

		_logger.LogWarning($"ChatHub OnConnectedAsync , userId = {userId} |");

		if (!string.IsNullOrEmpty(userId))
			await Groups.AddToGroupAsync(Context.ConnectionId, userId);

		await base.OnConnectedAsync();
	}

	public override async Task OnDisconnectedAsync(Exception exception)
	{
		var userId = Context.User!.FindFirstValue(ClaimTypes.NameIdentifier);

		_logger.LogWarning($"ChatHub OnDisconnectedAsync , userId = {userId} |");

		if (!string.IsNullOrEmpty(userId))
			await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);

		await base.OnDisconnectedAsync(exception);
	}
}
