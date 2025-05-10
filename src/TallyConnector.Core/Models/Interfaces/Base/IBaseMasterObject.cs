namespace TallyConnector.Core.Models.Interfaces;



public interface IBaseMasterObject : IBaseObject
{
}




public interface IBaseTallyObjectDTO
{
    string RemoteId { get; set; }

    Action Action { get; set; }
}

public interface IBaseMasterObjectDTO : IBaseTallyObjectDTO
{
    public string Name { get; set; }
}