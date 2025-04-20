namespace ClassifiedsApp.Application.Dtos.Chats;

public class ChatRoomDto
{
	public Guid Id { get; set; }
	public Guid BuyerId { get; set; }
	public string BuyerName { get; set; } = string.Empty;
	public Guid SellerId { get; set; }
	public string SellerName { get; set; } = string.Empty;
	public Guid AdId { get; set; }
	public string AdTitle { get; set; } = string.Empty;
	public string AdImageUrl { get; set; } = string.Empty;
	public decimal AdPrice { get; set; }
	public DateTimeOffset LastMessageAt { get; set; }
	public int UnreadCount { get; set; }
}