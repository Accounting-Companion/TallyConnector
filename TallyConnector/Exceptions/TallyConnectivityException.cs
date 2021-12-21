using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TallyConnector.Exceptions
{
    [Serializable]
    public class TallyConnectivityException : Exception
    {
        public TallyConnectivityException()
        {
        }

        public TallyConnectivityException(string message) : base(message)
        {
        }
        public TallyConnectivityException(string message,string Url) :
            base($"{message} at {Url}")
        {

        }
        public TallyConnectivityException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TallyConnectivityException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
