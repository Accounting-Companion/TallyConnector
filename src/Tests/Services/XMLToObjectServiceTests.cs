using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TallyConnector.Core.Models;

namespace Tests.Services;
public class XMLToObjectServiceTests
{
    [Test]
    public void TestXMLToObject()
    {
        string xml = File.ReadAllText("C:\\Users\\talla\\Downloads\\Untitled-1.xml");
        XmlAttributeOverrides XMLAttributeOverrides = new();
        XmlAttributes attrs = new();
        attrs.XmlElements.Add(new("VOUCHER"));
        XMLAttributeOverrides.Add(typeof(Colllection<Voucher>), "Objects", attrs);
        var voucher = XMLToObject.GetObjfromXml<Envelope<Voucher>>(xml, XMLAttributeOverrides);
    }
}
