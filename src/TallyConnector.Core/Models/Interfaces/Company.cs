namespace TallyConnector.Core.Models.Interfaces;
public interface IBaseCompany : IBaseObject
{
    string CompNum { get; set; }
    string? Name { get; set; }
    bool IsGroupCompany { get; set; }
}
public interface ICompany : IBaseCompany
{
    string GUID { get; set; }
    DateTime? StartingFrom { get; set; }
}
