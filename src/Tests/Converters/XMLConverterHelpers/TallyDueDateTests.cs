using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TallyConnector.Core.Converters.XMLConverterHelpers;

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
    public void CheckTallyDueDatewithDays()
    {
        string TallyDueDateXml = "<BILLCREDITPERIOD JD=\"44651\" P=\"1 Days\">1 Days</BILLCREDITPERIOD>";
        var billdate = new DateTime(2022, 4, 1);
        using TextReader reader = new StringReader(TallyDueDateXml);
        var dueDate = (TallyDueDate)xmlSerializer.Deserialize(reader);
        Assert.Multiple(() =>
        {
            //Assert.That(dueDate.BillDate, Is.EqualTo(new DateTime(2022, 4, 1)));
            Assert.That(dueDate.DueDate, Is.EqualTo(billdate.AddDays(1)));
            Assert.That(dueDate.Value, Is.EqualTo(1));
            Assert.That(dueDate.Suffix, Is.EqualTo(DueDateFormat.Day));
        });
    }
}
