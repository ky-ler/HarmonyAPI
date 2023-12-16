using Api.Models;

namespace Api.Data;

public class MemberDto
{
    public required string Id { get; set; }
    public string? Username { get; set; }
    public string? ImageUrl { get; set; }
    public Member.MemberRoles? MemberRole { get; set; }
}