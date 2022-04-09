namespace TallyConnector.Models;

public interface IBasicTallyObject
{
    string GUID { get; set; }
    int? MasterId { get; set; }
}