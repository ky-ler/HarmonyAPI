using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models;

[Index(nameof(InviteCode), IsUnique = true)]
public class Server
{
    public Guid Id { get; set; }
    public required string Name { get; set; }

    [Column(TypeName = "text")]
    public required string ImageUrl { get; set; }
    public required string InviteCode { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public string? UserId { get; set; }

    public User? User { get; set; }

    public List<Member> Members { get; set; } = [];

    public List<Channel> Channels { get; set; } = [];

}