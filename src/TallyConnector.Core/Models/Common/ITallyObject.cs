using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TallyConnector.Core.Models.Common;
public interface ITallyObject
{

}
public interface IPostTallyObject : ITallyObject
{
    public string Action { get; set; }
}
public abstract class TallyObject : ITallyObject
{
    public abstract string CollectionObjectType { get; }
    public abstract string XmlRootTag { get; }
}
