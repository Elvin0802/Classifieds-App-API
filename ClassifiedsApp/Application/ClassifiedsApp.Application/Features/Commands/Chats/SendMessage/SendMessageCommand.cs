using ClassifiedsApp.Application.Common.Results;
using ClassifiedsApp.Application.Dtos.Chats;
using MediatR;

namespace ClassifiedsApp.Application.Features.Commands.Chats.SendMessage;

public class SendMessageCommand : IRequest<Result<ChatMessageDto>>
{
	public Guid ChatRoomId { get; set; }
	public string Content { get; set; } = string.Empty;
}
