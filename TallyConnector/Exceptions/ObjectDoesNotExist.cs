using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TallyConnector.Exceptions
{
    public class ObjectDoesNotExist : Exception
    {
        public ObjectDoesNotExist()
        {
        }

        public ObjectDoesNotExist(string message) : base(message)
        {
        }
        public ObjectDoesNotExist(string objectType,string identifier,string identifiervalue,string companyname) :
            base(companyname != null ? $"{objectType} with {identifier} - \"{identifiervalue}\" in \"{companyname}\" does not exist": $"{objectType} with {identifier} - \"{identifiervalue}\" does not exist in active company in Tally")
        {
        }

        public ObjectDoesNotExist(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ObjectDoesNotExist(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
