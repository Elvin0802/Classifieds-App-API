using Microsoft.AspNetCore.SignalR;
using System.IdentityModel.Tokens.Jwt;

namespace ClassifiedsApp.SignalR.Hubs;

public class ChatHub : Hub
{
	public override async Task OnConnectedAsync()
	{
		var userId = Context.User!.FindFirst(JwtRegisteredClaimNames.Sub)!.Value;

		if (!string.IsNullOrEmpty(userId))
			await Groups.AddToGroupAsync(Context.ConnectionId, userId);

		await base.OnConnectedAsync();
	}

	public override async Task OnDisconnectedAsync(Exception exception)
	{
		var userId = Context.User!.FindFirst(JwtRegisteredClaimNames.Sub)!.Value;

		if (!string.IsNullOrEmpty(userId))
			await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);

		await base.OnDisconnectedAsync(exception);
	}
}