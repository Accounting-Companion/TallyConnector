using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TallyConnector.Core.Models.Masters;

namespace Tests.Services.TallyService.TallyObjects.Accounting;
public class VoucherTypeTests : BaseTallyServiceTest
{
    [Test]
    public async Task GetVoucherType()
    {
        var k = await _tallyService.GetVoucherTypeAsync<VoucherType>("Attendance");
    }
}
