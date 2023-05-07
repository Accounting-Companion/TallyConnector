namespace TallyConnector.Core.Models.Service;
public class TallyResult
{
    public RespStatus Status { get; set; }

    public string? Response { get; set; }
}
public enum RespStatus
{
    Sucess,
    Failure
}