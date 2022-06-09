using System.Collections.Generic;
using System.Threading.Tasks;
using TallyConnector.Core.Models;
using TallyConnector.Core.Models.Masters;
using TallyConnector.Core.Models.Masters.CostCenter;
using TallyConnector.Core.Models.Masters.Inventory;
using TallyConnector.Core.Models.Masters.Payroll;

namespace Tests;

public class TallyTests
{
    TallyConnector.Tally TTally = new();
    [SetUp]
    public void Setup()
    {

    }

    [Test]
    public async Task TallyPortCheck()
    {

        await TTally.Check();
        Assert.IsNotNull(TTally.Status);
        Assert.AreEqual("Running", TTally.Status);
    }
    [Test]
    public async Task TallyCheckCopanies()
    {
        await TTally.Check();
        await TTally.GetCompaniesList();
        Assert.IsNotNull(TTally.CompaniesList);

    }

    [Test]
    public async Task TallyCheckCopaniesondisk()
    {
        await TTally.Check();
        List<CompanyOnDisk> companies = await TTally.GetCompaniesListinPath();
        Assert.IsNotNull(companies);

    }


    [Test]
    public async Task CheckGetData()
    {
        await TTally.Check();
        await TTally.FetchAllTallyData();
        List<BasicTallyObject> LedgerMasters = TTally.Masters.Find(master => master.MasterType == TallyObjectType.Ledgers).Masters;
        Assert.IsNotNull(LedgerMasters);
    }

    [Test]
    public async Task CheckGetActiveCompany()
    {
        string test = await TTally.GetActiveTallyCompany();
        Assert.IsNotNull(test);
    }

    [Test]
    public void CheckGetObject()
    {
        //TTally.GetObjectfromTally();
        Assert.IsNotNull(null);
    }
    [Test]
    public async Task CheckGetGroup()
    {
        await TTally.Check();
        //Group group = await TTally.GetGroup("Sundry Debtors");
        Group group = await TTally.GetObjectfromTally<Group>("Sales Tax Payable");
        group.OtherAttributes = null;
        group.OtherFields = null;
        string grp = group.GetXML();
        Assert.NotNull(group);
    }
    [Test]
    public async Task CheckCreateGroup()
    {
        await TTally.Check();
        PResult result = await TTally.PostGroup(new Group()
        {
            Name = "TestGroup2345",
            Parent = "Primary",
            AddlAllocType = AdAllocType.AppropriateByValue,
        });
        Assert.IsTrue(result.Status == RespStatus.Sucess);
    }
    [Test]
    public async Task CheckAlterGroup()
    {
        await TTally.Check();
        PResult result = await TTally.PostGroup(new Group()
        {
            OldName = "TestGroup",
            Parent = "Sundry Debtors",
            AddlAllocType = AdAllocType.NotApplicable,
        }); ;
        Assert.IsTrue(result.Status == RespStatus.Sucess);
    }
    [Test]
    public async Task CheckDeleteGroup()
    {
        await TTally.Check();
        PResult result = await TTally.PostGroup(new Group()
        {
            OldName = "TestGroup",
            Action = Action.Delete,
        });
        Assert.IsTrue(result.Status == RespStatus.Sucess);
    }
    [Test]
    public async Task CheckGetLedger()
    {
        await TTally.Check();
        Ledger ledger = await TTally.GetLedger<Ledger>("abc India Pvt. Ltd.");

        //ledger.OtherAttributes = null;
        //ledger.OtherFields = null;
        //ledger.OpeningBal.ForexAmount = 200;
        //ledger.OpeningBal.RateOfExchange = 55;

        //await TTally.PostLedger<Ledger>(ledger);
        //string json = ledger.GetJson();
        Assert.NotNull(ledger);
        Assert.NotNull(ledger.Name, "abc India Pvt. Ltd.");
    }

    [Test]
    public async Task CheckCreateLedger()
    {
        if (await TTally.Check())
        {
            Ledger nledger = new() { Name = "TestLedger12345", Alias = "gj2345knndnk", Group = "Sundry Debtors" };
            PResult pResult = await TTally.PostLedger(nledger);
            Assert.IsTrue(pResult.Status == RespStatus.Sucess);
        }
    }

    [Test]
    public async Task CheckAlterLedger()
    {
        if (await TTally.Check())
        {
            Ledger nledger = new() { OldName = "TestLedger", Name = "TestLedgeediteddd", Group = "Sundry Debtors" };
            PResult pResult = await TTally.PostLedger(nledger);
            Assert.IsTrue(pResult.Status == RespStatus.Sucess);
        }
    }
    [Test]
    public async Task CheckGetLedgerDynamic()
    {
        await TTally.Check();
        List<string> fields = new() { "Name", "Parent", "OpeningBalance", "Closing Balance" };
        Ledger ledger = await TTally.GetLedgerDynamic<Ledger>("Canara Bank", fromDate: "01032021", toDate: "31032021", fetchList: fields);
        string xml = ledger.GetXML();
        Assert.NotNull(ledger);
    }

    [Test]
    public async Task CheckGetCostCategory()
    {
        await TTally.Check();
        CostCategory costCategory = await TTally.GetCostCategory<CostCategory>("Test");
        Assert.NotNull(costCategory);
    }

    [Test]
    public async Task CheckGetCostCenter()
    {
        await TTally.Check();
        CostCenter costCenter = await TTally.GetCostCenter<CostCenter>("Deepak");
        Assert.NotNull(costCenter);
    }

    [Test]
    public async Task CheckGetStockGroup()
    {
        await TTally.Check();
        StockGroup stockGroup = await TTally.GetStockGroup<StockGroup>("HCL");
        Assert.NotNull(stockGroup);
    }

    [Test]
    public async Task CheckGetStockCategory()
    {
        await TTally.Check();
        StockCategory stockCategory = await TTally.GetStockCategory<StockCategory>("Asdrt");
        Assert.NotNull(stockCategory);
    }

    [Test]
    public async Task CheckGetStockItem()
    {
        await TTally.Check();
        StockItem stockItem = await TTally.GetStockItem<StockItem>("Test Unit");

        Assert.NotNull(stockItem);
    }

    [Test]
    public async Task CheckGetGodown()
    {
        await TTally.Check();
        Godown godown = await TTally.GetGodown<Godown>("ASadf");
        Assert.NotNull(godown);
    }

    [Test]
    public async Task CheckGetVoucherType()
    {
        await TTally.Check();
        VoucherType voucherType = await TTally.GetVoucherType<VoucherType>("POS Sales");
        Assert.NotNull(voucherType);
    }

    [Test]
    public async Task CheckGetUnits()
    {
        await TTally.Check();
        Unit unit = await TTally.GetUnit<Unit>("Box of 100 strips of 10 tablets");
        Assert.NotNull(unit);
    }

    [Test]
    public async Task CheckGetCurrency()
    {
        await TTally.Check();
        Currency Currency = await TTally.GetCurrency<Currency>("5344", MasterLookupField.AlterId);
        Assert.NotNull(Currency);
    }

    [Test]
    public async Task CheckGetAttendanceType()
    {
        await TTally.Check();
        AttendanceType attendanceType = await TTally.GetAttendanceType<AttendanceType>("TEST");
        Assert.NotNull(attendanceType);
    }

    [Test]
    public async Task CheckGetEmployeeGroup()
    {
        await TTally.Check();
        EmployeeGroup EmpGrp = await TTally.GetEmployeeGroup<EmployeeGroup>("Sales");
        Assert.NotNull(EmpGrp);
    }

    [Test]
    public async Task CheckGetEmployee()
    {
        await TTally.Check();
        Employee Employee = await TTally.GetEmployee<Employee>("Ajay");
        Assert.NotNull(Employee);
    }



    [Test]
    public async Task CheckGetVoucher()
    {
        await TTally.Check();
        Voucher voucher = await TTally.GetVoucher<Voucher>("1", VoucherLookupField.VoucherNumber);
        Assert.NotNull(voucher);
    }

    [Test]
    public async Task CheckGetBasicVoucher()
    {
        await TTally.Check();
        var BasicData = await TTally.GetBasicObjectData("VOUCHER");

        Assert.NotZero(BasicData.Count);
    }



    [Test]
    public async Task CheckGetLicenseInfo()
    {
        await TTally.Check();
        await TTally.GetLicenseInfo();
    }
    [Test]
    public async Task CheckVoucherCount()
    {
        await TTally.Check();
        string ReportName = "$$NUM";
        //CusColEnvelope cusColEnvelope = new(RequestTye.Export, HType.Function, ReportName);
        //cusColEnvelope.Body.Desc.TDL.TDLMessage = new(reportName: ReportName,
        //                                              colName: "VoucherTypeColl",
        //                                              colType: "VoucherType",
        //                                              fields: new List<Field>()
        //                                              {
        //                                                  new Field("Count","$$DirectTotalVch:$Name","Count")
        //                                              },
        //                                              nativeFields: new List<string>() { "Name" });
        //string xml = cusColEnvelope.GetXML();
    }



}
