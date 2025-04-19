namespace TallyConnector.Core.Exceptions;

[Serializable]
public class TallyConnectivityException : Exception
{
    public TallyConnectivityException()
    {
    }

    public TallyConnectivityException(string message) : base(message)
    {
    }
    public TallyConnectivityException(string message, string Url) :
        base($"{message} at {Url}")
    {

    }
    public TallyConnectivityException(string message, string Url,Exception innerException) :
        base($"{message} at {Url}", innerException)
    {

    }
    public TallyConnectivityException(string message, Exception innerException) : base(message, innerException)
    {
    }

}

