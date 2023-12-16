using System.Text.Json.Serialization;

namespace Api.Models;

public class Member
{
    public enum MemberRoles
    {
        Admin,
        Moderator,
        Member,
    }

    public Guid Id { get; set; }
    public MemberRoles MemberRole { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    public string? UserId { get; set; }
    [JsonIgnore]
    public User? User { get; set; }

    public Guid? ServerId { get; set; }
    [JsonIgnore]
    public Server? Server { get; set; }

    [JsonIgnore]
    public List<Message> Messages { get; set; } = [];

}
