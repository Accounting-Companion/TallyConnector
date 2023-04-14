using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TallyConnector.Core.Exceptions;
public class TallyXMLParsingException : Exception
{
    public string XMLPart { get; set; }
    public IEnumerable<string> ExceptionTrace { get; set; }

    public TallyXMLParsingException(string message, Exception innerException, string xmlPart, IEnumerable<string> exceptionTrace) : base(message, innerException)
    {
        XMLPart = xmlPart;
        ExceptionTrace = exceptionTrace;
    }
}
