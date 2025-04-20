//using ClassifiedsApp.Application.Common.Results;
//using ClassifiedsApp.Application.Dtos.Chats;
//using ClassifiedsApp.Core.Entities;
//using MediatR;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ClassifiedsApp.Application.Features.Queries.Chats.GetChatMessagesByChatRoom;

//public class GetChatMessagesByChatRoomQueryHandler : IRequestHandler<GetChatMessagesByChatRoomQuery, Result<List<ChatMessageDto>>>
//{
//	private readonly IRepository<ChatRoom> _chatRoomRepository;
//	private readonly IRepository<ChatMessage> _chatMessageRepository;
//	private readonly IRepository<AppUser> _userRepository;

//	public GetChatMessagesByChatRoomQueryHandler(
//		IRepository<ChatRoom> chatRoomRepository,
//		IRepository<ChatMessage> chatMessageRepository,
//		IRepository<AppUser> userRepository)
//	{
//		_chatRoomRepository = chatRoomRepository;
//		_chatMessageRepository = chatMessageRepository;
//		_userRepository = userRepository;
//	}

//	public async Task<Result<List<ChatMessageDto>>> Handle(GetChatMessagesByChatRoomQuery request, CancellationToken cancellationToken)
//	{
//		var chatRoom = await _chatRoomRepository.GetByIdAsync(request.ChatRoomId);
//		if (chatRoom == null)
//			return Result<List<ChatMessageDto>>.Failure("Chat room not found");

//		if (chatRoom.BuyerId != request.UserId && chatRoom.SellerId != request.UserId)
//			return Result<List<ChatMessageDto>>.Failure("User is not part of this chat room");

//		var messages = await _chatMessageRepository.ListWithIncludeAsync(
//			m => m.AdId == chatRoom.AdId &&
//				(m.SenderId == chatRoom.BuyerId || m.SenderId == chatRoom.SellerId) &&
//				(m.ReceiverId == chatRoom.BuyerId || m.ReceiverId == chatRoom.SellerId),
//			m => m.Sender);

//		// Sort by creation time
//		messages = messages.OrderBy(m => m.CreatedAt).ToList();

//		var messageDtos = new List<ChatMessageDto>();

//		foreach (var message in messages)
//		{
//			messageDtos.Add(new ChatMessageDto
//			{
//				Id = message.Id,
//				Content = message.Content,
//				SenderId = message.SenderId,
//				SenderName = message.Sender.Name,
//				CreatedAt = message.CreatedAt,
//				IsRead = message.IsRead
//			});
//		}

//		return Result<List<ChatMessageDto>>.Success(messageDtos);
//	}
//}
