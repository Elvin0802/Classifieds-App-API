using ClassifiedsApp.Application.Interfaces.Repositories.Chats;
using ClassifiedsApp.Core.Entities;
using ClassifiedsApp.Infrastructure.Persistence.Context;
using ClassifiedsApp.Infrastructure.Persistence.Repositories.Common;

namespace ClassifiedsApp.Infrastructure.Persistence.Repositories.Chats;

public class ChatMessageReadRepository : ReadRepository<ChatMessage>, IChatMessageReadRepository
{
	public ChatMessageReadRepository(ApplicationDbContext context) : base(context) { }
}
