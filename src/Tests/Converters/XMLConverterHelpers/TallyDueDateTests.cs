namespace Tests.Converters.XMLConverterHelpers;
public class TallyDueDateTests
{
    XmlAttributeOverrides xmlAttributeOverrides = new();
    XmlSerializer xmlSerializer;
    XmlWriterSettings settings;

    public TallyDueDateTests()
    {
        XmlAttributes xmlAttributes = new() { XmlRoot = new XmlRootAttribute("BILLCREDITPERIOD") };

        xmlAttributeOverrides.Add(typeof(TallyDueDate), xmlAttributes);
        xmlSerializer = new(typeof(TallyDueDate), xmlAttributeOverrides);

        settings = new()
        {
            OmitXmlDeclaration = true,
            //NewLineChars = "&#13;&#10;", //If /r/n in Xml replace
            //NewLineHandling = NewLineHandling.Entitize,
            Encoding = Encoding.Unicode,
            CheckCharacters = false,
        };
    }

    [Test]
    [TestCase(DueDateFormat.Day, 1)]
    [TestCase(DueDateFormat.Day, 5)]
    [TestCase(DueDateFormat.Month, 5)]
    [TestCase(DueDateFormat.Year, 1)]
    [TestCase(DueDateFormat.Year, 2)]
    [TestCase(DueDateFormat.Week, 1)]
    [TestCase(DueDateFormat.Week, 2)]
    public void CheckTallyDueDateDeSerialize(DueDateFormat dueDateFormat, int value)
    {
        string TallyDueDateXml = $"<BILLCREDITPERIOD JD=\"44651\" P=\"{value} Days\">{value} {dueDateFormat}s</BILLCREDITPERIOD>";
        var billdate = new DateTime(2022, 4, 1);
        using TextReader reader = new StringReader(TallyDueDateXml);
        var dueDate = (TallyDueDate)xmlSerializer.Deserialize(reader);
        Assert.Multiple(() =>
        {
            Assert.That(dueDate.DueDate, Is.EqualTo(dueDateFormat == DueDateFormat.Month ?
                billdate.AddMonths(value) : dueDateFormat == DueDateFormat.Week ?
                billdate.AddDays(value * 7) : dueDateFormat == DueDateFormat.Year ?
                billdate.AddYears(value) : billdate.AddDays(value)));
            Assert.That(dueDate.Value, Is.EqualTo(value));
            Assert.That(dueDate.Suffix, Is.EqualTo(dueDateFormat));
        });
    }

    [Test]
    [TestCase(2022, 1, 31, "d-MMM-yy")]
    [TestCase(2022, 1, 1, "d-MMM-yy")]
    [TestCase(2006, 1, 31, "d-MMM-yyyy")]
    [TestCase(2006, 1, 1, "d-MMM-yyyy")]
    public void CheckTallyDueDatewithDateDeSerialize(int year, int month, int day, string format)
    {
        var dt = new DateTime(year, month, day);
        string TallyDueDateXml = $"<BILLCREDITPERIOD JD=\"44651\" P=\"{dt.ToString(format)}\">{dt.ToString(format)}</BILLCREDITPERIOD>";

        using TextReader reader = new StringReader(TallyDueDateXml);
        var dueDate = (TallyDueDate)xmlSerializer.Deserialize(reader);
        Assert.Multiple(() =>
        {
            Assert.That(dueDate.DueDate, Is.EqualTo(new DateTime(year, month, day)));
            Assert.That(dueDate.Value, Is.EqualTo(0));
            Assert.That(dueDate.Suffix, Is.EqualTo(DueDateFormat.Date));
        });
    }

    [Test]
    public void CheckTallyDueDateSerializewithonlyDueDate()
    {
        TallyDueDate TallyDueDate = new DateTime(2022, 1, 1);
        TextWriter textWriter = new StringWriter();
        var writer = XmlWriter.Create(textWriter, settings);
        xmlSerializer.Serialize(writer, TallyDueDate);
        string xml = textWriter.ToString();
        Assert.That(xml, Is.EqualTo("<BILLCREDITPERIOD TYPE=\"Due Date\">01-Jan-2022</BILLCREDITPERIOD>"));
    }
    [Test]
    [TestCase(1, DueDateFormat.Day)]
    [TestCase(10, DueDateFormat.Day)]
    [TestCase(1, DueDateFormat.Month)]
    [TestCase(10, DueDateFormat.Month)]
    [TestCase(1, DueDateFormat.Week)]
    [TestCase(10, DueDateFormat.Week)]
    [TestCase(1, DueDateFormat.Year)]
    [TestCase(10, DueDateFormat.Year)]
    public void CheckTallyDueDateSerialize(int value, DueDateFormat dueDateFormat)
    {
        TallyDueDate TallyDueDate = new(value, dueDateFormat);
        TextWriter textWriter = new StringWriter();
        var writer = XmlWriter.Create(textWriter, settings);
        xmlSerializer.Serialize(writer, TallyDueDate);
        string xml = textWriter.ToString();
        Assert.That(xml, Is.EqualTo($"<BILLCREDITPERIOD TYPE=\"Due Date\">{value} {dueDateFormat}s</BILLCREDITPERIOD>"));
    }

}
