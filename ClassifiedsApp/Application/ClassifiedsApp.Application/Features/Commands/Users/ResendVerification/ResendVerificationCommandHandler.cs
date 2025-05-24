using ClassifiedsApp.Application.Common.Results;
using ClassifiedsApp.Application.Interfaces.Services.Users;
using MediatR;

namespace ClassifiedsApp.Application.Features.Commands.Users.ResendVerification;

public class ResendVerificationCommandHandler : IRequestHandler<ResendVerificationCommand, Result>
{
	readonly IUserService _userService;

	public ResendVerificationCommandHandler(IUserService userService)
	{
		_userService = userService;
	}

	public async Task<Result> Handle(ResendVerificationCommand request, CancellationToken cancellationToken)
	{
		try
		{
			if (!await _userService.ResendVerificationCodeAsync(request.Email))
				throw new Exception("Verification Code Not Sended.");

			return Result.Success("Verification code sended successfully.");
		}
		catch (Exception ex)
		{
			return Result.Failure($"Error occoured. {ex.Message}");
		}
	}
}
