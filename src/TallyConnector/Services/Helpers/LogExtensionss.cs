namespace TallyConnector.Services;
public partial class BaseTallyService

{
#if NET6_0
    private void LogRequestXML(global::System.String xml, global::System.String requestType)
    {
        if (_logger?.IsEnabled(LogLevel.Debug) ?? false)
        {
            _logger?.Log(
                LogLevel.Debug,
                new EventId(1, nameof(LogRequestXML)),
                $"Sending Request ({requestType}) To Tally XML - {xml}",
                null);
        }
    }
#else
    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Debug,
        Message = "Sending Request ({requestType}) To Tally XML - {xml}")]
    private partial void LogRequestXML(string xml, string requestType) ;
#endif


}
