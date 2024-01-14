namespace TallyConnector.Core.Models.Common.Response;
[XmlRoot("ENVELOPE")]
public class ReportResponseEnvelope<T> where T : ITallyBaseObject
{
    public List<T>? Objects { get; set; }
}
