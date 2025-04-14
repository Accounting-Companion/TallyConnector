using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TallyConnector.Core.Attributes.SourceGenerator;
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class ImplementTallyService : Attribute
{
    public string HandlerName { get; set; }

    



    public ImplementTallyService(string handlerName)
    {
        HandlerName = handlerName;
    }
}
