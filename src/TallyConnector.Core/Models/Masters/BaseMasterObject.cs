using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TallyConnector.Core.Models.Interfaces.Masters;

namespace TallyConnector.Core.Models.Masters;


public class BaseMasterObject : TallyObject, IBaseMasterObject
{
    [XmlElement("NAME")]
    public string Name { get; set; }
}