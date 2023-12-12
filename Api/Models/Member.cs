namespace Api.Models;

public class Member
{
    public enum MemberRoles
    {
        Admin,
        Member
    }

    public Guid Id { get; set; } = Guid.NewGuid();

    public MemberRoles MemberRole { get; set; } = MemberRoles.Member;

    public string? UserId { get; set; }

    public User? User { get; set; }

    public Guid? ServerId { get; set; }

    public Server Server { get; set; } = null!;

    public IEnumerable<Message>? Messages { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
}
