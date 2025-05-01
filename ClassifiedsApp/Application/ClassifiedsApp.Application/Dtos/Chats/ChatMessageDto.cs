namespace ClassifiedsApp.Application.Dtos.Chats;

public class ChatMessageDto
{
	public Guid Id { get; set; }
	public string Content { get; set; } = string.Empty;
	public Guid SenderId { get; set; }
	public string SenderName { get; set; } = string.Empty;
	public DateTimeOffset CreatedAt { get; set; }
	public Guid ChatRoomId { get; set; }
	public Guid ReceiverId { get; set; }
	public bool IsRead { get; set; }
}