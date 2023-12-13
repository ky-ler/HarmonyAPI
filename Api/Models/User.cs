namespace Api.Models;

public class User
{
    public string Id { get; set; } = null!;

    public string Username { get; set; } = null!;

    public IEnumerable<Server> Servers { get; set; } = new List<Server>();
    public IEnumerable<Channel> Channels { get; set; } = new List<Channel>();
    public IEnumerable<Member> Members { get; set; } = new List<Member>();
}
