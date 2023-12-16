namespace Api.Models;

public class User
{
    public string Id { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string ImageUrl { get; set; } = "https://via.placeholder.com/160x160";

    public List<Server> Servers { get; set; } = [];
    public List<Channel> Channels { get; set; } = [];
    public List<Member> Members { get; set; } = [];
}
