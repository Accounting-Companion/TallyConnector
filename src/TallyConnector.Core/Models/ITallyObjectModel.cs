namespace TallyConnector.Core.Models;

public interface ITallyObject
{
    string? GUID { get; set; }
    string? RemoteId { get; set; }
    int? MasterId { get; set; }

    public void PrepareForExport();

    public void RemoveNullChilds();
}

public interface ICheckNull
{
    public bool IsNull();
}