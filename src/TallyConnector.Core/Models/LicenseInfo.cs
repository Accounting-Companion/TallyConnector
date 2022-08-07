namespace TallyConnector.Core.Models;

[XmlRoot(ElementName = "LICENSEINFO")]
public class LicenseInfo : TallyXmlJson
{
    [XmlElement(ElementName = "SERIALNUMBER")]
    [TDLXMLSet("$$LicenseInfo:SerialNumber")]
    public string? SerialNumber { get; set; }

    [XmlElement(ElementName = "REMOTESERIALNUMBER")]
    [TDLXMLSet("$$LicenseInfo:RemoteSerialNumber")]
    public string? RemoteSerialNumber { get; set; }

    [XmlElement(ElementName = "ACCOUNTID")]
    [TDLXMLSet("$$LicenseInfo:AccountID")]
    public string? AccountId { get; set; }

    [XmlElement(ElementName = "ADMINMAILID")]
    [TDLXMLSet("$$LicenseInfo:AdminEmailID")]
    public string? AdminMailId { get; set; }

    [XmlElement(ElementName = "ISADMIN")]
    [TDLXMLSet("$$LicenseInfo:IsAdmin")]
    public TallyYesNo? IsAdmin { get; set; }

    [XmlElement(ElementName = "ISEDUCATIONALMODE")]
    [TDLXMLSet("$$LicenseInfo:IsEducationalMode")]
    public TallyYesNo? IsEducationalMode { get; set; }

    [XmlElement(ElementName = "ISSILVER")]
    [TDLXMLSet("$$LicenseInfo:IsAdmin")]
    public TallyYesNo? IsSilver { get; set; }

    [XmlElement(ElementName = "ISGOLD")]
    [TDLXMLSet("$$LicenseInfo:IsAdmin")]
    public TallyYesNo? IsGold { get; set; }

    [XmlElement(ElementName = "PLANNAME")]
    [TDLXMLSet("If $$LicenseInfo:IsEducationalMode Then \"Educational Version\" ELSE  If $$LicenseInfo:IsSilver Then \"Silver\" ELSE  If $$LicenseInfo:IsGold Then \"Gold\" else \"\"")]
    public string? PlanName { get; set; }

    [XmlElement(ElementName = "ISINDIAN")]
    [TDLXMLSet("$$LicenseInfo:IsAdmin")]
    public TallyYesNo? IsIndian { get; set; }

    [XmlElement(ElementName = "ISREMOTEACCESSMODE")]
    [TDLXMLSet("$$LicenseInfo:IsRemoteAccessMode")]
    public TallyYesNo? IsRemoteAccessMode { get; set; }

    [XmlElement(ElementName = "ISLICCLIENTMODE")]
    [TDLXMLSet("$$LicenseInfo:IsLicClientMode")]
    public TallyYesNo? IsLicenseClientMode { get; set; }

    [XmlElement(ElementName = "APPLICATIONPATH")]
    [TDLXMLSet("$$SysInfo:ApplicationPath")]
    public string? ApplicationPath { get; set; }

    [XmlElement(ElementName = "DATAPATH")]
    [TDLXMLSet("##SVCurrentPath")]
    public string? DataPath { get; set; }

    [XmlElement(ElementName = "USERLEVEL")]
    [TDLXMLSet("$$cmpuserlevel")]
    public string? UserLevel { get; set; }

    [XmlElement(ElementName = "USERNAME")]
    [TDLXMLSet("$$cmpusername")]
    public string? UserName { get; set; }

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