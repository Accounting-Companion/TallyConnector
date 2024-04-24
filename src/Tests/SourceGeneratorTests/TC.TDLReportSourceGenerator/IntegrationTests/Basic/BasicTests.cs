using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TallyConnector.Core.Attributes;

namespace IntegrationTests.Basic
{
    [TestClass]
    public class BasicTests
    {
        [TestMethod]
        public void TestConstStrings()
        {
            // FieldNames
            Assert.AreEqual($"TC_{nameof(TestBasicNS.Voucher)}_{nameof(TestBasicNS.Voucher.Name)}", TestBasicNS.TallyService.VoucherNameTDLFieldName);
            Assert.AreEqual($"TC_{nameof(TestBasicNS.Voucher)}_{nameof(TestBasicNS.Voucher.Parent)}", TestBasicNS.TallyService.VoucherParentTDLFieldName);


            Assert.AreEqual($"TC_{nameof(TestBasicNS.Voucher)}List", TestBasicNS.TallyService.VoucherReportName);
            Assert.AreEqual($"TC_{nameof(TestBasicNS.Voucher)}{nameof(TestBasicNS.Voucher.LedgerEntries)}List", TestBasicNS.TallyService.VoucherLedgerEntriesReportName);
        }
        [TestMethod]
        public void TestParts()
        {
            var parts = TestBasicNS.TallyService.GetVoucherTDLParts();

            Assert.AreEqual(2, parts.Length);
            var part1 = parts[0];

            Assert.AreEqual(TestBasicNS.TallyService.VoucherReportName, part1.Name);

            var part2 = parts[1];
            Assert.AreEqual(TestBasicNS.TallyService.VoucherLedgerEntriesReportName, part2.Name);
        }
    }
}

namespace TestBasicNS
{
    [GenerateHelperMethod<Voucher>(GenerationMode = TallyConnector.Core.Models.GenerationMode.GetMultiple)]
    public partial class TallyService : TallyConnector.Services.BaseTallyService
    {
    }
    public class Voucher : TallyConnector.Core.Models.IBaseObject
    {
        [XmlElement(ElementName = "PARENT")]
        public string? Parent { get; set; }

        [TDLCollection(CollectionName = "LedgerEntries")]
        public List<LedgerEntry> LedgerEntries { get; set; } = [];

        [XmlElement(ElementName = "NAME")]
        public string? Name { get; set; }


    }

    public partial class LedgerEntry
    {
        [XmlElement(ElementName = "NAME")]
        public string? Name { get; set; }
    }
}
