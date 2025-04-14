using TallyConnector.Core.Models.Interfaces;

namespace TallyConnector.Core.Models.Interfaces.Masters;
public interface IBaseLedger : IBaseMasterObject
{
    string Name { get; set; }
    string Group { get; set; }
}
