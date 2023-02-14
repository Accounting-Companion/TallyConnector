using System.Text.RegularExpressions;

namespace TallyConnector.Core.Models;

[XmlRoot(ElementName = "LICENSEINFO")]
public class LicenseInfo : TallyXmlJson
{
    [XmlElement(ElementName = "SERIALNUMBER")]
    [XMLTDLField("$$LicenseInfo:SerialNumber")]
    public string? SerialNumber { get; set; }

    [XmlElement(ElementName = "REMOTESERIALNUMBER")]
    [XMLTDLField("$$LicenseInfo:RemoteSerialNumber")]
    public string? RemoteSerialNumber { get; set; }

    [XmlElement(ElementName = "ACCOUNTID")]
    [XMLTDLField("$$LicenseInfo:AccountID")]
    public string? AccountId { get; set; }

    [XmlElement(ElementName = "ADMINMAILID")]
    [XMLTDLField("$$LicenseInfo:AdminEmailID")]
    public string? AdminMailId { get; set; }

    [XmlElement(ElementName = "ISADMIN")]
    [XMLTDLField("$$LicenseInfo:IsAdmin")]
    public TallyYesNo? IsAdmin { get; set; }

    [XmlElement(ElementName = "ISEDUCATIONALMODE")]
    [XMLTDLField("$$LicenseInfo:IsEducationalMode")]
    public TallyYesNo? IsEducationalMode { get; set; }

    [XmlElement(ElementName = "ISSILVER")]
    [XMLTDLField("$$LicenseInfo:IsAdmin")]
    public TallyYesNo? IsSilver { get; set; }

    [XmlElement(ElementName = "ISGOLD")]
    [XMLTDLField("$$LicenseInfo:IsAdmin")]
    public TallyYesNo? IsGold { get; set; }

    [XmlElement(ElementName = "PLANNAME")]
    [XMLTDLField("If $$LicenseInfo:IsEducationalMode Then \"Educational Version\" ELSE  If $$LicenseInfo:IsSilver Then \"Silver\" ELSE  If $$LicenseInfo:IsGold Then \"Gold\" else \"\"")]
    public string? PlanName { get; set; }

    [XmlElement(ElementName = "ISINDIAN")]
    [XMLTDLField("$$LicenseInfo:IsAdmin")]
    public TallyYesNo? IsIndian { get; set; }

    [XmlElement(ElementName = "ISREMOTEACCESSMODE")]
    [XMLTDLField("$$LicenseInfo:IsRemoteAccessMode")]
    public TallyYesNo? IsRemoteAccessMode { get; set; }

    [XmlElement(ElementName = "ISLICCLIENTMODE")]
    [XMLTDLField("$$LicenseInfo:IsLicClientMode")]
    public TallyYesNo? IsLicenseClientMode { get; set; }

    [XmlElement(ElementName = "APPLICATIONPATH")]
    [XMLTDLField("$$SysInfo:ApplicationPath")]
    public string? ApplicationPath { get; set; }

    [XmlElement(ElementName = "DATAPATH")]
    [XMLTDLField("##SVCurrentPath")]
    public string? DataPath { get; set; }

    [XmlElement(ElementName = "USERLEVEL")]
    [XMLTDLField("$$cmpuserlevel")]
    public string? UserLevel { get; set; }

    [XmlElement(ElementName = "USERNAME")]
    [XMLTDLField("$$cmpusername")]
    public string? UserName { get; set; }

    [XmlElement(ElementName = "TALLYVERSION")]
    [XMLTDLField(Constants.License)]
    public string? TallyVersion { get; set; }

    public string? TallyShortVersion => Regex.Replace(Regex.Match(TallyVersion, "[a-zA-Z. 0-9-]+").Value, "(\\s([a-zA-Z]+\\s)+)", "");
}


[XmlRoot(ElementName = "ENVELOPE")]
public class LicInfoEnvelope : TallyXmlJson
{

    [XmlElement(ElementName = "HEADER")]
    public Header? Header { get; set; }

    [XmlElement(ElementName = "BODY")]
    public LicInfoBody Body { get; set; } = new LicInfoBody();
}

[XmlRoot(ElementName = "BODY")]
public class LicInfoBody
{
    [XmlElement(ElementName = "DESC")]
    public Description Desc { get; set; } = new Description();

    [XmlElement(ElementName = "DATA")]
    public LicInfoData Data { get; set; } = new LicInfoData();
}

[XmlRoot(ElementName = "DATA")]
public class LicInfoData
{
    [XmlElement(ElementName = "COLLECTION")]
    public LicInfoColloction Collection { get; set; } = new LicInfoColloction();
}

[XmlRoot(ElementName = "COLLECTION")]
public class LicInfoColloction
{
    [XmlElement(ElementName = "LICENSEINFO")]
    public LicenseInfo? LicenseInfo { get; set; }

}