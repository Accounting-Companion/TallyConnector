using TallyConnector.Core.Models.Masters;

namespace Tests.Converters.XMLConverterHelpers;
public class TallyAmountTests
{
    XmlAttributeOverrides xmlAttributeOverrides = new();
    XmlSerializer xmlSerializer;
    XmlWriterSettings settings;
    public TallyAmountTests()
    {
        XmlAttributes xmlAttributes = new() { XmlRoot = new XmlRootAttribute("AMOUNT") };

        xmlAttributeOverrides.Add(typeof(TallyAmount), xmlAttributes);
        xmlSerializer = new(typeof(TallyAmount), xmlAttributeOverrides);

        settings = new()
        {
            OmitXmlDeclaration = true,
            NewLineChars = "&#13;&#10;", //If /r/n in Xml replace
                                         //NewLineHandling = NewLineHandling.Entitize,
            Encoding = Encoding.Unicode,
            CheckCharacters = false,
        };
    }


    [Test]
    public void CheckTallyAmountConverterforCreditAmount()
    {
        string NormalAmountXml = "<AMOUNT>5000</AMOUNT>";

        using TextReader reader = new StringReader(NormalAmountXml);
        var Amount = (TallyAmount)xmlSerializer.Deserialize(reader);

        Assert.AreEqual(Amount.Amount, 5000);
        Assert.AreEqual(Amount.IsDebit, false);

    }
    [Test]
    public void CheckTallyAmountConverterforDebitAmount()
    {
        string NormalAmountXml = "<AMOUNT>-5000</AMOUNT>";

        using TextReader reader = new StringReader(NormalAmountXml);
        var Amount = (TallyAmount)xmlSerializer.Deserialize(reader);

        Assert.AreEqual(Amount.Amount, 5000);
        Assert.AreEqual(Amount.IsDebit, true);

    }
    [Test]
    public void CheckTallyAmountConverterforNoAmount()
    {
        string NormalAmountXml = "<AMOUNT></AMOUNT>";

        using TextReader reader = new StringReader(NormalAmountXml);
        var Amount = (TallyAmount)xmlSerializer.Deserialize(reader);

        Assert.AreEqual(Amount.Amount, 0);
        Assert.AreEqual(Amount.IsDebit, false);

    }
    [Test]
    public void CheckTallyAmountConverterforAmountwithForex()
    {
        string NormalAmountXml = "<AMOUNT>$ 4875.0000 @ ₹ 38342.6231/$  = ₹ 186920287.61</AMOUNT>";

        using TextReader reader = new StringReader(NormalAmountXml);
        var Amount = (TallyAmount)xmlSerializer.Deserialize(reader);

        Assert.AreEqual(Amount.Amount, 186920287.61);
        Assert.AreEqual(Amount.Currency, "$");
        Assert.AreEqual(Amount.IsDebit, false);
        Assert.AreEqual(Amount.ForexAmount, 4875.0000);
        Assert.AreEqual(Amount.RateOfExchange, 38342.6231);

    }
    [Test]
    public void CheckTallyAmountConverterforDebitAmountwithForex()
    {
        string NormalAmountXml = "<AMOUNT>-$ 10000.0000 @ ₹ 42.50/$  = -₹ 425000.00</AMOUNT>";

        using TextReader reader = new StringReader(NormalAmountXml);
        var Amount = (TallyAmount)xmlSerializer.Deserialize(reader);

        Assert.AreEqual(Amount.IsDebit, true);
        Assert.AreEqual(Amount.Amount, 425000.00);
        Assert.AreEqual(Amount.Currency, "$");
        Assert.AreEqual(Amount.ForexAmount, 10000.0000);
        Assert.AreEqual(Amount.RateOfExchange, 42.50);

    }

    [Test]
    public void CheckTallyAmountConverterforDebitAmountwithForexPartial()
    {
        string NormalAmountXml = "<AMOUNT>-$ 10000.0000 @  = -₹ 425000.00</AMOUNT>";

        using TextReader reader = new StringReader(NormalAmountXml);
        var Amount = (TallyAmount)xmlSerializer.Deserialize(reader);

        Assert.AreEqual(Amount.IsDebit, true);
        Assert.AreEqual(Amount.Amount, 425000.00);
        Assert.AreEqual(Amount.Currency, "$");

    }

    [Test]
    public void CheckConstructor()
    {
        TallyAmount tallyAmount = 5000;

        TextWriter textWriter = new StringWriter();
        var writer = XmlWriter.Create(textWriter, settings);
        xmlSerializer.Serialize(writer, tallyAmount);
        string xml = textWriter.ToString();

        Assert.AreEqual((decimal)tallyAmount, 5000);
        Assert.AreEqual(xml, "<AMOUNT>5000</AMOUNT>");
    }

    [Test]
    public void CheckConstructorDebitAmount()
    {
        TallyAmount tallyAmount = -5000;
        TextWriter textWriter = new StringWriter();
        var writer = XmlWriter.Create(textWriter, settings);
        xmlSerializer.Serialize(writer, tallyAmount);
        string xml = textWriter.ToString();

        //Assert.AreEqual((decimal)tallyAmount, 5000);
        Assert.AreEqual(xml, "<AMOUNT>-5000</AMOUNT>");
    }

    [Test]
    public void CheckConstructorwithForeignCurrency()
    {
        TallyAmount tallyAmount = new(500, 50, "$", false, 500 * 50);
        TextWriter textWriter = new StringWriter();
        var writer = XmlWriter.Create(textWriter, settings);
        xmlSerializer.Serialize(writer, tallyAmount);
        string xml = textWriter.ToString();

        Assert.AreEqual((decimal)tallyAmount, 25000);
        Assert.AreEqual(xml, "<AMOUNT>$ 500 @ 50</AMOUNT>");
    }

    [Test]
    public void CheckConstructorwithDebitForeignCurrency()
    {
        TallyAmount tallyAmount = new(-500, 50, "$", true, -500 * 50);
        TextWriter textWriter = new StringWriter();
        var writer = XmlWriter.Create(textWriter, settings);
        xmlSerializer.Serialize(writer, tallyAmount);
        string xml = textWriter.ToString();

        Assert.AreEqual((decimal)tallyAmount, 25000);
        Assert.AreEqual(xml, "<AMOUNT>-$ 500 @ 50</AMOUNT>");
    }


    [Test]
    public async Task CheckCreateLedgerTallyAmountwithNull()
    {
        TallyService tally = new();
        var ledgerName = "TesttoTallyAmount";
        Ledger ledger = new(ledgerName, "Sundry Debtors")
        {
            OpeningBal = null,
        };
        var resp = await tally.PostLedgerAsync(ledger);
        Assert.AreEqual(resp.Status, TCM.RespStatus.Sucess);

        var TLedger = await tally.GetLedgerAsync<Ledger>(ledgerName);
        Assert.AreEqual(TLedger.Name, ledgerName);
        Assert.AreEqual(TLedger.OpeningBal.Amount, 0);
        var delResp = await tally.PostLedgerAsync(new Ledger() { Action = TCM.Action.Delete, OldName = "TesttoTallyAmount" });

        Assert.AreEqual(delResp.Status, TCM.RespStatus.Sucess);
    }
    [Test]
    public async Task CheckCreateLedgerTallyAmount()
    {
        TallyService tally = new();
        var ledgerName = "TesttoTallyAmount";
        Ledger ledger = new(ledgerName, "Sundry Debtors")
        {
            OpeningBal = 5000,
        };
        var resp = await tally.PostLedgerAsync(ledger);
        Assert.AreEqual(resp.Status, TCM.RespStatus.Sucess);

        var TLedger = await tally.GetLedgerAsync<Ledger>(ledgerName);
        Assert.AreEqual(TLedger.Name, ledgerName);
        Assert.AreEqual(TLedger.OpeningBal.Amount, 5000);
        var delResp = await tally.PostLedgerAsync(new Ledger() { Action = TCM.Action.Delete, OldName = "TesttoTallyAmount" });

        Assert.AreEqual(delResp.Status, TCM.RespStatus.Sucess);
    }
    [Test]
    public async Task CheckCreateLedgerTallyDebitAmount()
    {
        TallyService tally = new();
        var ledgerName = "TesttoTallyAmount";
        Ledger ledger = new(ledgerName, "Sundry Debtors")
        {
            OpeningBal = -5000,
        };
        var resp = await tally.PostLedgerAsync(ledger);
        Assert.AreEqual(resp.Status, TCM.RespStatus.Sucess);

        var TLedger = await tally.GetLedgerAsync<Ledger>(ledgerName);
        Assert.AreEqual(TLedger.Name, ledgerName);
        Assert.AreEqual(TLedger.OpeningBal.Amount, 5000);
        var delResp = await tally.PostLedgerAsync(new Ledger() { Action = TCM.Action.Delete, OldName = "TesttoTallyAmount" });

        Assert.AreEqual(delResp.Status, TCM.RespStatus.Sucess);
    }

    [Test]
    public async Task CheckCreateLedgerTallyAmountwithForex()
    {
        TallyService tally = new();
        var ledgerName = "TesttoTallyAmount";
        Ledger ledger = new(ledgerName, "Sundry Debtors")
        {
            OpeningBal = new(5000, 20, "$"),
        };
        var resp = await tally.PostLedgerAsync(ledger);
        Assert.AreEqual(resp.Status, TCM.RespStatus.Sucess);

        var TLedger = await tally.GetLedgerAsync<Ledger>(ledgerName);
        Assert.AreEqual(TLedger.Name, ledgerName);
        Assert.AreEqual(TLedger.OpeningBal.Amount, 100000);
        Assert.AreEqual(TLedger.OpeningBal.ForexAmount, 5000);
        Assert.AreEqual(TLedger.OpeningBal.RateOfExchange, 20);
        Assert.AreEqual(TLedger.OpeningBal.IsDebit, false);
        Assert.AreEqual(TLedger.OpeningBal.Currency, "$");
        var delResp = await tally.PostLedgerAsync(new Ledger() { Action = TCM.Action.Delete, OldName = "TesttoTallyAmount" });

        Assert.AreEqual(delResp.Status, TCM.RespStatus.Sucess);
    }

    [Test]
    public async Task CheckCreateLedgerTallyDebitAmountwithForex()
    {
        TallyService tally = new();
        var ledgerName = "TesttoTallyAmount";
        Ledger ledger = new(ledgerName, "Sundry Debtors")
        {
            OpeningBal = new(-5000, 20, "$"),
        };
        var resp = await tally.PostLedgerAsync(ledger);
        Assert.AreEqual(resp.Status, TCM.RespStatus.Sucess);

        var TLedger = await tally.GetLedgerAsync<Ledger>(ledgerName);
        Assert.AreEqual(TLedger.Name, ledgerName);
        Assert.AreEqual(TLedger.OpeningBal.Amount, 100000);
        Assert.AreEqual(TLedger.OpeningBal.ForexAmount, 5000);
        Assert.AreEqual(TLedger.OpeningBal.RateOfExchange, 20);
        Assert.AreEqual(TLedger.OpeningBal.IsDebit, true);
        Assert.AreEqual(TLedger.OpeningBal.Currency, "$");
        var delResp = await tally.PostLedgerAsync(new Ledger() { Action = TCM.Action.Delete, OldName = "TesttoTallyAmount" });

        Assert.AreEqual(delResp.Status, TCM.RespStatus.Sucess);
    }

}

