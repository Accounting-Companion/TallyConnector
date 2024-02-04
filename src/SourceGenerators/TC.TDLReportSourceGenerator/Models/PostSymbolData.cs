namespace TC.TDLReportSourceGenerator.Models;

internal class MapSymbolData
{
    public string Name { get; }
    public string FullName { get; }

    public string MapToFullName { get; }
    public string MapToName { get; }


    public List<ChildMapSymbolData> ChildMapSymbolData { get; } = [];
}

internal class ChildMapSymbolData
{
    public string PropertyName { get; }
    public string PropertyToName { get; }
    public bool IsComplex { get; }
    public FunctionDetail? FunctionDetail { get; }
}