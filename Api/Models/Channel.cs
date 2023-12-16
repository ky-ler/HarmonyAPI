using System.Text.Json.Serialization;

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

    [JsonIgnore]
    public string? UserId { get; set; }
    [JsonIgnore]
    public User? User { get; set; }

    public Guid ServerId { get; set; }
    [JsonIgnore]
    public Server Server { get; set; } = null!;

    public List<Message> Messages { get; set; } = [];

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
