using ClassifiedsApp.Core.Entities.Common;

namespace ClassifiedsApp.Core.Entities;

public class ChatMessage : BaseEntity
{
	public string Content { get; set; } = string.Empty;
	public Guid SenderId { get; set; }
	public AppUser Sender { get; set; } = null!;
	public Guid ReceiverId { get; set; }
	public AppUser Receiver { get; set; } = null!;
	public Guid AdId { get; set; }
	public Ad Ad { get; set; } = null!;
	public bool IsRead { get; set; } = false;
}