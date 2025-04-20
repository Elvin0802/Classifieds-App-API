using ClassifiedsApp.Application.Common.Results;
using ClassifiedsApp.Application.Interfaces.Repositories.Chats;
using ClassifiedsApp.Application.Interfaces.Services.SignalR;
using MediatR;

namespace ClassifiedsApp.Application.Features.Commands.Chats.MarkMessagesAsRead;


public class MarkMessagesAsReadCommandHandler : IRequestHandler<MarkMessagesAsReadCommand, Result>
{
	readonly IChatMessageReadRepository _chatMessageReadRepository;
	readonly IChatMessageWriteRepository _chatMessageWriteRepository;
	readonly IChatRoomReadRepository _chatRoomReadRepository;
	private readonly IChatHub _chatHub;

	public MarkMessagesAsReadCommandHandler(IChatMessageReadRepository chatMessageReadRepository,
											IChatMessageWriteRepository chatMessageWriteRepository,
											IChatRoomReadRepository chatRoomReadRepository,
											IChatHub chatHub)
	{
		_chatMessageReadRepository = chatMessageReadRepository;
		_chatMessageWriteRepository = chatMessageWriteRepository;
		_chatRoomReadRepository = chatRoomReadRepository;
		_chatHub = chatHub;
	}

	public async Task<Result> Handle(MarkMessagesAsReadCommand request, CancellationToken cancellationToken)
	{
		try
		{
			var chatRoom = await _chatRoomReadRepository.GetByIdAsync(request.ChatRoomId, false)
						   ?? throw new KeyNotFoundException("Chat room not found.");

			if (chatRoom.BuyerId != request.UserId && chatRoom.SellerId != request.UserId)
				throw new Exception("User is not part of this chat room");

			var unreadMessages = _chatMessageReadRepository.GetWhere(m => m.ReceiverId == request.UserId
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
			var otherUserId = request.UserId == chatRoom.BuyerId ? chatRoom.SellerId : chatRoom.BuyerId;

			await _chatHub.SendMessageAsync(otherUserId.ToString(), "MessagesRead", chatRoom.Id);

			return Result.Success();
		}
		catch (Exception ex)
		{
			return Result.Failure($"Error occoured. {ex.Message}");
		}
	}
}
