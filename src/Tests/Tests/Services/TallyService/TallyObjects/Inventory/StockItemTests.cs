using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TallyConnector.Core.Models;
using TallyConnector.Core.Models.Masters.Inventory;

namespace Tests.Services.TallyService.TallyObjects.Inventory;
public class StockItemTests :  BaseTallyServiceTest
{
    [Test]
    public async Task TestCreateStockItem()
    {

        TCM.TallyResult tallyResult = await _tallyService.PostStockItemAsync(new StockItem() { Name="cfvbnm",MailingNames=new() { Guid.NewGuid().ToString("N").Substring(0, 8) } });
        Assert.That(tallyResult, Is.Not.Null);
        Assert.That(tallyResult.Status, Is.EqualTo(RespStatus.Sucess));
    }
    [Test]
    public async Task TestGetAllStockItems()
    {

        var stockItems = await _tallyService.GetStockItemsAsync();
        Assert.That(stockItems, Is.Not.Null);
    }

}
