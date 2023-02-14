namespace TallyConnector.Core.Models.Interfaces;
public interface ICreateNamedTallyObject :  ICreateTallyObject, INamedObject
{
}
public interface IReadNamedTallyObject :  IReadTallyObject, INamedObject
{

}
public interface INamedTallyObject : ITallyObject, INamedObject
{

}
public interface INamedObject  
{
    string Name { get; set; }
}
