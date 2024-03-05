using System.Data;
using TallyConnector.Core.Models;
using TallyConnector.Services;

namespace TC.Core.Tests;

[TestClass]
public class UnitTest1
{
    TallyService _service = new();
    public UnitTest1()
    {

    }

    [TestMethod]
    public async Task TestMethod1()
    {
        const string srcCollName = "TC_SrcStockItemsCOll";
        const string collName = "TC_CustomBatchCollection";
        RequestEnvelope requestEnvelope = new RequestEnvelope(HType.Collection, collName);
        TDLMessage tDLMessage = requestEnvelope.Body.Desc.TDL.TDLMessage;

        tDLMessage.Collection = [new Collection(srcCollName, colType: "StockItem"), new() { Name = collName, Collections = srcCollName, Walk = "BatchAllocations", NativeFields = ["*"] }];
        
        var reqxml = requestEnvelope.GetXML();
        TallyResult tallyResult = await _service.SendRequestAsync(reqxml, "Batch Allocations - All Items");
        var envelope =  XMLToObject.GetObjfromXml<Envelope<CustomItemAllocationType>>(tallyResult.Response);
        var items = envelope?.Body.Data.Collection?.Objects;
    }
}