using TallyConnector.Core.Models.Masters;

namespace Tests.Services.TallyService.TallyObjects.Accounting;
public class VoucherTypeTests : BaseTallyServiceTest
{
    [Test]
    public async Task GetVoucherType()
    {
        var k = await _tallyService.GetVoucherTypeAsync<VoucherType>("POS Sales");
    }
    [Test]
    public async Task GetAllVoucherTypes()
    {
        var k = await _tallyService.GetVoucherTypesAsync();
    }
}
