using System.Collections.Generic;
using System.Xml.Serialization;

namespace TallyConnector.Models
{
    [XmlRoot(ElementName = "CURRENCY")]
    public class Currencies : TallyXmlJson
    {
        [XmlAttribute(AttributeName = "ID")]
        public int TallyId { get; set; }

        [XmlElement(ElementName = "ORIGINALNAME")]
        public string OriginalName { get; set; }

        [XmlElement(ElementName = "MAILINGNAME")]
        public string MailingName { get; set; }

        [XmlElement(ElementName = "EXPANDEDSYMBOL")]
        public string ExpandedSymbol { get; set; }

        [XmlElement(ElementName = "DECIMALSYMBOL")]
        public string DecimalSymbol { get; set; }

        [XmlElement(ElementName = "DECIMALPLACES")]
        public int DecimalPlaces { get; set; }

        [XmlElement(ElementName = "INMILLIONS")]
        public string InMilllions { get; set; }

        [XmlElement(ElementName = "ISSUFFIX")]
        public string IsSuffix { get; set; }

        [XmlElement(ElementName = "HASSPACE")]
        public string HasSpace { get; set; }

        [XmlElement(ElementName = "DECIMALPLACESFORPRINTING")]
        public int DecimalPlaces_Print { get; set; }


        /// <summary>
        /// Accepted Values //Create, Alter, Delete
        /// </summary>
        [XmlAttribute(AttributeName = "Action")]
        public string Action { get; set; }

        //[XmlElement(ElementName = "DAILYSTDRATES.LIST")]
        //public List<DailystdRate> DailystdRateList { get; set; }

        //[XmlElement(ElementName = "DAILYBUYINGRATES.LIST")]
        //public List<DailyBuyingRate> DailysBuyingRateList { get; set; }

        //[XmlElement(ElementName = "DAILYSELLINGRATES.LIST")]
        //public List<DailySellingRate> DailySellingRateList { get; set; }
    }
    //[XmlRoot(ElementName = "DAILYSTDRATES.LIST")]
    //public class DailystdRate
    //{

    //    [XmlElement(ElementName = "DATE")]
    //    public int Date { get; set; }

    //    [XmlElement(ElementName = "SPECIFIEDRATE")]
    //    public string SpecifiedRate { get; set; }
    //}

    //[XmlRoot(ElementName = "DAILYSELLINGRATES.LIST")]
    //public class DailySellingRate : DailystdRate
    //{
    //}
    //[XmlRoot(ElementName = "DAILYBUYINGRATES.LIST")]
    //public class DailyBuyingRate : DailystdRate
    //{
    //}


    [XmlRoot(ElementName = "ENVELOPE")]
    public class CurrencyEnvelope : TallyXmlJson
    {

        [XmlElement(ElementName = "HEADER")]
        public Header Header { get; set; }

        [XmlElement(ElementName = "BODY")]
        public CrncyBody Body { get; set; } = new();
    }

    [XmlRoot(ElementName = "BODY")]
    public class CrncyBody
    {
        [XmlElement(ElementName = "DESC")]
        public Description Desc { get; set; } = new();

        [XmlElement(ElementName = "DATA")]
        public CrncyData Data { get; set; } = new();
    }

    [XmlRoot(ElementName = "DATA")]
    public class CrncyData
    {
        [XmlElement(ElementName = "TALLYMESSAGE")]
        public CrncyMessage Message { get; set; } = new();
    }

    [XmlRoot(ElementName = "TALLYMESSAGE")]
    public class CrncyMessage
    {
        [XmlElement(ElementName = "CURRENCY")]
        public Currencies Currency { get; set; }
    }


}
