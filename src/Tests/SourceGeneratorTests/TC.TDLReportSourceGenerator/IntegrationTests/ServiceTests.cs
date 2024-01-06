using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TallyConnector.Core.Models;
using TallyConnector.Core.Models.Masters;

namespace IntegrationTests;
public  class ServiceTests
{
    public RequestEnvelope GetRequestEnvelope<T>()
    {
        return new();
    }
}
