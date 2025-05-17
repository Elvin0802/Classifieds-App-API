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
	readonly IHttpContextAccessor _contextAccessor;

	public RefreshTokenLoginCommandHandler(IAuthService authService, IHttpContextAccessor contextAccessor)
	{
		_authService = authService;
		_contextAccessor = contextAccessor;
	}

	public async Task<Result<AuthTokenDto>> Handle(RefreshTokenLoginCommand request, CancellationToken cancellationToken)
	{
		try
		{
			var refreshToken = _contextAccessor.HttpContext?.Request.Cookies["refreshToken"];

			if (string.IsNullOrEmpty(refreshToken))
				throw new UnauthorizedAccessException("Refresh token not found.");

			var tokenDto = await _authService.RefreshTokenLoginAsync(refreshToken)
							?? throw new AuthenticationFailedException("Token not created , refresh token login failed.");

			_contextAccessor.HttpContext?.Response.Headers.Append("Authorization", $"Bearer {tokenDto.AccessToken}");

			_contextAccessor.HttpContext?.Response.Cookies.Append("refreshToken", tokenDto.RefreshToken!, new CookieOptions
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
