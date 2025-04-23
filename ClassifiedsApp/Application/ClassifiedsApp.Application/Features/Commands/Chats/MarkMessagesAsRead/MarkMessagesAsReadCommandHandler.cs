using ClassifiedsApp.Application.Common.Consts;
using ClassifiedsApp.Application.Common.Results;
using ClassifiedsApp.Application.Interfaces.Repositories.Chats;
using ClassifiedsApp.Application.Interfaces.Services.SignalR;
using ClassifiedsApp.Application.Interfaces.Services.Users;
using MediatR;

namespace ClassifiedsApp.Application.Features.Commands.Chats.MarkMessagesAsRead;


public class MarkMessagesAsReadCommandHandler : IRequestHandler<MarkMessagesAsReadCommand, Result>
{
	readonly IChatMessageReadRepository _chatMessageReadRepository;
	readonly IChatMessageWriteRepository _chatMessageWriteRepository;
	readonly IChatRoomReadRepository _chatRoomReadRepository;
	readonly IChatHub _chatHub;
	readonly ICurrentUserService _currentUserService;

	public MarkMessagesAsReadCommandHandler(IChatMessageReadRepository chatMessageReadRepository,
											IChatMessageWriteRepository chatMessageWriteRepository,
											IChatRoomReadRepository chatRoomReadRepository,
											IChatHub chatHub,
											ICurrentUserService currentUserService)
	{
		_chatMessageReadRepository = chatMessageReadRepository;
		_chatMessageWriteRepository = chatMessageWriteRepository;
		_chatRoomReadRepository = chatRoomReadRepository;
		_chatHub = chatHub;
		_currentUserService = currentUserService;
	}

	public async Task<Result> Handle(MarkMessagesAsReadCommand request, CancellationToken cancellationToken)
	{
		try
		{
			var chatRoom = await _chatRoomReadRepository.GetByIdAsync(request.ChatRoomId, false)
						   ?? throw new KeyNotFoundException("Chat room not found.");

			if (chatRoom.BuyerId != _currentUserService.UserId!.Value && chatRoom.SellerId != _currentUserService.UserId.Value)
				throw new Exception("User is not part of this chat room");

			var unreadMessages = _chatMessageReadRepository.GetWhere(m => m.ReceiverId == _currentUserService.UserId!.Value
																	   && !m.IsRead
																	   && m.AdId == chatRoom.AdId);

			foreach (var message in unreadMessages)
			{
				message.IsRead = true;
				message.UpdatedAt = DateTimeOffset.UtcNow;
				_chatMessageWriteRepository.Update(message);
			}

			await _chatMessageWriteRepository.SaveAsync();

			// Notify sender that messages were read
			var otherUserId = _currentUserService.UserId!.Value == chatRoom.BuyerId ? chatRoom.SellerId : chatRoom.BuyerId;

			await _chatHub.SendMessageAsync(otherUserId.ToString(), SignalRMethodNames.MessagesReadName, chatRoom.Id);

			return Result.Success();
		}
		catch (Exception ex)
		{
			return Result.Failure($"Error occoured. {ex.Message}");
		}
	}
}
