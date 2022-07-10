using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Services.TallyService.TallyObjects;
internal class VoucherTests : BaseTallyServiceTest
{
    [Test]
    public async Task CheckGetAllVouchers()
    {
        var objects = await _tallyService.GetAllObjectsAsync<TCM.Voucher>(new()
        {
            FromDate = new(2010, 4, 1),
            FetchList = new List<string>()
                {
                    "MasterId", "*", "AllledgerEntries", "ledgerEntries", "Allinventoryenntries",
                    "InventoryEntries", "InventoryEntriesIn", "InventoryEntriesOut"
                }
        });
        Assert.NotNull(objects);
        Assert.AreEqual(1131, objects.Count);
    }
}
