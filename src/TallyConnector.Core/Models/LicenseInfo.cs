using System.Text.RegularExpressions;

namespace TallyConnector.Core.Models;

[XmlRoot(ElementName = "LICENSEINFO")]
public class LicenseInfo : TallyXmlJson
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
    public TallyYesNo? IsAdmin { get; set; }

    [XmlElement(ElementName = "ISEDUCATIONALMODE")]
    [TDLField("$$LicenseInfo:IsEducationalMode")]
    public TallyYesNo? IsEducationalMode { get; set; }

    [XmlElement(ElementName = "ISSILVER")]
    [TDLField("$$LicenseInfo:IsAdmin")]
    public TallyYesNo? IsSilver { get; set; }

    [XmlElement(ElementName = "ISGOLD")]
    [TDLField("$$LicenseInfo:IsAdmin")]
    public TallyYesNo? IsGold { get; set; }

    [XmlElement(ElementName = "PLANNAME")]
    [TDLField("If $$LicenseInfo:IsEducationalMode Then \"Educational Version\" ELSE  If $$LicenseInfo:IsSilver Then \"Silver\" ELSE  If $$LicenseInfo:IsGold Then \"Gold\" else \"\"")]
    public string? PlanName { get; set; }

    [XmlElement(ElementName = "ISINDIAN")]
    [TDLField("$$LicenseInfo:IsIndian")]
    public TallyYesNo? IsIndian { get; set; }

    [XmlElement(ElementName = "ISREMOTEACCESSMODE")]
    [TDLField("$$LicenseInfo:IsRemoteAccessMode")]
    public TallyYesNo? IsRemoteAccessMode { get; set; }

    [XmlElement(ElementName = "ISLICCLIENTMODE")]
    [TDLField("$$LicenseInfo:IsLicClientMode")]
    public TallyYesNo? IsLicenseClientMode { get; set; }

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
    [TDLField(Constants.License)]
    public string? TallyVersion { get; set; }

    public string? TallyShortVersion => Regex.Replace(Regex.Match(TallyVersion, "[a-zA-Z. 0-9-]+").Value, "(\\s([a-zA-Z]+\\s)+)", "");
}
