using ClassifiedsApp.Application.Features.Queries.Common;

namespace ClassifiedsApp.Application.Features.Queries.Chats.GetChatRoom;

public class GetChatRoomQuery : GetDataByIdQuery<GetChatRoomQueryResponse>
{
	public Guid ChatRoomId { get; set; }
}
