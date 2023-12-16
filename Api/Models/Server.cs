using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Api.Models;

[Index(nameof(InviteCode), IsUnique = true)]
public class Server
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public required string Name { get; set; }

    [Column(TypeName = "text")]
    public string ImageUrl { get; set; } = "https://via.placeholder.com/160x160";

    public string InviteCode { get; set; } = Guid.NewGuid().ToString();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public string? UserId { get; set; }

    [JsonIgnore]
    public User? User { get; set; }

    public List<Member> Members { get; set; } = [];

    public List<Channel> Channels { get; set; } = [];

}