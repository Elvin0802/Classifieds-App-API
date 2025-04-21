using ClassifiedsApp.Application.Common.Results;
using ClassifiedsApp.Application.Dtos.Chats;
using ClassifiedsApp.Application.Interfaces.Repositories.Chats;
using ClassifiedsApp.Application.Interfaces.Services.SignalR;
using ClassifiedsApp.Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ClassifiedsApp.Application.Features.Commands.Chats.SendMessage;

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, Result<ChatMessageDto>>
{
	readonly IChatRoomReadRepository _chatRoomReadRepository;
	readonly IChatMessageWriteRepository _chatMessageWriteRepository;
	readonly UserManager<AppUser> _userManager;
	private readonly IChatHub _chatHub;

	public SendMessageCommandHandler(IChatRoomReadRepository chatRoomReadRepository,
									 IChatMessageWriteRepository chatMessageWriteRepository,
									 UserManager<AppUser> userManager,
									 IChatHub chatHub)
	{
		_chatRoomReadRepository = chatRoomReadRepository;
		_chatMessageWriteRepository = chatMessageWriteRepository;
		_userManager = userManager;
		_chatHub = chatHub;
	}

	public async Task<Result<ChatMessageDto>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
	{
		try
		{
			var chatRoom = await _chatRoomReadRepository.GetByIdAsync(request.ChatRoomId)
						   ?? throw new KeyNotFoundException("Chat room not found.");

			if (chatRoom.BuyerId != request.SenderId && chatRoom.SellerId != request.SenderId)
				throw new Exception("User is not part of this chat room.");

			var receiverId = request.SenderId == chatRoom.BuyerId ? chatRoom.SellerId : chatRoom.BuyerId;

			var message = new ChatMessage()
			{
				Content = request.Content,
				SenderId = request.SenderId,
				ReceiverId = receiverId,
				AdId = chatRoom.AdId
			};

			await _chatMessageWriteRepository.AddAsync(message);
			await _chatMessageWriteRepository.SaveAsync();

			var sender = await _userManager.FindByIdAsync(request.SenderId.ToString())
						 ?? throw new KeyNotFoundException("User not found.");

			var messageDto = new ChatMessageDto
			{
				Id = message.Id,
				Content = message.Content,
				SenderId = message.SenderId,
				SenderName = sender.Name,
				CreatedAt = message.CreatedAt,
				IsRead = message.IsRead
			};

			await _chatHub.SendMessageAsync(receiverId.ToString(), "ReceiveMessage", messageDto);
			// await _chatHub.SendMessageAsync(receiverId.ToString(), SignalRMethodNames, messageDto);

			return Result.Success(messageDto, "Send message successfully completed.");
		}
		catch (Exception ex)
		{
			return Result.Failure<ChatMessageDto>($"Error occoured. {ex.Message}");
		}
	}
}
