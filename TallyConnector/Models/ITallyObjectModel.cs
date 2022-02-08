namespace TallyConnector.Models;

public interface ITallyObject
{
    string GUID { get; set; }
    int? TallyId { get; set; }
}