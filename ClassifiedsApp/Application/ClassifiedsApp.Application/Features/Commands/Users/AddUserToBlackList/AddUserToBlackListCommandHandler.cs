using ClassifiedsApp.Application.Common.Results;
using ClassifiedsApp.Application.Interfaces.Services.BlackList;
using MediatR;

namespace ClassifiedsApp.Application.Features.Commands.Users.AddUserToBlackList;

public class AddUserToBlackListCommandHandler : IRequestHandler<AddUserToBlackListCommand, Result>
{
	private readonly IBlacklistService _blacklistService;

	public AddUserToBlackListCommandHandler(IBlacklistService blacklistService)
	{
		_blacklistService = blacklistService;
	}

	public async Task<Result> Handle(AddUserToBlackListCommand request, CancellationToken cancellationToken)
	{
		try
		{
			await _blacklistService.BlacklistUserAsync(request.Email, request.Reason);

			return Result.Success("User successfully added to the black list.");
		}
		catch (Exception ex)
		{
			return Result.Failure<string>($"Error occurred: {ex.Message}");
		}
	}

}
