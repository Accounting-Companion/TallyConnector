namespace TallyConnector.SourceGenerators.Models;
public class HelperMethodArgs
{
    public HelperMethodArgs(string nameSpace,
                            string className,
                            string objectNamespace,
                            string objectName,
                            string pluralName,
                            string genericTypeName)
    {
        NameSpace = nameSpace;
        ClassName = className;
        ObjectNameSpace = objectNamespace;
        ObjectName = objectName;
        PluralName = pluralName;
        GenericTypeName = genericTypeName;
    }

    public string NameSpace { get; }
    public string ClassName { get; }
    public string ObjectNameSpace { get; }
    public string ObjectName { get; }
    public string PluralName { get; }
    public string GenericTypeName { get; }
}
