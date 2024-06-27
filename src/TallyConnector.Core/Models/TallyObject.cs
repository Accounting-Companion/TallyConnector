
/* Unmerged change from project 'TallyConnector.Core (net6.0)'
Before:
namespace TallyConnector.Core.Models.Masters;
After:
using TallyConnector;
using TallyConnector.Core;
using TallyConnector.Core.Models;
using TallyConnector.Core.Models;
using TallyConnector.Core.Models.Masters;
*/
using TallyConnector.Core.Models.Masters;

namespace TallyConnector.Core.Models;
public class BaseTallyObject : IBaseTallyObject
{
    [XmlElement(ElementName = "GUID")]
    [IgnoreForCreateDTO]
    [TDLField(ExcludeInFetch = true)]
    public string GUID { get; set; }

    [XmlElement(ElementName = "REMOTEALTGUID")]
    public string RemoteId { get; set; }
}
public class TallyObject : BaseTallyObject, ITallyObject
{
    [XmlElement(ElementName = "MASTERID")]
    [IgnoreForCreateDTO]
    [TDLField(ExcludeInFetch = true)]
    public int MasterId { get; set; }

    [IgnoreForCreateDTO]
    [XmlElement(ElementName = "ALTERID")]
    public int AlterId { get; set; }

}
