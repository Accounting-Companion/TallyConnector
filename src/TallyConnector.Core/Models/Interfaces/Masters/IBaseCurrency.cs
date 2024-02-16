using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TallyConnector.Core.Models.Interfaces.Masters;
public interface IBaseCurrency : IBaseMasterObject
{
    string Name { get; set; }
    string FormalName { get; set; }
}
