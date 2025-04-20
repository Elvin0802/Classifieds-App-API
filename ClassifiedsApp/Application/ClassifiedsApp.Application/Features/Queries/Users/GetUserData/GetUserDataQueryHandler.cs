using AutoMapper;
using ClassifiedsApp.Application.Common.Results;
using ClassifiedsApp.Application.Dtos.Auth.Users;
using ClassifiedsApp.Application.Interfaces.Services.Users;
using ClassifiedsApp.Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ClassifiedsApp.Application.Features.Queries.Users.GetUserData;

public class GetUserDataQueryHandler : IRequestHandler<GetUserDataQuery, Result<GetUserDataQueryResponse>>
{
	readonly UserManager<AppUser> _userManager;
	readonly IMapper _mapper;
	readonly ICurrentUserService _currentUserService;

	public GetUserDataQueryHandler(UserManager<AppUser> userManager, IMapper mapper, ICurrentUserService currentUserService)
	{
		_userManager = userManager;
		_mapper = mapper;
		_currentUserService = currentUserService;
	}

	public async Task<Result<GetUserDataQueryResponse>> Handle(GetUserDataQuery request, CancellationToken cancellationToken)
	{
		try
		{
			var user = await _userManager.FindByIdAsync(_currentUserService.UserId.ToString()!)
						?? throw new KeyNotFoundException("User not found.");

			var userDto = _mapper.Map<AppUserDto>(user);

			userDto.IsAdmin = await _userManager.IsInRoleAsync(user!, "Admin");

			return Result.Success(new GetUserDataQueryResponse() { Item = userDto }, "User Data retrieved successfully.");
		}
		catch (Exception ex)
		{
			return Result.Failure<GetUserDataQueryResponse>($"Failed to retrieve user: {ex.Message}");
		}
	}
}
