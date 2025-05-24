using ClassifiedsApp.Application.Common.Results;
using ClassifiedsApp.Application.Interfaces.Services.BlackList;
using MediatR;

namespace ClassifiedsApp.Application.Features.Queries.Users.GetUsersFromBlackList;

public class GetUsersFromBlackListQueryHandler : IRequestHandler<GetUsersFromBlackListQuery, Result<GetUsersFromBlackListQueryResponse>>
{
	private readonly IBlacklistService _blacklistService;

	public GetUsersFromBlackListQueryHandler(IBlacklistService blacklistService)
	{
		_blacklistService = blacklistService;
	}

	public async Task<Result<GetUsersFromBlackListQueryResponse>> Handle(GetUsersFromBlackListQuery request, CancellationToken cancellationToken)
	{
		try
		{
			var blacklistedUsers = (await _blacklistService.GetBlacklistedUsersAsync()).ToList();

			var data = new GetUsersFromBlackListQueryResponse
			{
				Items = blacklistedUsers,
				PageNumber = 1,
				PageSize = blacklistedUsers.Count,
				TotalCount = blacklistedUsers.Count
			};

			return Result.Success(data, "Blacklisted users retrieved successfully.");
		}
		catch (Exception ex)
		{
			return Result.Failure<GetUsersFromBlackListQueryResponse>($"Failed to retrieve blacklisted users: {ex.Message}");
		}

	}
}
