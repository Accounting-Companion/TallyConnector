using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TallyConnector.Core.Models.Common;
public interface IBaseTallyObject
{
    string GUID { get; set; }
}
public interface ITallyObject : IBaseTallyObject
{
    string AlterId { get; set; }
    string MasterId { get; set; }
}
public interface IPostTallyObject : IBaseTallyObject
{
    public string Action { get; set; }
}
public partial class BaseTallyObject : IBaseTallyObject
{
    public string GUID { get; set; }
}
public abstract partial class TallyObject : BaseTallyObject, ITallyObject
{
    public string AlterId { get; set; }
    public string MasterId { get; set; }
}
