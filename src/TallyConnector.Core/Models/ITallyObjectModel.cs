namespace TallyConnector.Core.Models;

public interface ITallyObject
{
    int AlterId { get; set; }
    int MasterId { get; set; }
}
public interface INamedTallyObject : ITallyObject
{
    string Name { get; set; }
}
public interface IAliasTallyObject : INamedTallyObject
{
    string? Alias { get; set; }

    List<LanguageNameList> LanguageNameList { get; set; }
}
public interface ICheckNull
{
    public bool IsNull();
}