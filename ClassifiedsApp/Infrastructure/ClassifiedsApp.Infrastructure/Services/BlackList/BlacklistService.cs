using ClassifiedsApp.Application.Dtos.Auth.Users;
using ClassifiedsApp.Application.Interfaces.Services.BlackList;
using ClassifiedsApp.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace ClassifiedsApp.Infrastructure.Services.BlackList;

public class BlacklistService : IBlacklistService
{
	private readonly UserManager<AppUser> _userManager;
	private readonly ILogger<BlacklistService> _logger;

	public BlacklistService(UserManager<AppUser> userManager, ILogger<BlacklistService> logger)
	{
		_userManager = userManager;
		_logger = logger;
	}

	public async Task<bool> IsUserBlacklistedAsync(string email)
	{
		var user = await _userManager.FindByEmailAsync(email);
		return user?.IsBlacklisted ?? false;
	}

	public async Task<bool> IsUserBlacklistedAsync(Guid userId)
	{
		var user = await _userManager.FindByIdAsync(userId.ToString());
		return user?.IsBlacklisted ?? false;
	}

	public async Task BlacklistUserAsync(string email, string reason)
	{
		var user = await _userManager.FindByEmailAsync(email)
			?? throw new KeyNotFoundException($"User not found with email: {email}");

		if (user.IsBlacklisted)
			throw new InvalidOperationException($"User with email '{email}' is already blacklisted.");

		user.IsBlacklisted = true;
		user.BlacklistReason = reason;
		user.BlacklistedAt = DateTimeOffset.UtcNow;
		user.RefreshToken = null; // Invalidate refresh token
		user.RefreshTokenExpiresAt = null;

		var result = await _userManager.UpdateAsync(user);
		if (!result.Succeeded)
			throw new InvalidOperationException("Failed to blacklist user.");

		_logger.LogInformation("User {Email} blacklisted by admin , Reason: {Reason}",
			email, reason);
	}

	public async Task UnblacklistUserAsync(string email)
	{
		var user = await _userManager.FindByEmailAsync(email)
			?? throw new KeyNotFoundException($"User not found with email: {email}");

		if (!user.IsBlacklisted)
			throw new InvalidOperationException($"User with email '{email}' is not blacklisted.");

		user.IsBlacklisted = false;
		user.BlacklistReason = null;
		user.BlacklistedAt = null;

		var result = await _userManager.UpdateAsync(user);
		if (!result.Succeeded)
			throw new InvalidOperationException("Failed to unblacklist user.");

		_logger.LogInformation("User {Email} unblacklisted by admin.", email);
	}

	public async Task<IEnumerable<BlacklistUserDto>> GetBlacklistedUsersAsync()
	{
		var blacklistedUsers = _userManager.Users
			.Where(u => u.IsBlacklisted)
			.Select(u => new BlacklistUserDto
			{
				UserId = u.Id,
				Email = u.Email!,
				Reason = u.BlacklistReason!,
				BlacklistedAt = u.BlacklistedAt!.Value
			})
			.ToList();

		return blacklistedUsers;
	}
}