using System.Text.Json.Serialization;

namespace Api.Models;

public class Member
{
    public enum MemberRoles
    {
        Admin,
        Member
    }
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? UserId { get; set; }
    [JsonIgnore]
    public User? User { get; set; }
    public Guid? ServerId { get; set; }
    [JsonIgnore]
    public Server Server { get; set; } = null!;

    public MemberRoles MemberRole { get; set; } = MemberRoles.Member;

    [JsonIgnore]
    public IEnumerable<Message>? Messages { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
