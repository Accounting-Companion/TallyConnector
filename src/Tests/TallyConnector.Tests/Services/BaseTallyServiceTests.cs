using System.Collections;
using TallyConnector.Core;
using TallyConnector.Core.Models.Request;
using TallyConnector.Models.TallyPrime.V6.Masters;
using TallyConnector.Models.TallyPrime.V6.Masters.Meta;
using TallyConnector.Services;
using XmlSourceGenerator.Abstractions;

namespace TallyConnector.Tests.Services;

public class BaseTallyServiceTests

{
    private readonly BaseTallyService TallyService;
    public BaseTallyServiceTests()
    {
        TallyService = new();
    }

    [Test]
    public async Task TestGetActiveSimpleCompanyNameAsync()
    {
        string companyName = await TallyService.GetActiveSimpleCompanyNameAsync();
        Assert.That(companyName, Is.Not.Null);

    }
    [Test]
    public async Task TestGetLicenseInfoAsync()
    {
        var licenseInfo = await TallyService.GetLicenseInfoAsync();
        Assert.That(licenseInfo, Is.Not.Null);

    }
    [Test]
    public async Task TestAutoColStats()
    {
        //TallyAbstractClient tallyCommonService = new TallyAbstractClient();
        // var objects =await tallyCommonService.GetObjectsAsync<Models.TallyPrime.V6.Masters.Ledger>(new());
        // await new TallyPrimeService().GetGroupsAsync();
        //await TallyService.(new TallyConnector.Core.Models.Request.AutoColumnReportPeriodRequestOptions());
    }

    public async Task TestNewXML()
    {
        var postEnvelope = new RequestEnvelope();
        postEnvelope.Body.RequestData.Data ??=  [];
        var data = postEnvelope.Body.RequestData.Data;

      
        postEnvelope.WriteToXml();


    }
}
