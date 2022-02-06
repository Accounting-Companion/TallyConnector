using MCS = TallyConnector.Models.Masters.CostCenter;

namespace TallyConnector.Models.Masters.Payroll;

[XmlRoot(ElementName = "COSTCENTRE")]
public class EmployeeGroup : MCS.CostCenter
{
    [XmlElement(ElementName = "FORPAYROLL")]
    public string ForPayroll { get; set; }

    [XmlElement(ElementName = "ISEMPLOYEEGROUP")]
    public string IsEmployeeGroup { get; set; }


}

[XmlRoot(ElementName = "ENVELOPE")]
public class EmployeeGroupEnvelope : TallyXmlJson
{

    [XmlElement(ElementName = "HEADER")]
    public Header Header { get; set; }

    [XmlElement(ElementName = "BODY")]
    public EmployeeGroupBody Body { get; set; } = new();
}

[XmlRoot(ElementName = "BODY")]
public class EmployeeGroupBody
{
    [XmlElement(ElementName = "DESC")]
    public Description Desc { get; set; } = new();

    [XmlElement(ElementName = "DATA")]
    public EmployeeGroupData Data { get; set; } = new();
}

[XmlRoot(ElementName = "DATA")]
public class EmployeeGroupData
{
    [XmlElement(ElementName = "TALLYMESSAGE")]
    public EmployeeGroupMessage Message { get; set; } = new();


    [XmlElement(ElementName = "COLLECTION")]
    public EmployeeGroupColl Collection { get; set; } = new EmployeeGroupColl();


}

[XmlRoot(ElementName = "COLLECTION")]
public class EmployeeGroupColl
{
    [XmlElement(ElementName = "COSTCENTRE")]
    public List<EmployeeGroup> EmployeeGroups { get; set; }
}

[XmlRoot(ElementName = "TALLYMESSAGE")]
public class EmployeeGroupMessage
{
    [XmlElement(ElementName = "COSTCENTRE")]
    public EmployeeGroup EmployeeGroup { get; set; }
}