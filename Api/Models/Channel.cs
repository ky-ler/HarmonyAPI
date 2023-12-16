using System.Text.Json.Serialization;

namespace Api.Models;

public class Channel
{
    public enum ChannelTypes
    {
        Text,
        Voice
    }

    public Guid Id { get; set; }
    public required string Name { get; set; }
    public ChannelTypes ChannelType { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [JsonIgnore]
    public string? UserId { get; set; }
    [JsonIgnore]
    public User? User { get; set; }

    public Guid? ServerId { get; set; }
    [JsonIgnore]
    public Server? Server { get; set; }

    public List<Message> Messages { get; set; } = [];
}
