using ClassifiedsApp.Application.Common.Results;
using ClassifiedsApp.Application.Dtos.Chats;
using MediatR;

namespace ClassifiedsApp.Application.Features.Commands.Chats.CreateChatRoom;

public class CreateChatRoomCommand : IRequest<Result<ChatRoomDto>>
{
	public Guid BuyerId { get; set; }
	public Guid AdId { get; set; }
}
