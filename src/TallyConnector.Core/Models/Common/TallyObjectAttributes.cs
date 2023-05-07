using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TallyConnector.Core.Models.Common;
public class TallyObjectAttributes
{
    [XmlAttribute("NAME")]
    public string Name { get; set; }

    [XmlAttribute(AttributeName = "ISMODIFY")]
    public bool IsModify { get; set; }

    [XmlAttribute(AttributeName = "ISFIXED")]
    public bool IsFixed { get; set; }

    [XmlAttribute(AttributeName = "ISINITIALIZE")]
    public bool IsInitialize { get; set; }

    [XmlAttribute(AttributeName = "ISOPTION")]
    public bool IsOption { get; set; }

    [XmlAttribute(AttributeName = "ISINTERNAL")]
    public bool IsInternal { get; set; }

    [XmlElement("USE")]
    public string Use { get; set; }

    public void SetAttributes(bool ismodify = false,
                              bool isFixed = false,
                              bool isInitialize = false,
                              bool isOption = false,
                              bool isInternal = false)
    {
        IsModify = ismodify;
        IsFixed = isFixed;
        IsInitialize = isInitialize;
        IsOption = isOption;
        IsInternal = isInternal;
    }

}
