using ClassifiedsApp.Core.Entities.Common;

namespace ClassifiedsApp.Core.Entities;

public class ChatRoom : BaseEntity
{
	public Guid BuyerId { get; set; }
	public AppUser Buyer { get; set; } = null!;
	public Guid SellerId { get; set; }
	public AppUser Seller { get; set; } = null!;
	public Guid AdId { get; set; }
	public Ad Ad { get; set; } = null!;
	public IList<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}