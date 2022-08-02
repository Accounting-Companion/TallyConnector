namespace TallyConnector.Services;
internal class TLogger
{
    private readonly ILogger? _logger;

    public TLogger(ILogger? logger = null)
    {
        _logger = logger ?? NullLogger.Instance;
    }

    internal void LogTallyRequest(string? rXML)
    {
        if (_logger?.IsEnabled(LogLevel.Trace) ?? false)
        {
            _logger?.LogTrace("Sending request to tally with payload - {sXml}", rXML);
        }
        else
        {
            _logger?.LogInformation("Sending request to tally");
        }

    }
    internal void LogTallyResponse(string respXML)
    {
        if (_logger?.IsEnabled(LogLevel.Trace) ?? false)
        {
            _logger?.LogTrace("Received response from tally - {sXml}", respXML);

        }
        else
        {
            _logger?.LogInformation("Received response from tally");
        }
    }

    internal void BuildingOptions(Type type)
    {
        _logger?.LogDebug("Building {name}",type.Name);
    }

    internal void TallyReqError(string message)
    {
        _logger?.LogError("Error ocuured while sending request to Tally - {}", message);
    }
}
