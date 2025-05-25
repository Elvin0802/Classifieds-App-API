using ClassifiedsApp.Application.Dtos.Auth.Users;
using ClassifiedsApp.Application.Interfaces.Repositories.Ads;
using ClassifiedsApp.Application.Interfaces.Services.BlackList;
using ClassifiedsApp.Core.Entities;
using ClassifiedsApp.Core.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace ClassifiedsApp.Infrastructure.Services.BlackList;

public class BlacklistService : IBlacklistService
{
	readonly UserManager<AppUser> _userManager;
	readonly IAdWriteRepository _adWriteRepository;
	readonly ILogger<BlacklistService> _logger;

	public BlacklistService(UserManager<AppUser> userManager, ILogger<BlacklistService> logger,
	IAdWriteRepository adWriteRepository)
	{
		_userManager = userManager;
		_adWriteRepository = adWriteRepository;
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
		user.RefreshToken = null;
		user.RefreshTokenExpiresAt = null;

		foreach (var ad in user.Ads)
		{
			ad.ArchivedAt = DateTimeOffset.UtcNow;
			ad.Status = AdStatus.Archived;
		}

		await _adWriteRepository.SaveAsync();

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

		foreach (var ad in user.Ads)
		{
			ad.ExpiresAt = DateTimeOffset.UtcNow.AddDays(-1);
			ad.Status = AdStatus.Expired;
		}

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