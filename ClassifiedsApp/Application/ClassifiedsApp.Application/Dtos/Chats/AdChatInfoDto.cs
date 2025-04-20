namespace ClassifiedsApp.Application.Dtos.Chats;

public class AdChatInfoDto
{
	public Guid Id { get; set; }
	public string Title { get; set; } = string.Empty;
	public string ImageUrl { get; set; } = string.Empty;
	public decimal Price { get; set; }
	public string SellerName { get; set; } = string.Empty;
}