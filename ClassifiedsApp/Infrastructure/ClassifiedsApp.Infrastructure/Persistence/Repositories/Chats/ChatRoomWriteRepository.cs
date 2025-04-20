using ClassifiedsApp.Application.Interfaces.Repositories.Chats;
using ClassifiedsApp.Core.Entities;
using ClassifiedsApp.Infrastructure.Persistence.Context;
using ClassifiedsApp.Infrastructure.Persistence.Repositories.Common;

namespace ClassifiedsApp.Infrastructure.Persistence.Repositories.Chats;

public class ChatRoomWriteRepository : WriteRepository<ChatRoom>, IChatRoomWriteRepository
{
	public ChatRoomWriteRepository(ApplicationDbContext context) : base(context) { }
}
