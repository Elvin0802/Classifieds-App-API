﻿using ClassifiedsApp.Application.Dtos.Auth.Users;
using ClassifiedsApp.Core.Entities;

namespace ClassifiedsApp.Application.Interfaces.Services.Users;

public interface IUserService
{
	Task<bool> CreateAsync(CreateAppUserDto dto);
	Task<bool> UpdatePasswordAsync(string userId, string resetToken, string newPassword);
	Task<bool> ChangePasswordAsync(string userId, string oldPassword, string newPassword);
	Task UpdateRefreshTokenAsync(AppUser user, string refreshToken, DateTimeOffset expiresAt);
	Task<bool> VerifyEmailAsync(string email, string verificationCode);
	Task<bool> ResendVerificationCodeAsync(string email);
	Task StoreVerificationCodeAsync(string email, string code, string token);
}
