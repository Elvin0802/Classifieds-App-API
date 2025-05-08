using ClassifiedsApp.Application.Common.Results;
using ClassifiedsApp.Application.Interfaces.Services.Users;
using ClassifiedsApp.Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassifiedsApp.Application.Features.Commands.Users.ChangeName;

public class ChangeNameCommandHandler : IRequestHandler<ChangeNameCommand, Result>
{
	readonly UserManager<AppUser> _userManager;
	readonly ICurrentUserService _currentUserService;

	public ChangeNameCommandHandler(UserManager<AppUser> userManager, ICurrentUserService currentUserService)
	{
		_userManager = userManager;
		_currentUserService = currentUserService;
	}

	public async Task<Result> Handle(ChangeNameCommand request, CancellationToken cancellationToken)
	{
		try
		{
			var exisitingUser = await _userManager.FindByEmailAsync(_currentUserService.Email!)
								?? throw new KeyNotFoundException("User Not Found.");

			exisitingUser.Name = request.Name.Trim();

			if (!(await _userManager.UpdateAsync(exisitingUser)).Succeeded)
				throw new Exception("Name not changed.");

			return Result.Success("Name changed.");
		}
		catch (Exception ex)
		{
			return Result.Failure($"Error occoured. {ex.Message}");
		}
	}
}
