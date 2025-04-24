using Azure.Identity;
using ClassifiedsApp.Application.Common.Results;
using ClassifiedsApp.Application.Dtos.Auth.Token;
using ClassifiedsApp.Application.Interfaces.Services.Auth;
using MediatR;

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
			var tokenDto = await _authService.RefreshTokenLoginAsync(request.RefreshToken)
							?? throw new AuthenticationFailedException("Token not created , refresh token login failed.");

			return Result.Success(tokenDto, "Refresh Token Login successfull.");
		}
		catch (Exception ex)
		{
			return Result.Failure<AuthTokenDto>($"Error occoured. {ex.Message}");
		}
	}

}
