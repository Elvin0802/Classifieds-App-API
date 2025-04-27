using ClassifiedsApp.Application.Common.Consts;
using ClassifiedsApp.Application.Common.Results;
using ClassifiedsApp.Application.Dtos.Chats;
using ClassifiedsApp.Application.Interfaces.Repositories.Chats;
using ClassifiedsApp.Application.Interfaces.Services.SignalR;
using ClassifiedsApp.Application.Interfaces.Services.Users;
using ClassifiedsApp.Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ClassifiedsApp.Application.Features.Commands.Chats.SendMessage;

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, Result<ChatMessageDto>>
{
	readonly IChatRoomReadRepository _chatRoomReadRepository;
	readonly IChatMessageWriteRepository _chatMessageWriteRepository;
	readonly UserManager<AppUser> _userManager;
	readonly IChatHub _chatHub;
	readonly ICurrentUserService _currentUserService;

	public SendMessageCommandHandler(IChatRoomReadRepository chatRoomReadRepository,
									 IChatMessageWriteRepository chatMessageWriteRepository,
									 UserManager<AppUser> userManager,
									 IChatHub chatHub,
									 ICurrentUserService currentUserService)
	{
		_chatRoomReadRepository = chatRoomReadRepository;
		_chatMessageWriteRepository = chatMessageWriteRepository;
		_userManager = userManager;
		_chatHub = chatHub;
		_currentUserService = currentUserService;
	}

	public async Task<Result<ChatMessageDto>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
	{
		try
		{
			var chatRoom = await _chatRoomReadRepository.GetByIdAsync(request.ChatRoomId)
						   ?? throw new KeyNotFoundException("Chat room not found.");

			if (chatRoom.BuyerId != _currentUserService.UserId!.Value && chatRoom.SellerId != _currentUserService.UserId.Value)
				throw new Exception("User is not part of this chat room.");

			var receiverId = _currentUserService.UserId!.Value == chatRoom.BuyerId ? chatRoom.SellerId : chatRoom.BuyerId;

			var message = new ChatMessage()
			{
				Content = request.Content,
				SenderId = _currentUserService.UserId!.Value,
				ReceiverId = receiverId,
				AdId = chatRoom.AdId,
				ChatRoomId = chatRoom.Id,
			};

			await _chatMessageWriteRepository.AddAsync(message);
			await _chatMessageWriteRepository.SaveAsync();

			var sender = await _userManager.FindByIdAsync(_currentUserService.UserId!.Value.ToString())
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

			await _chatHub.SendMessageAsync(receiverId.ToString(), SignalRMethodNames.ReceiveMessageName, messageDto);

			return Result.Success(messageDto, "Send message successfully completed.");
		}
		catch (Exception ex)
		{
			return Result.Failure<ChatMessageDto>($"Error occoured. {ex.Message}");
		}
	}
}
