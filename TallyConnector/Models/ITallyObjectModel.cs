namespace TallyConnector.Models;

public interface ITallyObject
{
    string GUID { get; set; }
    int? MasterId { get; set; }

    public void PrepareForExport();
}