using NUnit.Framework;
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
            await TTally.GetCompaniesList();
            Assert.IsNotNull(TTally.CompaniesInfo);
            
        }
        

        [Test]
        public async Task CheckGetData()
        {

            await TTally.FetchAllTallyData();
            Assert.IsNotNull(TTally.Ledgers);
        }

        [Test]
        public async Task CheckGetGroup()
        {
            Group group = await TTally.GetGroup("Sundry Debtors");
            Assert.NotNull(group);
        }

        [Test]
        public async Task CheckGetLedger()
        {
            Ledger ledger = await TTally.GetLedger("Cash");
            Assert.NotNull(ledger);
        }

        [Test]
        public async Task CheckGetCostCategory()
        {
            CostCategory costCategory = await TTally.GetCostCategory("Primary Cost Category");
            Assert.NotNull(costCategory);
        }

        [Test]
        public async Task CheckGetCostCenter()
        {
            CostCenter costCenter = await TTally.GetCostCenter("Deepak");
            Assert.NotNull(costCenter);
        }

        [Test]
        public async Task CheckGetStockGroup()
        {
            StockGroup stockGroup = await TTally.GetStockGroup("Accessories");
            Assert.NotNull(stockGroup);
        }
        
        [Test]
        public async Task CheckGetStockCategory()
        {
            StockCategory stockCategory = await TTally.GetStockCategory("Accessories");
            Assert.NotNull(stockCategory);
        }

        [Test]
        public async Task CheckGetStockItem()
        {
            StockItem stockItem = await TTally.GetStockItem("CDROM Disks 100s");
            Assert.NotNull(stockItem);
        }

        [Test]
        public async Task CheckGetGodown()
        {
            Godown godown = await TTally.GetGodown("Assembly Floor");
            Assert.NotNull(godown);
        }

        [Test]
        public async Task CheckGetVoucherType()
        {
            VoucherType voucherType = await TTally.GetVoucherType("Attendance");
            Assert.NotNull(voucherType);
        }

        [Test]
        public async Task CheckGetUnits()
        {
            Unit unit = await TTally.GetUnit("Box");
            Assert.NotNull(unit);
        }

        [Test]
        public async Task CheckGetCurrency()
        {
            Currencies Currency = await TTally.GetCurrency("$");
            Assert.NotNull(Currency);
        }




        [Test]
        public async Task CheckGetVoucher()
        {
            Voucher voucher = await TTally.GetVoucherByMasterID("5036");
            Assert.NotNull(voucher);
        }

    }
}