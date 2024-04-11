namespace TallyConnector.Core.Models.Masters;

public interface ITallyObject
{
    int AlterId { get; set; }
    int MasterId { get; set; }
}