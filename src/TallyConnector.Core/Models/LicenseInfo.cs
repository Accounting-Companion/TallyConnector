using System.Diagnostics;
using System.Xml.Schema;

namespace TallyConnector.Core.Models;

[XmlRoot(ElementName = "LICENSEINFO")]
public class LicenseInfo : BaseObject
{
    [XmlElement(ElementName = "SERIALNUMBER")]
    [TDLField("$$LicenseInfo:SerialNumber")]
    public string? SerialNumber { get; set; }

    [XmlElement(ElementName = "REMOTESERIALNUMBER")]
    [TDLField("$$LicenseInfo:RemoteSerialNumber")]
    public string? RemoteSerialNumber { get; set; }

    [XmlElement(ElementName = "ACCOUNTID")]
    [TDLField("$$LicenseInfo:AccountID")]
    public string? AccountId { get; set; }

    [XmlElement(ElementName = "ADMINMAILID")]
    [TDLField("$$LicenseInfo:AdminEmailID")]
    public string? AdminMailId { get; set; }

    [XmlElement(ElementName = "ISADMIN")]
    [TDLField("$$LicenseInfo:IsAdmin")]
    public bool? IsAdmin { get; set; }

    [XmlElement(ElementName = "ISEDUCATIONALMODE")]
    [TDLField("$$LicenseInfo:IsEducationalMode")]
    public bool? IsEducationalMode { get; set; }

    [XmlElement(ElementName = "ISSILVER")]
    [TDLField("$$LicenseInfo:IsAdmin")]
    public bool? IsSilver { get; set; }

    [XmlElement(ElementName = "ISGOLD")]
    [TDLField("$$LicenseInfo:IsAdmin")]
    public bool? IsGold { get; set; }

    [XmlElement(ElementName = "PLANNAME")]
    [TDLField("If $$LicenseInfo:IsEducationalMode Then \"Educational Version\" ELSE  If $$LicenseInfo:IsSilver Then \"Silver\" ELSE  If $$LicenseInfo:IsGold Then \"Gold\" else \"\"")]
    public string? PlanName { get; set; }

    [XmlElement(ElementName = "ISINDIAN")]
    [TDLField("$$LicenseInfo:IsIndian")]
    public bool? IsIndian { get; set; }

    [XmlElement(ElementName = "ISREMOTEACCESSMODE")]
    [TDLField("$$LicenseInfo:IsRemoteAccessMode")]
    public bool? IsRemoteAccessMode { get; set; }

    [XmlElement(ElementName = "ISLICCLIENTMODE")]
    [TDLField("$$LicenseInfo:IsLicClientMode")]
    public bool? IsLicenseClientMode { get; set; }

    [XmlElement(ElementName = "APPLICATIONPATH")]
    [TDLField("$$SysInfo:ApplicationPath")]
    public string? ApplicationPath { get; set; }

    [XmlElement(ElementName = "DATAPATH")]
    [TDLField("##SVCurrentPath")]
    public string? DataPath { get; set; }

    [XmlElement(ElementName = "USERLEVEL")]
    [TDLField("$$cmpuserlevel")]
    public string? UserLevel { get; set; }

    [XmlElement(ElementName = "USERNAME")]
    [TDLField("$$cmpusername")]
    public string? UserName { get; set; }

    [XmlElement(ElementName = "TALLYVERSION")]
    public string? TallyVersion { get; set; }

    [XmlElement(ElementName = "TALLYSHORTVERSION")]
    public ShortVersion TallyShortVersion { get; set; } = null!;

    [XmlElement(ElementName = "ISTALLYPRIME")]
    public bool IsTallyPrime { get; set; }

    [XmlElement(ElementName = "ISTALLYPRIMEEDITLOG")]
    public bool IsTallyPrimeEditLog { get; set; }

    [XmlElement(ElementName = "ISTALLYPRIMESERVER")]
    public bool IsTallyPrimeServer { get; set; }
}
[Serializable]
[DebuggerDisplay("{ToString()}")]
public class ShortVersion : IXmlSerializable
{
    public ShortVersion()
    {
    }

    public ShortVersion(int majorVersion, decimal minorVersion)
    {
        MajorVersion = majorVersion;
        MinorVersion = minorVersion;
    }

    public int MajorVersion { get; private set; }
    public decimal MinorVersion { get; private set; }

    public XmlSchema? GetSchema()
    {
        return null;
    }

    public void ReadXml(XmlReader reader)
    {
        bool isEmptyElement = reader.IsEmptyElement;
        if (!isEmptyElement)
        {
            string content = reader.ReadElementContentAsString();
            if (content != null)
            {
                ShortVersion shortVersion = content;
                MajorVersion = shortVersion.MajorVersion;
                MinorVersion = shortVersion.MinorVersion;
            }
        }
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteString(ToString());
    }
    public override string ToString()
    {
        return $"{MajorVersion}.{MinorVersion}";
    }
    public static implicit operator ShortVersion(string version)
    {
        var strings = version.Split(['.'], count: 2);
        int length = strings.Length;
        int _majorVersion = 0;
        decimal _minorVersion = 0;
        if (length > 0)
        {
            _ = int.TryParse(strings[0], out _majorVersion);
            if (length > 1)
            {
                _ = decimal.TryParse(strings[1], out _minorVersion);
            }
        }
        return new(_majorVersion, _minorVersion);
    }
}
