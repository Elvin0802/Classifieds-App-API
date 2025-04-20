using ClassifiedsApp.Application.Common.Results;
using ClassifiedsApp.Application.Dtos.Chats;
using ClassifiedsApp.Application.Interfaces.Repositories.Ads;
using MediatR;

namespace ClassifiedsApp.Application.Features.Queries.Chats.GetAdChatInfo;

public class GetAdChatInfoQueryHandler : IRequestHandler<GetAdChatInfoQuery, Result<GetAdChatInfoQueryResponse>>
{
	private readonly IAdReadRepository _adReadRepository;

	public GetAdChatInfoQueryHandler(IAdReadRepository adReadRepository)
	{
		_adReadRepository = adReadRepository;
	}

	public async Task<Result<GetAdChatInfoQueryResponse>> Handle(GetAdChatInfoQuery request, CancellationToken cancellationToken)
	{
		try
		{
			var ad = await _adReadRepository.GetAdByIdWithIncludesAsync(request.Id)
					 ?? throw new KeyNotFoundException("Ad not found");

			var mainImage = ad.Images.FirstOrDefault(i => i.SortOrder == 0);
			string imageUrl = mainImage?.Url ?? "";

			var data = new GetAdChatInfoQueryResponse()
			{
				Item = new AdChatInfoDto
				{
					Id = ad.Id,
					Title = ad.Title,
					ImageUrl = imageUrl,
					Price = ad.Price,
					SellerName = ad.AppUser.Name
				}
			};

			return Result.Success(data, "Ad chat info retrieved successfully.");
		}
		catch (Exception ex)
		{
			return Result.Failure<GetAdChatInfoQueryResponse>($"Failed to retrieve ad chat info: {ex.Message}");
		}
	}
}
