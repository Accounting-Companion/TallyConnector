using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using TallyConnector.Models;

namespace Tests
{
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
            Assert.IsNotNull(TTally.Ledgers);
        }

        [Test]
        public async Task CheckGetGroup()
        {
            await TTally.Check();
            Group group = await TTally.GetGroup("Sundry Debtors");
            Assert.NotNull(group);
        }
        [Test]
        public async Task CheckCreateGroup()
        {
            await TTally.Check();
            PResult result = await TTally.PostGroup(new Group()
            {
                Name = "TestGroup",
                Parent = "Sundry Debtors",
                AddLAllocType = AdAllocType.AppropriateByValue,
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
                AddLAllocType = AdAllocType.NotApplicable,
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
            Ledger ledger = await TTally.GetLedger("TestLedgerCEdited__");
            ledger.OtherAttributes = null;
            ledger.OtherFields = null;
            await TTally.PostLedger(ledger);
            Assert.NotNull(ledger);
        }

        [Test]
        public async Task CheckCreateLedger()
        {
            if (await TTally.Check())
            {
                Ledger nledger = new() { Name = "TestLedgerCres",Alias="gjkwknndnk", Group = "Sundry Debtors" };
                PResult pResult = await TTally.PostLedger(nledger);
                Assert.IsTrue(pResult.Status == RespStatus.Sucess);
            }
        }

        [Test]
        public async Task CheckAlterLedger()
        {
            if (await TTally.Check())
            {
                Ledger nledger = new() { OldName = "TestLedgerCres", Name = "TestLedgerCEdited___", Group = "Sundry Debtors" };
                PResult pResult = await TTally.PostLedger(nledger);
                Assert.IsTrue(pResult.Status == RespStatus.Sucess);
            }
        }
        [Test]
        public async Task CheckGetLedgerDynamic()
        {
            await TTally.Check();
            List<string> fields = new List<string>() { "Name", "Parent", "OpeningBalance", "Closing Balance" };
            Ledger ledger = await TTally.GetLedgerDynamic("Canara Bank", fromDate: "01032021", toDate: "31032021", Nativelist: fields);
            string xml = ledger.GetXML();
            Assert.NotNull(ledger);
        }

        [Test]
        public async Task CheckGetCostCategory()
        {
            await TTally.Check();
            CostCategory costCategory = await TTally.GetCostCategory("Test");
            Assert.NotNull(costCategory);
        }

        [Test]
        public async Task CheckGetCostCenter()
        {
            await TTally.Check();
            CostCenter costCenter = await TTally.GetCostCenter("Deepak");
            Assert.NotNull(costCenter);
        }

        [Test]
        public async Task CheckGetStockGroup()
        {
            await TTally.Check();
            StockGroup stockGroup = await TTally.GetStockGroup("Accessories");
            Assert.NotNull(stockGroup);
        }

        [Test]
        public async Task CheckGetStockCategory()
        {
            await TTally.Check();
            StockCategory stockCategory = await TTally.GetStockCategory("Accessories");
            Assert.NotNull(stockCategory);
        }

        [Test]
        public async Task CheckGetStockItem()
        {
            await TTally.Check();
            StockItem stockItem = await TTally.GetStockItem("CDROM Disks 100s");
            stockItem.OtherFields = null;
            stockItem.OtherAttributes = null;
            await TTally.PostStockItem(stockItem);
            Assert.NotNull(stockItem);
        }

        [Test]
        public async Task CheckGetGodown()
        {
            await TTally.Check();
            Godown godown = await TTally.GetGodown("Assembly Floor");
            Assert.NotNull(godown);
        }

        [Test]
        public async Task CheckGetVoucherType()
        {
            await TTally.Check();
            VoucherType voucherType = await TTally.GetVoucherType("Attendance");
            Assert.NotNull(voucherType);
        }

        [Test]
        public async Task CheckGetUnits()
        {
            await TTally.Check();
            Unit unit = await TTally.GetUnit("Box");
            Assert.NotNull(unit);
        }

        [Test]
        public async Task CheckGetCurrency()
        {
            await TTally.Check();
            Currency Currency = await TTally.GetCurrency("$");
            Assert.NotNull(Currency);
        }

        [Test]
        public async Task CheckGetEmployeeGroup()
        {
            await TTally.Check();
            EmployeeGroup EmpGrp = await TTally.GetEmployeeGroup("Accounts");
            Assert.NotNull(EmpGrp);
        }

        [Test]
        public async Task CheckGetEmployee()
        {
            await TTally.Check();
            Employee Employee = await TTally.GetEmployee("Rahul");
            Assert.NotNull(Employee);
        }



        [Test]
        public async Task CheckGetVoucher()
        {
            await TTally.Check();
            Voucher voucher = await TTally.GetVoucher("dfghj");
            Assert.NotNull(voucher);
        }



        [Test]
        public async Task CheckGetLicenseInfo()
        {
            await TTally.Check();
            await TTally.GetLicenseInfo();
        }
    }
}