namespace TallyConnector.Core.Models.Interfaces;
public interface ICreateNameAndAliasTallyObject : ICreateNamedTallyObject, IAliasObject
{
}
public interface IReadNameAndAliasTallyObject : IReadNamedTallyObject, IAliasObject
{
}
public interface INameAndAliasTallyObject : INamedTallyObject
{
   public List<LanguageNameList>? LanguageNameList { get; set; }
}
public interface IAliasObject
{
    string? Alias { get; set; }
}
