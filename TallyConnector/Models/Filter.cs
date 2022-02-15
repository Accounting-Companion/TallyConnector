namespace TallyConnector.Models;

public class Filter
{
    public Filter()
    {
    }

    public Filter(string filterName, string filterFormulae)
    {
        FilterName = filterName;
        FilterFormulae = filterFormulae;
    }

    public string FilterName { get; set; }

    public string FilterFormulae { get; set; }
}
