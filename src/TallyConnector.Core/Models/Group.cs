using System.Xml.Linq;
using TallyConnector.Core.Models.Common;
using TallyConnector.Core.Models.Common.Request;

namespace TallyConnector.Core.Models;


public partial class BaseTallyGroup : TallyObject
{
    
    public BaseTallyGroup(string name)
    {
        Name = name;
    }
    public BaseTallyGroup(string name, string? ParentGroupName)
    {
        Name = name;
        ParentGroup = ParentGroupName;
    }


    [TDLXMLSet("$Name")]
    public string Name { get; set; }

    [XmlElement(ElementName = "PARENT")]
    //[Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    [Required]
    public string? ParentGroup { get; set; }

    [TDLCollection(CollectionName = "LanguageName")]
    [XmlElement(ElementName = "LANGUAGENAME.LIST")]
    public List<LanguageNameList> LanguageNameList { get; set; }


}
public partial class LanguageNameList
{
    [TDLCollection(CollectionName = "Name")]
    [XmlElement(ElementName = "NAME.LIST")]
    public NameList NameList { get; set; }

    public int LanguageId { get; set; }
}
public partial class NameList
{
    [TDLCollection(CollectionName = "Name")]
    [XmlElement(ElementName = "NAME")]
    [TDLXMLSet(Set ="$Name")]
    public List<string>? NAMES { get; set; }
}
[GenerateTDLReport]
public partial class Group : BaseTallyGroup
{
    public Group(string name) : base(name)
    {
    }
    public Group(string name, string? ParentGroupName) : base(name, ParentGroupName)
    {
    }
    public static TDLMessage GetTDLMessaget()
    {

        TDLMessage tDLMessage = new()
        {
            Reports = new() { GetTDLReport() },
            Forms = new() { GetTDLForm() },
            Parts = GetTDLReportParts().ToList(),
            Lines = GetTDLReportLines().ToList(),
            Fields = GetTDLReportFields().ToList(),
        };
        return tDLMessage;

        //Part part = new(TDLReportName,CollectionName);
    }
    //public static global::TallyConnector.Core.Models.Common.Request.Line GetTDLLine2()
    //{
    //    return new global::TallyConnector.Core.Models.Common.Request.Line(TDLReportName, new List<string>() { "TC_NAME,TC_PARENT" }, TallyObject.GetTDLReportFields().Length > 0 ? TallyObject.TDLReportName : null, "BASETALLYGROUP");
    //}
    //public static global::TallyConnector.Core.Models.Common.Request.Report[] GetTDLReportst()
    //{
    //    int count = 1;
    //    Report[] baseReports = BaseTallyGroup.GetTDLReports();
    //    count += baseReports.Length;
    //    global::TallyConnector.Core.Models.Common.Request.Report[]  reports = new Report[count];
    //    for (int i = 0; i < count; i++)
    //    {
    //        if (i == 0)
    //        {
    //            reports[i] = new Report(TDLReportName);
    //            continue;
    //        }
    //        reports[i] = baseReports[i - 1];
    //    }
    //    return reports;
    //}
    //public static global::TallyConnector.Core.Models.Common.Request.Report[] GetTDLReportst2()
    //{
    //    int count = 0;
    //    Report[] baseReports = BaseTallyGroup.GetTDLReports();
    //    count += baseReports.Length;
    //    global::TallyConnector.Core.Models.Common.Request.Report[]  reports = new Report[]
    //    { 
    //        new Report(TDLReportName),
    //    };
    //    for (int i = 0; i < count; i++)
    //    {

    //        reports[i] = baseReports[i];
    //    }
    //    return reports;
    //}
    //public static global::TallyConnector.Core.Models.Common.Request.Report[] GetTDLReportst3()
    //{
    //    global::TallyConnector.Core.Models.Common.Request.Report[]  reports = new Report[1];
    //    reports[0] = new Report(TDLReportName);
    //    return Array.Empty<Report>();
    //}
}
public class GroupCreate : IPostTallyObject
{
    public string Action { get; set; }

    public string GUID { get; set; }
}



