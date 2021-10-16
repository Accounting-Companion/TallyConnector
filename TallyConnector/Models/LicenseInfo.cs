using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TallyConnector.Models
{
    [XmlRoot(ElementName = "LICENSEINFO")]
    public class LicenseInfo:TallyXmlJson
    {
        [XmlElement(ElementName = "SERIALNUMBER")]
        public string SerialNumber { get; set; }

        [XmlElement(ElementName = "REMOTESERIALNUMBER")]
        public string RemoteSerialNumber { get; set; }

        [XmlElement(ElementName = "ACCOUNTID")]
        public string AccountId { get; set; }

        [XmlElement(ElementName = "ADMINMAILID")]
        public string AdminMailId { get; set; }

        [XmlElement(ElementName = "ISADMIN")]
        public string IsAdmin { get; set; }

        [XmlElement(ElementName = "ISEDUCATIONALMODE")]
        public string IsEducationalMode { get; set; }

        [XmlElement(ElementName = "ISSILVER")]
        public string IsSilver { get; set; }

        [XmlElement(ElementName = "ISGOLD")]
        public string IsGold { get; set; }

        [XmlElement(ElementName = "PLANNAME")]
        public string PlanName { get; set; }

        [XmlElement(ElementName = "ISINDIAN")]
        public string IsIndian { get; set; }

        [XmlElement(ElementName = "ISREMOTEACCESSMODE")]
        public string IsRemoteAccessMode { get; set; }

        [XmlElement(ElementName = "ISLICCLIENTMODE")]
        public string IsLicenseClientMode { get; set; }

        [XmlElement(ElementName = "APPLICATIONPATH")]
        public string ApplicationPath { get; set; }

        [XmlElement(ElementName = "DATAPATH")]
        public string DataPath { get; set; }

        [XmlElement(ElementName = "USERLEVEL")]
        public string UserLevel { get; set; }

        [XmlElement(ElementName = "USERNAME")]
        public string UserName { get; set; }

    }


    [XmlRoot(ElementName = "ENVELOPE")]
    public class LicInfoEnvelope : TallyXmlJson
    {

        [XmlElement(ElementName = "HEADER")]
        public Header Header { get; set; }

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
        public LicInfoColloction Collection { get; set; } =  new LicInfoColloction();
    }

    [XmlRoot(ElementName = "COLLECTION")]
    public class LicInfoColloction
    {
        [XmlElement(ElementName = "LICENSEINFO")]
        public LicenseInfo LicenseInfo { get; set; }
        
    }
}
