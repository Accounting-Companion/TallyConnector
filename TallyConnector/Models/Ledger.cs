using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace TallyConnector.Models
{
    [Serializable]
    [XmlRoot("LEDGER")]
    public class Ledger
    {

        [XmlAttribute(AttributeName = "ID")]
        public int TallyId { get; set; }

        [XmlAttribute(AttributeName = "NAME")]
        [MaxLength(75)]
        [Required]
        public string Name { get; set; }

        [XmlElement(ElementName = "PARENT")]
        [MaxLength(75)]
        [Required]
        public string Parent { get; set; }

        [XmlElement(ElementName = "Alias")]
        [MaxLength(75)]
        public string Alias { get; set; }

        [XmlElement(ElementName = "TAXTYPE")]
        [MaxLength(75)]
        public string TaxType { get; set; }

        [XmlElement(ElementName = "ISBILLWISEON")]
        [MaxLength(75)]
        public string IsBillwise { get; set; }

        [XmlElement(ElementName = "ISCOSTCENTRESON")]
        [MaxLength(75)]
        public string IsCostcenter { get; set; }

        [XmlElement(ElementName = "ISREVENUE")]
        [MaxLength(75)]
        public string Isrevenue { get; set; }

        [XmlElement(ElementName = "OPENINGBALANCE")]
        [MaxLength(75)]
        public string OpeningBal { get; set; }

        [XmlElement(ElementName = "ISDEEMEDPOSITIVE")]
        [MaxLength(75)]
        public string DeemedPositive { get; set; }

        [XmlElement(ElementName = "AFFECTSSTOCK")]
        [MaxLength(75)]
        public string AffectStock { get; set; }

        [XmlElement(ElementName = "CANDELETE")]
        [MaxLength(75)]
        public string CanDelete { get; set; }

        [XmlElement(ElementName = "FORPAYROLL")]
        [MaxLength(75)]
        public string Forpayroll { get; set; }

        [XmlElement(ElementName = "ISOTHTERRITORYASSESSEE")]
        [MaxLength(75)]
        public string IsOtherTerritory { get; set; }

        [XmlElement(ElementName = "CONSIDERPURCHASEFOREXPORT")]
        [MaxLength(75)]
        public string PurchaseforExport { get; set; }

        [XmlElement(ElementName = "ISTRANSPORTER")]
        [MaxLength(75)]
        public string IsTransporter { get; set; }

        [XmlElement(ElementName = "ISECOMMOPERATOR")]
        [MaxLength(75)]
        public string IsEcommerceOperator { get; set; }

        [XmlElement(ElementName = "CREDITLIMIT")]
        [MaxLength(75)]
        public string CreditLimit { get; set; }

        [XmlElement(ElementName = "ADDRESS.LIST")]
        public virtual Address Address { get; set; }
    }

}
