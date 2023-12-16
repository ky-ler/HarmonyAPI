using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Api.Models;

public class Message
{

    public Guid Id { get; set; } = Guid.NewGuid();

    [Column(TypeName = "text")]
    public string Content { get; set; } = null!;

    public string? Username { get; set; }
    public string? ImageUrl { get; set; } = "https://via.placeholder.com/160x160";

    [JsonIgnore]
    public Guid? MemberId { get; set; }
    [JsonIgnore]
    public Member? Member { get; set; }

    public Guid? ChannelId { get; set; }
    [JsonIgnore]
    public Channel? Channel { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "text")]
    public string? FileUrl { get; set; }

    public bool IsDeleted { get; set; } = false;
}
