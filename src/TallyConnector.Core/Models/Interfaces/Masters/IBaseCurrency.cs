using TallyConnector.Core.Models.Interfaces;

namespace TallyConnector.Core.Models.Interfaces.Masters;
public interface IBaseCurrency : IBaseMasterObject
{
    string Name { get; set; }
    string FormalName { get; set; }
}
