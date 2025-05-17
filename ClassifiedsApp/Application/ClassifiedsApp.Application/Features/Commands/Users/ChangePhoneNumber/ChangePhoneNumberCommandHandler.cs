using ClassifiedsApp.Application.Common.Results;
using ClassifiedsApp.Application.Features.Commands.Users.ChangeName;
using ClassifiedsApp.Application.Interfaces.Services.Users;
using ClassifiedsApp.Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassifiedsApp.Application.Features.Commands.Users.ChangePhoneNumber;

public class ChangePhoneNumberCommandHandler : IRequestHandler<ChangePhoneNumberCommand, Result>
{
	readonly UserManager<AppUser> _userManager;
	readonly ICurrentUserService _currentUserService;

	public ChangePhoneNumberCommandHandler(UserManager<AppUser> userManager, ICurrentUserService currentUserService)
	{
		_userManager = userManager;
		_currentUserService = currentUserService;
	}

	public async Task<Result> Handle(ChangePhoneNumberCommand request, CancellationToken cancellationToken)
	{
		try
		{
			var exisitingUser = await _userManager.FindByEmailAsync(_currentUserService.Email!)
								?? throw new KeyNotFoundException("User Not Found.");

			if (!(await _userManager.SetPhoneNumberAsync(exisitingUser, request.PhoneNumber)).Succeeded)
				throw new Exception("Name not changed.");

			return Result.Success("Name changed.");
		}
		catch (Exception ex)
		{
			return Result.Failure($"Error occoured. {ex.Message}");
		}
	}
}
