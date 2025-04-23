using ClassifiedsApp.Application.Common.Results;
using MediatR;

namespace ClassifiedsApp.Application.Features.Queries.Chats.GetChatMessagesByChatRoom;

public class GetChatMessagesByChatRoomQuery : IRequest<Result<GetChatMessagesByChatRoomQueryResponse>>
{
	public Guid ChatRoomId { get; set; }
}
