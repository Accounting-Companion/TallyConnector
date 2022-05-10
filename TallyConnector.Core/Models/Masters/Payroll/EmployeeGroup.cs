namespace TallyConnector.Core.Models.Masters.Payroll;

[XmlRoot(ElementName = "COSTCENTRE")]
public class EmployeeGroup : CostCenter.CostCenter
{
    [XmlElement(ElementName = "FORPAYROLL")]
    public string? ForPayroll { get; set; }

    [XmlElement(ElementName = "ISEMPLOYEEGROUP")]
    public string? IsEmployeeGroup { get; set; }


}