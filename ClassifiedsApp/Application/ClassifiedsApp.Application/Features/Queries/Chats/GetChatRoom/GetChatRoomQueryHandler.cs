using ClassifiedsApp.Application.Common.Results;
using ClassifiedsApp.Application.Dtos.Chats;
using ClassifiedsApp.Application.Interfaces.Repositories.Chats;
using ClassifiedsApp.Application.Interfaces.Services.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClassifiedsApp.Application.Features.Queries.Chats.GetChatRoom;


public class GetChatRoomQueryHandler : IRequestHandler<GetChatRoomQuery, Result<GetChatRoomQueryResponse>>
{

	readonly IChatRoomReadRepository _chatRoomReadRepository;
	readonly IChatMessageReadRepository _сhatMessageReadRepository;
	readonly ICurrentUserService _currentUserService;

	public GetChatRoomQueryHandler(IChatRoomReadRepository chatRoomReadRepository,
								   IChatMessageReadRepository сhatMessageReadRepository,
								   ICurrentUserService currentUserService)
	{
		_chatRoomReadRepository = chatRoomReadRepository;
		_сhatMessageReadRepository = сhatMessageReadRepository;
		_currentUserService = currentUserService;
	}

	public async Task<Result<GetChatRoomQueryResponse>> Handle(GetChatRoomQuery request, CancellationToken cancellationToken)
	{
		try
		{
			var chatRoom = await _chatRoomReadRepository.Table.Where(cr => cr.Id == request.Id)
									.Include(cr => cr.Ad)
										.ThenInclude(ad => ad.Images)
									.Include(cr => cr.Seller)
									.Include(cr => cr.Buyer)
									.Include(cr => cr.Messages)
									.FirstOrDefaultAsync(cancellationToken: cancellationToken)
						   ?? throw new KeyNotFoundException("Chat room not found.");

			if (chatRoom.BuyerId != _currentUserService.UserId && chatRoom.SellerId != _currentUserService.UserId)
				throw new Exception("User is not part of this chat room.");

			var unreadCount = await _сhatMessageReadRepository.Table.CountAsync(m => m.ReceiverId == _currentUserService.UserId
																				  && !m.IsRead
																				  && m.AdId == chatRoom.AdId,
																				  cancellationToken: cancellationToken);

			var lastMessage = _сhatMessageReadRepository.GetWhere(m => m.AdId == chatRoom.AdId &&
						(m.SenderId == chatRoom.BuyerId || m.SenderId == chatRoom.SellerId) &&
						(m.ReceiverId == chatRoom.BuyerId || m.ReceiverId == chatRoom.SellerId))
					.ToList();

			var lastMessageAt = lastMessage.Count != 0 ? lastMessage.Max(m => m.CreatedAt) : chatRoom.CreatedAt;

			var mainImage = chatRoom.Ad.Images.FirstOrDefault(i => i.SortOrder == 0);
			string imageUrl = mainImage?.Url ?? "";

			var data = new GetChatRoomQueryResponse()
			{
				Item = new ChatRoomDto
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
				}
			};

			return Result.Success(data, "Chat room retrieved successfully.");
		}
		catch (Exception ex)
		{
			return Result.Failure<GetChatRoomQueryResponse>($"Failed to retrieve chat room: {ex.Message}");
		}
	}
}
