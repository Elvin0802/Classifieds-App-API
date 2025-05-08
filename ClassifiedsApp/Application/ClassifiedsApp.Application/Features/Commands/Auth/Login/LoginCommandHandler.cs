using Azure;
using Azure.Identity;
using ClassifiedsApp.Application.Common.Results;
using ClassifiedsApp.Application.Dtos.Auth.Token;
using ClassifiedsApp.Application.Interfaces.Services.Auth;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ClassifiedsApp.Application.Features.Commands.Auth.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthTokenDto>>
{
	readonly IAuthService _authService;

	public LoginCommandHandler(IAuthService authService)
	{
		_authService = authService;
	}

	public async Task<Result<AuthTokenDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
	{
		try
		{
			var tokenDto = await _authService.LoginAsync(request.Email, request.Password)
							?? throw new AuthenticationFailedException("Token not created , login failed.");

			request.Response.Headers.Append("Authorization", $"Bearer {tokenDto.AccessToken}");

			request.Response.Cookies.Append("refreshToken", tokenDto.RefreshToken!, new CookieOptions
			{
				HttpOnly = true,
				Secure = false,
				SameSite = SameSiteMode.Lax,
				Expires = tokenDto.RefreshTokenExpiresAt,
			});

			return Result.Success(tokenDto, "Login successful.");
		}
		catch (Exception ex)
		{
			return Result.Failure<AuthTokenDto>($"Error occoured. {ex.Message}");
		}
	}
}
