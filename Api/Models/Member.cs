using System.Text.Json.Serialization;

namespace Api.Models;

public class Member
{
    public enum MemberRoles
    {
        Admin,
        Member,
        Moderator,
    }
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Username { get; set; }
    public string? ImageUrl { get; set; } = "https://via.placeholder.com/160x160";
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public MemberRoles MemberRole { get; set; } = MemberRoles.Member;

    public string? UserId { get; set; }
    [JsonIgnore]
    public User? User { get; set; }

    public Guid? ServerId { get; set; }
    [JsonIgnore]
    public Server? Server { get; set; } = null!;

    [JsonIgnore]
    public List<Message>? Messages { get; set; }

}
