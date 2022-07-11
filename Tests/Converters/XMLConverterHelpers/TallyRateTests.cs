namespace Tests.Converters.XMLConverterHelpers;
internal class TallyRateTests
{
    XmlAttributeOverrides xmlAttributeOverrides = new();
    XmlSerializer xmlSerializer;
    XmlWriterSettings settings;
    public TallyRateTests()
    {
        XmlAttributes xmlAttributes = new() { XmlRoot = new XmlRootAttribute("RATE") };

        xmlAttributeOverrides.Add(typeof(TallyRate), xmlAttributes);
        xmlSerializer = new(typeof(TallyRate), xmlAttributeOverrides);

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
    public void CheckTallyRatewhenNull()
    {
        string TallyRateXml = "<RATE></RATE>";

        using TextReader reader = new StringReader(TallyRateXml);
        var rate = (TallyRate)xmlSerializer.Deserialize(reader);

        Assert.That(rate.RatePerUnit, Is.EqualTo(0));

    }
    [Test]
    public void CheckTallyRatewhenNullVariant2()
    {
        string TallyRateXml = "<RATE/>";

        using TextReader reader = new StringReader(TallyRateXml);
        var rate = (TallyRate)xmlSerializer.Deserialize(reader);

        Assert.That(rate.RatePerUnit, Is.EqualTo(0));

    }
    [Test]
    public void CheckTallyRate()
    {
        string TallyRateXml = "<RATE>8500.00/Nos</RATE>";

        using TextReader reader = new StringReader(TallyRateXml);
        var rate = (TallyRate)xmlSerializer.Deserialize(reader);
        Assert.Multiple(() =>
        {
            Assert.That(rate.RatePerUnit, Is.EqualTo(8500));
            Assert.That(rate.Unit, Is.EqualTo("Nos"));
        });
    }

    [Test]
    public void CheckTallyRateWithForeignCurrency()
    {
        string TallyRateXml = "<RATE>$ 200.0000 = ₹ 8500.00/Nos</RATE>";

        using TextReader reader = new StringReader(TallyRateXml);
        var rate = (TallyRate)xmlSerializer.Deserialize(reader);
        Assert.Multiple(() =>
        {
            Assert.That(rate.RatePerUnit, Is.EqualTo(8500));
            Assert.That(rate.ForexAmount, Is.EqualTo(200));
            Assert.That(rate.ForeignCurrency, Is.EqualTo("$"));
            Assert.That(rate.Unit, Is.EqualTo("Nos"));
        });
    }

    [Test]
    public void CheckTallyRateConstructor()
    {
        string TallyRateXml = "<RATE>8500/Nos</RATE>";

        TallyRate tallyRate = new(8500, "Nos");
        TextWriter textWriter = new StringWriter();
        var writer = XmlWriter.Create(textWriter, settings);
        xmlSerializer.Serialize(writer, tallyRate);
        string outxml = textWriter.ToString();
        Assert.Multiple(() =>
        {
            Assert.That(tallyRate.RatePerUnit, Is.EqualTo(8500));
            Assert.That(tallyRate.Unit, Is.EqualTo("Nos"));
            Assert.That(outxml, Is.EqualTo(TallyRateXml));
        });
    }

    [Test]
    public void CheckTallyRateConstructorwithForeignCurrency()
    {
        string TallyRateXml = "<RATE>$ 200 = 8500.0/Nos</RATE>";

        TallyRate tallyRate = new("Nos", 200, (decimal)42.50, "$");
        TextWriter textWriter = new StringWriter();
        var writer = XmlWriter.Create(textWriter, settings);
        xmlSerializer.Serialize(writer, tallyRate);
        string outxml = textWriter.ToString();

        Assert.Multiple(() =>
        {
            Assert.That(tallyRate.RatePerUnit, Is.EqualTo(8500));
            Assert.That(tallyRate.Unit, Is.EqualTo("Nos"));
            Assert.That(outxml, Is.EqualTo(TallyRateXml));
        });
    }
}
