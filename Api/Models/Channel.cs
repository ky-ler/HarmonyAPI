namespace Api.Models;

public class Channel
{
    public enum ChannelTypes
    {
        Text,
        Voice
    }

    public Guid Id { get; set; } = new Guid();
    public string Name { get; set; } = null!;
    public ChannelTypes ChannelType { get; set; } = ChannelTypes.Text;

    public string? UserId { get; set; }
    public User? User { get; set; }

    public Guid ServerId { get; set; }
    public Server Server { get; set; } = null!;

    public IEnumerable<Member> Members { get; set; } = new List<Member>();
    public IEnumerable<Message> Messages { get; set; } = new List<Message>();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
}
