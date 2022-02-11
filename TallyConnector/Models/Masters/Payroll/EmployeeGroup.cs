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