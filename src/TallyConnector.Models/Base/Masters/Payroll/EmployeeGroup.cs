namespace TallyConnector.Models.Base.Masters.Payroll;
[XmlRoot(ElementName = "COSTCENTRE")]
[XmlType(AnonymousType = true)]
public partial class EmployeeGroup : CostCentre
{
    public new static IEnumerable<string> GetDefaultFilters()
    {
        return ["EmployeesFilter", "EmployeeGroupFilter"];
    }
}
