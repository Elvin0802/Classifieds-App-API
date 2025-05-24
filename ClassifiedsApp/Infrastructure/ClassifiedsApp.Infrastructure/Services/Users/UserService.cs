using ClassifiedsApp.Application.Common.Helpers;
using ClassifiedsApp.Application.Dtos.Auth.Users;
using ClassifiedsApp.Application.Interfaces.Services.Cache;
using ClassifiedsApp.Application.Interfaces.Services.Mail;
using ClassifiedsApp.Application.Interfaces.Services.Users;
using ClassifiedsApp.Core.Entities;
using ClassifiedsApp.Infrastructure.Services.Cache;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;

namespace ClassifiedsApp.Infrastructure.Services.Users;

public class UserService : IUserService
{
	readonly UserManager<AppUser> _userManager;
	readonly ICacheService _cacheService;
	readonly IMailService _mailService;

	public UserService(UserManager<AppUser> userManager, ICacheService cacheService, IMailService mailService)
	{
		_userManager = userManager;
		_cacheService = cacheService;
		_mailService = mailService;
	}

	public async Task<bool> ChangePasswordAsync(string userId, string oldPassword, string newPassword)
	{
		AppUser? user = await _userManager.FindByIdAsync(userId);

		if (user is null)
			return false;

		var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);

		if (result.Succeeded)
			await _userManager.UpdateSecurityStampAsync(user);

		return result.Succeeded;
	}

	public async Task<bool> CreateAsyncOld(CreateAppUserDto dto) // *
	{
		var exisitingUser = await _userManager.FindByEmailAsync(dto.Email);

		if (exisitingUser is not null)
			return false;

		var user = new AppUser
		{
			Email = dto.Email,
			UserName = dto.Email,
			Name = dto.Name,
			PhoneNumber = dto.PhoneNumber,
			CreatedAt = DateTimeOffset.UtcNow,
			UpdatedAt = DateTimeOffset.UtcNow,
			ArchivedAt = DateTimeOffset.MinValue
		};

		var result = await _userManager.CreateAsync(user, dto.Password);

		if (result.Succeeded)
			await _userManager.AddToRoleAsync(user, "User");

		return result.Succeeded;
	}

	public async Task<bool> CreateAsync(CreateAppUserDto dto) // *
	{
		var existingUser = await _userManager.FindByEmailAsync(dto.Email);
		if (existingUser is not null)
			return false;

		var user = new AppUser
		{
			Email = dto.Email,
			EmailConfirmed = false,
			UserName = dto.Email,
			Name = dto.Name,
			PhoneNumber = dto.PhoneNumber,
			PhoneNumberConfirmed = true,
			CreatedAt = DateTimeOffset.UtcNow,
			UpdatedAt = DateTimeOffset.UtcNow,
			ArchivedAt = DateTimeOffset.MinValue
		};

		var result = await _userManager.CreateAsync(user, dto.Password);

		if (result.Succeeded)
		{
			var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

			var verificationCode = new Random().Next(100001, 999998).ToString();

			await StoreVerificationCodeAsync(user.Email, verificationCode, token);

			await _mailService.SendEmailConfirmMailAsync(dto.Email, verificationCode);

			await _userManager.AddToRoleAsync(user, "User");
		}

		return result.Succeeded;
	}

	public async Task<bool> VerifyEmailAsync(string email, string verificationCode)
	{
		var cacheKey = $"verification_{email}";

		var cache = await _cacheService.GetAsync<string>(cacheKey);

		if (cache is null)
			return false;

		var data = JsonSerializer.Deserialize<EmailVerificationDto>(cache.ToString());

		if (data is null || data.Code != verificationCode)
			return false;

		if (DateTime.UtcNow - data.Timestamp > TimeSpan.FromMinutes(10))
			return false;

		var user = await _userManager.FindByEmailAsync(email);
		if (user == null)
			return false;

		var result = await _userManager.ConfirmEmailAsync(user, data.Token);

		if (result.Succeeded)
		{
			await _cacheService.RemoveAsync(cacheKey);
			return true;
		}

		return false;
	}

	public async Task<bool> ResendVerificationCodeAsync(string email)
	{
		var user = await _userManager.FindByEmailAsync(email);
		if (user == null || user.EmailConfirmed)
			return false;

		var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

		var verificationCode = new Random().Next(100001, 999998).ToString();

		await StoreVerificationCodeAsync(email, verificationCode, token);

		await _mailService.SendEmailConfirmMailAsync(email, verificationCode);

		return true;
	}

	public async Task StoreVerificationCodeAsync(string email, string code, string token)
	{
		var cacheKey = $"verification_{email}";

		EmailVerificationDto verificationData = new() { Code = code, Token = token, Timestamp = DateTime.UtcNow };

		var json = JsonSerializer.Serialize(verificationData);

		await _cacheService.SetAsync(cacheKey, json, TimeSpan.FromMinutes(10));
	}

	public async Task<bool> UpdatePasswordAsync(string userId, string resetToken, string newPassword)
	{
		AppUser? user = await _userManager.FindByIdAsync(userId);

		if (user is null)
			return false;

		var result = await _userManager.ResetPasswordAsync(user, resetToken.UrlDecode(), newPassword);

		if (result.Succeeded)
			await _userManager.UpdateSecurityStampAsync(user);

		return result.Succeeded;
	}

	public async Task UpdateRefreshTokenAsync(AppUser user, string refreshToken, DateTimeOffset expiresAt)
	{
		if (user is null)
			throw new Exception("User is NULL !");

		user.RefreshToken = refreshToken;
		user.RefreshTokenExpiresAt = expiresAt;

		await _userManager.UpdateAsync(user);
	}
}
