using Api.Models;

namespace Api.Data;

public class ChannelDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public Channel.ChannelTypes ChannelType { get; set; }
}