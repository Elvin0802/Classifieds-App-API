using ClassifiedsApp.Application.Common.Results;
using ClassifiedsApp.Application.Interfaces.Services.BlackList;
using MediatR;

namespace ClassifiedsApp.Application.Features.Commands.Users.RemoveUserFromBlackList;

public class RemoveUserFromBlackListCommandHandler : IRequestHandler<RemoveUserFromBlackListCommand, Result>
{
	readonly IBlacklistService _blacklistService;

	public RemoveUserFromBlackListCommandHandler(IBlacklistService blacklistService)
	{
		_blacklistService = blacklistService;
	}

	public async Task<Result> Handle(RemoveUserFromBlackListCommand request, CancellationToken cancellationToken)
	{
		try
		{
			await _blacklistService.UnblacklistUserAsync(request.Email);

			return Result.Success("User successfully removed from black list.");
		}
		catch (Exception ex)
		{
			return Result.Failure<string>($"Error occurred: {ex.Message}");
		}
	}
}
