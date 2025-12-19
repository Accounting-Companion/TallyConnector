namespace TallyConnector.Core.Models.Interfaces;


public interface IBaseObject
{
    
}
public interface IBaseTallyObject : IBaseObject
{
    string GUID { get; set; }

    string RemoteId { get; set; }
}

