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
        Assert.Multiple(() =>
        {
            Assert.That(Amount.Amount, Is.EqualTo(5000));
            Assert.That(Amount.IsDebit, Is.EqualTo(false));
        });
    }

    [Test]
    public void CheckTallyAmountConverterforDebitAmount()
    {
        string NormalAmountXml = "<AMOUNT>-5000</AMOUNT>";

        using TextReader reader = new StringReader(NormalAmountXml);
        var Amount = (TallyAmount)xmlSerializer.Deserialize(reader);
        Assert.Multiple(() =>
        {
            Assert.That(Amount.Amount, Is.EqualTo(5000));
            Assert.That(Amount.IsDebit, Is.EqualTo(true));
        });
    }

    [Test]
    public void CheckTallyAmountConverterforNoAmount()
    {
        string NormalAmountXml = "<AMOUNT></AMOUNT>";

        using TextReader reader = new StringReader(NormalAmountXml);
        var Amount = (TallyAmount)xmlSerializer.Deserialize(reader);
        Assert.Multiple(() =>
        {
            Assert.That(0, Is.EqualTo(Amount.Amount));
            Assert.That(Amount.IsDebit, Is.EqualTo(false));
        });
    }

    [Test]
    public void CheckTallyAmountConverterforAmountwithForex()
    {
        string NormalAmountXml = "<AMOUNT>$ 4875.0000 @ ₹ 38342.6231/$  = ₹ 186920287.61</AMOUNT>";

        using TextReader reader = new StringReader(NormalAmountXml);
        var Amount = (TallyAmount)xmlSerializer.Deserialize(reader);
        Assert.Multiple(() =>
        {
            Assert.That(Amount.Amount, Is.EqualTo(186920287.61));
            Assert.That(Amount.Currency, Is.EqualTo("$"));
            Assert.That(Amount.IsDebit, Is.EqualTo(false));
            Assert.That(Amount.ForexAmount, Is.EqualTo(4875.0000));
            Assert.That(Amount.RateOfExchange, Is.EqualTo(38342.6231));
        });
    }

    [Test]
    public void CheckTallyAmountConverterforDebitAmountwithForex()
    {
        string NormalAmountXml = "<AMOUNT>-$ 10000.0000 @ ₹ 42.50/$  = -₹ 425000.00</AMOUNT>";

        using TextReader reader = new StringReader(NormalAmountXml);
        var Amount = (TallyAmount)xmlSerializer.Deserialize(reader);
        Assert.Multiple(() =>
        {
            Assert.That(Amount.IsDebit, Is.EqualTo(true));
            Assert.That(Amount.Amount, Is.EqualTo(425000.00));
            Assert.That(Amount.Currency, Is.EqualTo("$"));
            Assert.That(Amount.ForexAmount, Is.EqualTo(10000.0000));
            Assert.That(Amount.RateOfExchange, Is.EqualTo(42.50));
        });
    }

    [Test]
    public void CheckTallyAmountConverterforDebitAmountwithForexPartial()
    {
        string NormalAmountXml = "<AMOUNT>-$ 10000.0000 @  = -₹ 425000.00</AMOUNT>";

        using TextReader reader = new StringReader(NormalAmountXml);
        var Amount = (TallyAmount)xmlSerializer.Deserialize(reader);
        Assert.Multiple(() =>
        {
            Assert.That(Amount.IsDebit, Is.EqualTo(true));
            Assert.That(Amount.Amount, Is.EqualTo(425000.00));
            Assert.That(Amount.Currency, Is.EqualTo("$"));
        });
    }

    [Test]
    public void CheckConstructor()
    {
        TallyAmount tallyAmount = 5000;

        TextWriter textWriter = new StringWriter();
        var writer = XmlWriter.Create(textWriter, settings);
        xmlSerializer.Serialize(writer, tallyAmount);
        string xml = textWriter.ToString();

        Assert.That((decimal)tallyAmount, Is.EqualTo(5000));
        Assert.That(xml, Is.EqualTo("<AMOUNT>5000</AMOUNT>"));
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
        Assert.That(xml, Is.EqualTo("<AMOUNT>-5000</AMOUNT>"));
    }

    [Test]
    public void CheckConstructorwithForeignCurrency()
    {
        TallyAmount tallyAmount = new(500, 50, "$", false, 500 * 50);
        TextWriter textWriter = new StringWriter();
        var writer = XmlWriter.Create(textWriter, settings);
        xmlSerializer.Serialize(writer, tallyAmount);
        string xml = textWriter.ToString();
        Assert.Multiple(() =>
        {
            Assert.That((decimal)tallyAmount, Is.EqualTo(25000));
            Assert.That(xml, Is.EqualTo("<AMOUNT>$ 500 @ 50</AMOUNT>"));
        });
    }

    [Test]
    public void CheckConstructorwithDebitForeignCurrency()
    {
        TallyAmount tallyAmount = new(-500, 50, "$", true, -500 * 50);
        TextWriter textWriter = new StringWriter();
        var writer = XmlWriter.Create(textWriter, settings);
        xmlSerializer.Serialize(writer, tallyAmount);
        string xml = textWriter.ToString();
        Assert.Multiple(() =>
        {
            Assert.That((decimal)tallyAmount, Is.EqualTo(25000));
            Assert.That(xml, Is.EqualTo("<AMOUNT>-$ 500 @ 50</AMOUNT>"));
        });
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

        Assert.That(resp.Status, Is.EqualTo(TCM.RespStatus.Sucess));

        var TLedger = await tally.GetLedgerAsync<Ledger>(ledgerName);
        Assert.Multiple(() =>
        {
            Assert.That(TLedger.Name, Is.EqualTo(ledgerName));
            Assert.That(TLedger.OpeningBal.Amount, Is.EqualTo(0));
        });
        var delResp = await tally.PostLedgerAsync(new Ledger() { Action = TCM.Action.Delete, OldName = "TesttoTallyAmount" });

        Assert.That(delResp.Status, Is.EqualTo(TCM.RespStatus.Sucess));
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
        Assert.That(resp.Status, Is.EqualTo(TCM.RespStatus.Sucess));

        var TLedger = await tally.GetLedgerAsync<Ledger>(ledgerName);
        Assert.Multiple(() =>
        {
            Assert.That(TLedger.Name, Is.EqualTo(ledgerName));
            Assert.That(TLedger.OpeningBal.Amount, Is.EqualTo(5000));
        });
        var delResp = await tally.PostLedgerAsync(new Ledger() { Action = TCM.Action.Delete, OldName = "TesttoTallyAmount" });

        Assert.That(delResp.Status, Is.EqualTo(TCM.RespStatus.Sucess));
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
        Assert.That(resp.Status, Is.EqualTo(TCM.RespStatus.Sucess));

        var TLedger = await tally.GetLedgerAsync<Ledger>(ledgerName);
        Assert.Multiple(() =>
        {
            Assert.That(TLedger.Name, Is.EqualTo(ledgerName));
            Assert.That(TLedger.OpeningBal.Amount, Is.EqualTo(5000));
        });
        var delResp = await tally.PostLedgerAsync(new Ledger() { Action = TCM.Action.Delete, OldName = "TesttoTallyAmount" });

        Assert.That(delResp.Status, Is.EqualTo(TCM.RespStatus.Sucess));
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
        Assert.That(resp.Status, Is.EqualTo(TCM.RespStatus.Sucess));

        var TLedger = await tally.GetLedgerAsync<Ledger>(ledgerName);
        Assert.Multiple(() =>
        {
            Assert.That(TLedger.Name, Is.EqualTo(ledgerName));
            Assert.That(TLedger.OpeningBal.Amount, Is.EqualTo(100000));
            Assert.That(TLedger.OpeningBal.ForexAmount, Is.EqualTo(5000));
            Assert.That(TLedger.OpeningBal.RateOfExchange, Is.EqualTo(20));
            Assert.That(TLedger.OpeningBal.IsDebit, Is.EqualTo(false));
            Assert.That(TLedger.OpeningBal.Currency, Is.EqualTo("$"));
        });
        var delResp = await tally.PostLedgerAsync(new Ledger() { Action = TCM.Action.Delete, OldName = "TesttoTallyAmount" });

        Assert.That(delResp.Status, Is.EqualTo(TCM.RespStatus.Sucess));
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
        Assert.That(resp.Status, Is.EqualTo(TCM.RespStatus.Sucess));

        var TLedger = await tally.GetLedgerAsync<Ledger>(ledgerName);
        Assert.Multiple(() =>
        {
            Assert.That(TLedger.Name, Is.EqualTo(ledgerName));
            Assert.That(TLedger.OpeningBal.Amount, Is.EqualTo(100000));
            Assert.That(TLedger.OpeningBal.ForexAmount, Is.EqualTo(5000));
            Assert.That(TLedger.OpeningBal.RateOfExchange, Is.EqualTo(20));
            Assert.That(TLedger.OpeningBal.IsDebit, Is.EqualTo(true));
            Assert.That(TLedger.OpeningBal.Currency, Is.EqualTo("$"));
        });
        var delResp = await tally.PostLedgerAsync(new Ledger() { Action = TCM.Action.Delete, OldName = "TesttoTallyAmount" });

        Assert.That(delResp.Status, Is.EqualTo(TCM.RespStatus.Sucess));
    }
}

