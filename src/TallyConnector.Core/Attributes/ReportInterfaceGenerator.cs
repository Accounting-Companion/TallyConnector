namespace TallyConnector.Core.Attributes;
public interface IReportInterfaceGenerator
{
    public IEnumerable<string> GetFields();
    public string GetXMLTDLReport();
}
public interface IReportInterfaceGenerator<CollectionType> : IReportInterfaceGenerator where CollectionType : class
{

}

