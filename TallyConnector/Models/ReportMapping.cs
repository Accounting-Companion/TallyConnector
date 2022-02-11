using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TallyConnector.Models;

public class ReportField
{
    public ReportField(string XmlTag)
    {
        FieldName = XmlTag;
    }
    public ReportField(string XmlTag, List<ReportField> subFields = null)
    {
        SubFields = subFields;
    }
    public string FieldName { get; set; }
    public List<ReportField> SubFields { get; set; } = new();

    public List<string> Atrributes { get; set; } = new();
}


