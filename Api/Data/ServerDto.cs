namespace Api.Data;

public class ServerDto
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? ImageUrl { get; set; }
    public string? OwnerId { get; set; }
    public List<MemberDto>? Members { get; set; }
    public List<ChannelDto>? Channels { get; set; }
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
}
