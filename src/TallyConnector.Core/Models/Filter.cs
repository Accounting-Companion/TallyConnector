namespace TallyConnector.Core.Models;

public class Filter
{
    public Filter()
    {
    }

    public Filter(string filterName, string filterFormulae, bool excludeinCollection = false)
    {
        FilterName = filterName;
        FilterFormulae = filterFormulae;
        ExcludeinCollection = excludeinCollection;
    }

    public string FilterName { get; set; }

    public string FilterFormulae { get; set; }
    public bool ExcludeinCollection { get; set; }
}
