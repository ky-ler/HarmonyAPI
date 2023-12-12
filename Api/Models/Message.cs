using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models;

public class Message
{

    public Guid Id { get; set; } = Guid.NewGuid();

    [Column(TypeName = "text")]
    public string Content { get; set; } = null!;

    public string? UserId { get; set; } = null!;

    public User? User { get; set; }

    public Guid ChannelId { get; set; }

    public Channel Channel { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [Column(TypeName = "text")]
    public string? FileUrl { get; set; }

    public bool IsDeleted { get; set; } = false;
}
