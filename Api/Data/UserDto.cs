namespace Api.Data;

public class UserDto
{
    public required string Id { get; set; }

    public string? Username { get; set; }

    public string? ImageUrl { get; set; }
}
