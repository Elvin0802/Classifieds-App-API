using ClassifiedsApp.Application.Common.Results;
using MediatR;

namespace ClassifiedsApp.Application.Features.Queries.Chats.GetChatRoomsByUser;

public class GetChatRoomsByUserQuery : IRequest<Result<GetChatRoomsByUserQueryResponse>>
{
}
