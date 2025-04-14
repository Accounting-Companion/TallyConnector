using TallyConnector.Core.Models.Interfaces;

namespace TallyConnector.Core.Models.Interfaces.Masters;
internal interface IBaseGroup : IBaseMasterObject
{
    string Name { get; set; }
}
