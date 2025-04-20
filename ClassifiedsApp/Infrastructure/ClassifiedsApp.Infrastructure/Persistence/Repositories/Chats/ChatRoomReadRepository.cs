using ClassifiedsApp.Application.Interfaces.Repositories.Chats;
using ClassifiedsApp.Core.Entities;
using ClassifiedsApp.Infrastructure.Persistence.Context;
using ClassifiedsApp.Infrastructure.Persistence.Repositories.Common;

namespace ClassifiedsApp.Infrastructure.Persistence.Repositories.Chats;

public class ChatRoomReadRepository : ReadRepository<ChatRoom>, IChatRoomReadRepository
{
	public ChatRoomReadRepository(ApplicationDbContext context) : base(context) { }
}
