namespace TallyConnector.Services;
public partial class BaseTallyService

{
#if NET6_0
    private void LogRequestXML(global::System.String xml)
    {
        if (_logger?.IsEnabled(LogLevel.Debug) ?? false)
        {
            _logger?.Log(
                LogLevel.Debug,
                new EventId(1, nameof(LogRequestXML)),
                $"Sending Request To Tally XML - {xml}",
                null);
        }
    } 
    private void LogRequestType(global::System.String requestType)
    {
        if (_logger?.IsEnabled(LogLevel.Information) ?? false)
        {
            _logger?.Log(
                LogLevel.Debug,
                new EventId(1, nameof(LogRequestXML)),
                $"Sending Request Type - ({requestType}) To Tally",
                null);
        }
    }
#else
    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Debug,
        Message = "Sending Request To Tally XML - {xml}")]
    private partial void LogRequestXML(string xml) ;

    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Information,
        Message = "Sending Request Type - ({requestType}) To Tally")]
    private partial void LogRequestType(string requestType) ;
#endif


}
