namespace TallyConnector.TDLReportSourceGenerator.Services;
public static class TDLClassReportTransformer
{
    internal static void Trasform(this ClassData classData, IAssemblySymbol assembly, CancellationToken token)
    {
        TDLClassReport report = new();

        foreach (var member in classData.GetAllDirectMembers())
        {
            TrasformMember(member, classData, report);
        }
    }

    private static void TrasformMember(ClassPropertyData member, ClassData classData, TDLClassReport report)
    {
        var overiddenProperties = classData.GetOveriddenProperties(member.Name);
        if (member.IsComplex)
        {
            report.ComplexFieldsCount++;
        }
        else
        {
            if (member.IsList)
            {
                report.ComplexFieldsCount++;
            }
            report.SimpleFieldsCount++;
        }
    }

    private static IEnumerable<ClassPropertyData> GetOveriddenProperties(this ClassData classData, string name)
    {
        if (classData.BaseData == null)
        {
            return [];
        }
        classData.BaseData.Members.TryGetValue(name, out var classPropertyData);
        if (classPropertyData != null)
        {
            return [classPropertyData, .. classData.BaseData.GetOveriddenProperties(name)];
        }
        else
        {
            return classData.BaseData.GetOveriddenProperties(name);
        }

    }

    public static IEnumerable<ClassPropertyData> GetAllDirectMembers(this ClassData classData)
    {
        var baseProperties = classData.BaseData?.GetAllDirectMembers();
        return [.. baseProperties ?? [], .. classData.Members.Values];
    }
}
