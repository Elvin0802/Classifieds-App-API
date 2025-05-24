namespace ClassifiedsApp.Application.Dtos.Auth.Users;

public class BlacklistUserDto
{
	public Guid UserId { get; set; }
	public string Email { get; set; }
	public string Reason { get; set; }
	public DateTimeOffset BlacklistedAt { get; set; }
}
