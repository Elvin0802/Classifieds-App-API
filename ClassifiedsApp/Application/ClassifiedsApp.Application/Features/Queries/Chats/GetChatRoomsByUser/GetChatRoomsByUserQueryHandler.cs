using ClassifiedsApp.Application.Common.Results;
using ClassifiedsApp.Application.Dtos.Chats;
using ClassifiedsApp.Application.Interfaces.Repositories.Chats;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClassifiedsApp.Application.Features.Queries.Chats.GetChatRoomsByUser;

public class GetChatRoomsByUserQueryHandler : IRequestHandler<GetChatRoomsByUserQuery, Result<GetChatRoomsByUserQueryResponse>>
{
	readonly IChatRoomReadRepository _chatRoomReadRepository;
	readonly IChatMessageReadRepository _сhatMessageReadRepository;

	public GetChatRoomsByUserQueryHandler(IChatRoomReadRepository chatRoomReadRepository,
											IChatMessageReadRepository сhatMessageReadRepository)
	{
		_chatRoomReadRepository = chatRoomReadRepository;
		_сhatMessageReadRepository = сhatMessageReadRepository;
	}

	public async Task<Result<GetChatRoomsByUserQueryResponse>> Handle(GetChatRoomsByUserQuery request, CancellationToken cancellationToken)
	{
		try
		{
			var chatRooms = _chatRoomReadRepository.Table
							.Where(x => x.BuyerId == request.UserId || x.SellerId == request.UserId)
							.Include(x => x.Buyer)
							.Include(x => x.Seller)
							.Include(x => x.Ad)
							.Include(x => x.Ad.Images);

			var chatRoomDtos = new List<ChatRoomDto>();

			foreach (var chatRoom in chatRooms)
			{
				var unreadCount = await _сhatMessageReadRepository.Table
									.CountAsync(m => m.ReceiverId == request.UserId
												  && !m.IsRead
												  && m.AdId == chatRoom.AdId,
												  cancellationToken: cancellationToken);

				var lastMessage = _сhatMessageReadRepository
									.GetWhere(m => m.AdId == chatRoom.AdId
												&& (m.SenderId == chatRoom.BuyerId || m.SenderId == chatRoom.SellerId)
												&& (m.ReceiverId == chatRoom.BuyerId || m.ReceiverId == chatRoom.SellerId))
									.ToList();

				var lastMessageAt = lastMessage.Count != 0 ? lastMessage.Max(m => m.CreatedAt) : chatRoom.CreatedAt;

				var mainImage = chatRoom.Ad.Images.FirstOrDefault(i => i.SortOrder == 0);
				string imageUrl = mainImage?.Url ?? "";

				chatRoomDtos.Add(new ChatRoomDto
				{
					Id = chatRoom.Id,
					BuyerId = chatRoom.BuyerId,
					BuyerName = chatRoom.Buyer.Name,
					SellerId = chatRoom.SellerId,
					SellerName = chatRoom.Seller.Name,
					AdId = chatRoom.AdId,
					AdTitle = chatRoom.Ad.Title,
					AdImageUrl = imageUrl,
					AdPrice = chatRoom.Ad.Price,
					LastMessageAt = lastMessageAt,
					UnreadCount = unreadCount
				});
			}

			// Sort by last message time
			chatRoomDtos = chatRoomDtos.OrderByDescending(c => c.LastMessageAt).ToList();

			var data = new GetChatRoomsByUserQueryResponse()
			{
				Items = chatRoomDtos,
				PageNumber = 1,
				PageSize = chatRoomDtos.Count,
				TotalCount = chatRoomDtos.Count
			};

			return Result.Success(data, "Chat rooms retrieved successfully.");
		}
		catch (Exception ex)
		{
			return Result.Failure<GetChatRoomsByUserQueryResponse>($"Failed to retrieve chat rooms : {ex.Message}");
		}
	}
}
