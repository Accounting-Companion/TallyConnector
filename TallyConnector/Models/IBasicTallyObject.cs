namespace TallyConnector.Models;

public interface IBasicTallyObject
{
    string GUID { get; set; }
    int? TallyId { get; set; }
}