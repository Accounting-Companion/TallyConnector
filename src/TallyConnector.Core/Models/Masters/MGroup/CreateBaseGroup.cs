using Mapper.Core.Attributes;
using TallyConnector.Core.Models.Interfaces.Masters.Group;

namespace TallyConnector.Core.Models.Masters.MGroup;


[Map<Group>]
public class CreateBaseGroup : ICreateBaseGroup
{
    public CreateBaseGroup(string name)
    {
        Name = name;
    }

    public CreateBaseGroup(string name, string? parent = null) : this(name)
    {
        Parent = parent;
    }

    public CreateBaseGroup(string name, string? alias, string? parent, string? remoteId) : this(name, alias)
    {
        Parent = parent;
        RemoteId = remoteId;
    }

    [Required]
    public string Name { get; set; }

    public string? Alias { get; set; }

    public string? Parent { get; set; }
    public string? RemoteId { get; set; }
}


