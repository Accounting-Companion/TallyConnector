namespace TallyConnector.Core.Models;

public interface ITallyObject
{
    string? GUID { get; set; }
    string? RemoteId { get; set; }
    int? MasterId { get; set; }

    public void PrepareForExport();
    /// <summary>
    /// Removes Null Childs that are created during xml deserilisation
    /// </summary>
    public void RemoveNullChilds();
}
public interface INamedTallyObject : ITallyObject
{
    string Name { get; set; }
}
public interface ICheckNull
{
    public bool IsNull();
}