using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TallyConnector.Core.Attributes;
using TallyConnector.Core.Models.Interfaces.Masters;
using TallyConnector.Core.Models.Interfaces.Voucher;
using TallyConnector.Services;

namespace IntegrationTests.Models;
[TestClass]
public class TestSimple
{
    [TestMethod]
    public async Task Testr()
    {
        var resp = await new VTallyService().GetVouchersAsync(new TallyConnector.Core.Models.RequestOptions());
    }
    [TestMethod]
    public async Task Testr2()
    {
        var resp = await new VTallyService().GetTBLedgersAsync(new TallyConnector.Core.Models.RequestOptions());
    }
}

[GenerateHelperMethod<Voucher>]
[GenerateHelperMethod<TBLedger>]
public partial class VTallyService : BaseTallyService
{

}
public class Voucher : IBaseVoucherObject
{
    [XmlElement("DATE")]
    public DateTime Date { get; set; }
    [XmlElement("VOUCHERTYPENAME")]
    public string VoucherNumber { get; set; }
    [XmlElement("VOUCHERNUMBER")]
    public string VoucherType { get; set; }
    [XmlElement("MASTERID")]
    public string MasterId { get; set; }
    [XmlElement("TOTALLEDDRVCHAMT")]
    public decimal DebitAmount { get; set; }
    [XmlElement("TOTALLEDCRVCHAMT")]
    public decimal CreditAmount { get; set; }
}


public class TBLedger : IBaseMasterObject
{
    public string LedgerName { get; set; }
    public decimal OpeningBal { get; set; }
    public decimal ClosingBal { get; set; }
    public decimal Debits { get; set; }
    public decimal Credits { get; set; }
}