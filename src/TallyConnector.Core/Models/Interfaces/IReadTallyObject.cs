namespace TallyConnector.Core.Models.Interfaces;
public interface IReadTallyObject
{
    public string GUID { get; set; }
    public int AlterId { get; set; }
    public int MasterId { get; set; }
    public string? RemoteId { get; set; }
}
