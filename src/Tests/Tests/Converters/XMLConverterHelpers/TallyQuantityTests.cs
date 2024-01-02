namespace Tests.Converters.XMLConverterHelpers;
internal class TallyQuantityTests
{
    XmlAttributeOverrides xmlAttributeOverrides = new();
    XmlSerializer xmlSerializer;
    XmlWriterSettings settings;

    public TallyQuantityTests()
    {
        XmlAttributes xmlAttributes = new() { XmlRoot = new XmlRootAttribute("QUANTITY") };

        xmlAttributeOverrides.Add(typeof(TallyQuantity), xmlAttributes);
        xmlSerializer = new(typeof(TallyQuantity), xmlAttributeOverrides);

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
    public void CheckTallyQuantitywithNoValue()
    {
        string TallyQuantitytXml = "<QUANTITY></QUANTITY>";

        using TextReader reader = new StringReader(TallyQuantitytXml);
        var quantity = (TallyQuantity)xmlSerializer.Deserialize(reader);
        Assert.Multiple(() =>
        {
            Assert.That(quantity.Number, Is.EqualTo(0));
            Assert.That(quantity.PrimaryUnits, Is.EqualTo(null));
            Assert.That(quantity.SecondaryUnits, Is.EqualTo(null));
        });
    }

    [Test]
    public void CheckTallyQuantitywithNoValuevariant2()
    {
        string TallyQuantitytXml = "<QUANTITY/>";

        using TextReader reader = new StringReader(TallyQuantitytXml);
        var quantity = (TallyQuantity)xmlSerializer.Deserialize(reader);
        Assert.Multiple(() =>
        {
            Assert.That(quantity.Number, Is.EqualTo(0));
            Assert.That(quantity.PrimaryUnits, Is.EqualTo(null));
            Assert.That(quantity.SecondaryUnits, Is.EqualTo(null));
        });
    }

    [Test]
    public void CheckTallyQuantity()
    {
        string TallyQuantitytXml = "<QUANTITY> 50 Nos</QUANTITY>";

        using TextReader reader = new StringReader(TallyQuantitytXml);
        var quantity = (TallyQuantity)xmlSerializer.Deserialize(reader);


        Assert.Multiple(() =>
        {
            Assert.That(quantity.Number, Is.EqualTo(50));
            Assert.That(quantity.PrimaryUnits.Number, Is.EqualTo(50));
            Assert.That(quantity.PrimaryUnits.Unit, Is.EqualTo("Nos"));
            Assert.That(quantity.SecondaryUnits, Is.EqualTo(null));
        });
    }

    [Test]
    public void CheckTallyQuantitywithSecondaryUnit()
    {
        string TallyQuantitytXml = "<QUANTITY> 1000 Nos =  100 Boxes</QUANTITY>";

        using TextReader reader = new StringReader(TallyQuantitytXml);
        var quantity = (TallyQuantity)xmlSerializer.Deserialize(reader);

        Assert.Multiple(() =>
        {
            Assert.That(quantity.Number, Is.EqualTo(1000));
            Assert.That(quantity.PrimaryUnits.Number, Is.EqualTo(1000));
            Assert.That(quantity.PrimaryUnits.Unit, Is.EqualTo("Nos"));
            Assert.That(quantity.SecondaryUnits.Number, Is.EqualTo(100));
            Assert.That(quantity.SecondaryUnits.Unit, Is.EqualTo("Boxes"));
        });
    }
    [Test]
    public void CheckTallyQuantityNull()
    {
        string TallyQuantitytXml = "<QUANTITY xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xsi:nil=\"true\" />";

        TallyQuantity tallyQuantity = null;
        TextWriter textWriter = new StringWriter();
        var writer = XmlWriter.Create(textWriter, settings);
        xmlSerializer.Serialize(writer, tallyQuantity);
        string outxml = textWriter.ToString();
        Assert.That(outxml, Is.EqualTo(TallyQuantitytXml));
    }
    [Test]
    public void CheckTallyQuantityConstructor()
    {
        string TallyQuantitytXml = "<QUANTITY>1000 Nos</QUANTITY>";

        TallyQuantity tallyQuantity = new(1000, "Nos");
        TextWriter textWriter = new StringWriter();
        var writer = XmlWriter.Create(textWriter, settings);
        xmlSerializer.Serialize(writer, tallyQuantity);
        string outxml = textWriter.ToString();

        Assert.Multiple(() =>
        {
            Assert.That(tallyQuantity.Number, Is.EqualTo(1000));
            Assert.That(tallyQuantity.PrimaryUnits.Number, Is.EqualTo(1000));
            Assert.That(tallyQuantity.PrimaryUnits.Unit, Is.EqualTo("Nos"));
            Assert.That(outxml, Is.EqualTo(TallyQuantitytXml));

        });
    }
    [Test]
    public void CheckTallyQuantityConstructorwithStockItem()
    {
        string TallyQuantitytXml = "<QUANTITY>1000 Nos</QUANTITY>";

        TallyQuantity tallyQuantity = new(new() { BaseUnit = "Nos" }, 1000);
        TextWriter textWriter = new StringWriter();
        var writer = XmlWriter.Create(textWriter, settings);
        xmlSerializer.Serialize(writer, tallyQuantity);
        string outxml = textWriter.ToString();
        Assert.Multiple(() =>
        {
            Assert.That(tallyQuantity.Number, Is.EqualTo(1000));
            Assert.That(tallyQuantity.PrimaryUnits.Number, Is.EqualTo(1000));
            Assert.That(tallyQuantity.PrimaryUnits.Unit, Is.EqualTo("Nos"));
            Assert.That(outxml, Is.EqualTo(TallyQuantitytXml));
        });
    }

    [Test]
    public void CheckTallyQuantityConstructorwithStockItemandAdditionalUnits()
    {
        string TallyQuantitytXml = "<QUANTITY>1000 Nos</QUANTITY>";

        TallyQuantity tallyQuantity = new(new() { BaseUnit = "Nos", AdditionalUnits = "Boxes" }, 1000);
        TextWriter textWriter = new StringWriter();
        var writer = XmlWriter.Create(textWriter, settings);
        xmlSerializer.Serialize(writer, tallyQuantity);
        string outxml = textWriter.ToString();
        Assert.Multiple(() =>
        {
            Assert.That(tallyQuantity.Number, Is.EqualTo(1000));
            Assert.That(tallyQuantity.PrimaryUnits.Number, Is.EqualTo(1000));
            Assert.That(tallyQuantity.PrimaryUnits.Unit, Is.EqualTo("Nos"));
            Assert.That(tallyQuantity.SecondaryUnits, Is.EqualTo(null));
            Assert.That(outxml, Is.EqualTo(TallyQuantitytXml));
        });
    }

    [Test]
    public void CheckTallyQuantityConstructorwithStockItemandAdditionalUnitsandConversion()
    {
        string TallyQuantitytXml = "<QUANTITY>1000 Nos = 70.00 Boxes</QUANTITY>";

        TallyQuantity tallyQuantity = new(new() { BaseUnit = "Nos", AdditionalUnits = "Boxes", Conversion = 1, Denominator = 15 }, 1000);
        TextWriter textWriter = new StringWriter();
        var writer = XmlWriter.Create(textWriter, settings);
        xmlSerializer.Serialize(writer, tallyQuantity);
        string outxml = textWriter.ToString();
        Assert.Multiple(() =>
        {
            Assert.That(tallyQuantity.Number, Is.EqualTo(1000));
            Assert.That(tallyQuantity.PrimaryUnits.Number, Is.EqualTo(1000));
            Assert.That(tallyQuantity.PrimaryUnits.Unit, Is.EqualTo("Nos"));
            Assert.That(tallyQuantity.SecondaryUnits.Number, Is.EqualTo(70));
            Assert.That(tallyQuantity.SecondaryUnits.Unit, Is.EqualTo("Boxes"));
            Assert.That(outxml, Is.EqualTo(TallyQuantitytXml));
        });
    }
}
