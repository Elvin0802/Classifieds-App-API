using Azure.Identity;
using ClassifiedsApp.Application.Common.Results;
using ClassifiedsApp.Application.Dtos.Auth.Token;
using ClassifiedsApp.Application.Interfaces.Services.Auth;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ClassifiedsApp.Application.Features.Commands.Auth.RefreshTokenLogin;

public class RefreshTokenLoginCommandHandler : IRequestHandler<RefreshTokenLoginCommand, Result<AuthTokenDto>>
{
	readonly IAuthService _authService;

	public RefreshTokenLoginCommandHandler(IAuthService authService)
	{
		_authService = authService;
	}

	public async Task<Result<AuthTokenDto>> Handle(RefreshTokenLoginCommand request, CancellationToken cancellationToken)
	{
		try
		{
			var refreshToken = request.Request.Cookies["refreshToken"];

			if (string.IsNullOrEmpty(refreshToken))
				throw new UnauthorizedAccessException("Refresh token not found.");

			var tokenDto = await _authService.RefreshTokenLoginAsync(refreshToken)
							?? throw new AuthenticationFailedException("Token not created , refresh token login failed.");

			request.Response.Headers.Append("Authorization", $"Bearer {tokenDto.AccessToken}");

			request.Response.Cookies.Append("refreshToken", tokenDto.RefreshToken!, new CookieOptions
			{
				HttpOnly = true,
				Secure = false,
				SameSite = SameSiteMode.Lax,
				Expires = tokenDto.RefreshTokenExpiresAt,
			});

			return Result.Success(tokenDto, "Refresh Token Login successfull.");
		}
		catch (Exception ex)
		{
			return Result.Failure<AuthTokenDto>($"Error occoured. {ex.Message}");
		}
	}

}
