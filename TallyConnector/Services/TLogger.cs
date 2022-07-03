namespace TallyConnector.Services;
internal class TLogger
{
    private readonly ILogger? _logger;

    public TLogger(ILogger? logger = null)
    {
        _logger = logger ?? NullLogger.Instance;
    }
    internal void LogTallyRequest(string rXML)
    {
        _logger?.LogTrace("Sending request to tally with payload - {sXml}", rXML);
    }
    internal void LogTallyResponse(string respXML)
    {
        _logger?.LogTrace("Received response from tally - {sXml}", respXML);
    }
}
