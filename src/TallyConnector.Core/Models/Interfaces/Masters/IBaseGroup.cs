using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TallyConnector.Core.Models.Interfaces.Masters;
internal interface IBaseGroup : IBaseMasterObject
{
    string Name { get; set; }
}
