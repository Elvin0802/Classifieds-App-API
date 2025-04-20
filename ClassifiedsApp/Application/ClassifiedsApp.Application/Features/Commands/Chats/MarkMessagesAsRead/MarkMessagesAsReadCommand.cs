using ClassifiedsApp.Application.Common.Results;
using MediatR;

namespace ClassifiedsApp.Application.Features.Commands.Chats.MarkMessagesAsRead;

public class MarkMessagesAsReadCommand : IRequest<Result>
{
	public Guid ChatRoomId { get; set; }
	public Guid UserId { get; set; }
}
