using ClassifiedsApp.Application.Common.Results;
using ClassifiedsApp.Application.Interfaces.Services.Users;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassifiedsApp.Application.Features.Commands.Users.VerifyEmail;

public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, Result>
{
	readonly IUserService _userService;

	public VerifyEmailCommandHandler(IUserService userService)
	{
		_userService = userService;
	}

	public async Task<Result> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
	{
		try
		{
			if (!await _userService.VerifyEmailAsync(request.Email, request.Code))
				throw new Exception("Email Not Confirmed.");

			return Result.Success("Email confirmed successfully.");
		}
		catch (Exception ex)
		{
			return Result.Failure($"Error occoured. {ex.Message}");
		}

	}
}
