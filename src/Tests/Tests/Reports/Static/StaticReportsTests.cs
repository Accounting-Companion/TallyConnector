using TallyConnector.Reports.Models.Static;
using Tests.Services.TallyService;

namespace Tests.Reports.Static;
internal class StaticReportsTests : BaseTallyServiceTest
{
    [Test]
    public async Task GetSatesTest()
    {
        StatesReport value = await _tallyService.GetTDLReportAsync<TallyState, StatesReport>();
        var t = value.States.GroupBy(st => st.Country).ToDictionary(c => c.Key, c => c.Select(k => k.Name));
        string json = JsonSerializer.Serialize(t);
    }
    [Test]
    public async Task GetCountriesTest()
    {
        StatesReport value = await _tallyService.GetTDLReportAsync<TallyState, StatesReport>();
        var t = value.States.GroupBy(st => st.Country).ToDictionary(c => c.Key, c => c.Select(k => k.Name));
        string json = JsonSerializer.Serialize(t);
    }

}

[XmlRoot(ElementName = "TALLYSTATE.LIST")]
public class StatesReport
{
    [XmlElement(ElementName = "TALLYSTATE")]
    public List<TallyState> States { get; set; }
}

