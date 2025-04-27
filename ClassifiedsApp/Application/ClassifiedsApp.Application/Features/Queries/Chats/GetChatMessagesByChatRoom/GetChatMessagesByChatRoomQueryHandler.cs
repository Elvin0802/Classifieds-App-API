using ClassifiedsApp.Application.Common.Results;
using ClassifiedsApp.Application.Dtos.Chats;
using ClassifiedsApp.Application.Interfaces.Repositories.Chats;
using ClassifiedsApp.Application.Interfaces.Services.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClassifiedsApp.Application.Features.Queries.Chats.GetChatMessagesByChatRoom;

public class GetChatMessagesByChatRoomQueryHandler : IRequestHandler<GetChatMessagesByChatRoomQuery, Result<GetChatMessagesByChatRoomQueryResponse>>
{
	readonly IChatRoomReadRepository _chatRoomReadRepository;
	readonly IChatMessageReadRepository _chatMessageReadRepository;
	readonly ICurrentUserService _currentUserService;


	public GetChatMessagesByChatRoomQueryHandler(IChatRoomReadRepository chatRoomReadRepository,
												 IChatMessageReadRepository chatMessageReadRepository,
												 ICurrentUserService currentUserService)
	{
		_chatRoomReadRepository = chatRoomReadRepository;
		_chatMessageReadRepository = chatMessageReadRepository;
		_currentUserService = currentUserService;
	}

	public async Task<Result<GetChatMessagesByChatRoomQueryResponse>> Handle(GetChatMessagesByChatRoomQuery request, CancellationToken cancellationToken)
	{
		try
		{
			var chatRoom = await _chatRoomReadRepository.GetByIdAsync(request.ChatRoomId)
							?? throw new KeyNotFoundException("Chat room not found.");

			if (chatRoom.BuyerId != _currentUserService.UserId!.Value && chatRoom.SellerId != _currentUserService.UserId.Value)
				throw new Exception("User is not part of this chat room.");

			var messages = _chatMessageReadRepository.Table
							.Where(m => m.AdId == chatRoom.AdId
									 && (m.SenderId == chatRoom.BuyerId || m.SenderId == chatRoom.SellerId)
									 && (m.ReceiverId == chatRoom.BuyerId || m.ReceiverId == chatRoom.SellerId))
							.Include(m => m.Sender)
							.Include(m => m.Receiver)
							.ToList();

			// Sort by creation time
			messages = messages.OrderBy(m => m.CreatedAt).ToList();

			var messageDtos = new List<ChatMessageDto>();

			foreach (var message in messages)
			{
				messageDtos.Add(new ChatMessageDto
				{
					Id = message.Id,
					Content = message.Content,
					SenderId = message.SenderId,
					ReceiverId = message.ReceiverId,
					ChatRoomId = chatRoom.Id,
					SenderName = message.Sender.Name,
					CreatedAt = message.CreatedAt,
					IsRead = message.IsRead
				});
			}

			var data = new GetChatMessagesByChatRoomQueryResponse()
			{
				Items = messageDtos,
				PageNumber = 1,
				PageSize = messageDtos.Count,
				TotalCount = messageDtos.Count
			};

			return Result.Success(data, "Chat Messages retrieved successfully.");
		}
		catch (Exception ex)
		{
			return Result.Failure<GetChatMessagesByChatRoomQueryResponse>($"Failed to retrieve Chat Messages : {ex.Message}");
		}
	}
}
