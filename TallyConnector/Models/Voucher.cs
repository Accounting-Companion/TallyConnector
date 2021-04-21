﻿using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace TallyConnector.Models
{
    [Serializable]
    [XmlRoot(ElementName = "VOUCHER")]
    public class Voucher:TallyXmlJson
    {

        [XmlAttribute(AttributeName = "ID")]
        public int TallyId { get; set; }

        [XmlElement(ElementName = "DATE")]
        public string VchDate { get; set; }


        [XmlElement(ElementName = "VOUCHERTYPENAME")]
        public string VoucherType { get; set; }


        [XmlElement(ElementName = "VOUCHERNUMBER")]
        public string VoucherNumber { get; set; }

        [XmlElement(ElementName = "ISOPTIONAL")]
        public string Isoptional { get; set; }

        [XmlElement(ElementName = "EFFECTIVEDATE")]
        public string EffectiveDate { get; set; }

        
        [XmlElement(ElementName = "ALLLEDGERENTRIES.LIST")]
        public List<IVoucherLedger> Ledgers { get; set; }

        [XmlAttribute(AttributeName = "DATE")]
        public string Dt
        {
            get
            {
                return VchDate;
            }
            set
            {
                VchDate = value;
            }
        }

        [XmlAttribute(AttributeName = "TAGNAME")]
        public string TAGNAME
        {
            get
            {
                return "Voucher Number";
            }
            set
            {
                value = "Voucher Number";
            }
        }

        [XmlAttribute(AttributeName = "TAGVALUE")]
        public string TAGVALUE
        {
            get
            {
                return VoucherNumber;
            }
            set
            {
                VoucherNumber = value;
            }
        }

        [XmlAttribute(AttributeName = "Action")]
        public string Action { get; set; }

        [XmlAttribute(AttributeName = "VCHTYPE")]
        public string VCHTYPE
        {
            get
            {
                return VoucherType;
            }
            set
            {
                VoucherType = value;
            }
        }

      


    }

    [XmlRoot(ElementName = "LEDGERENTRIES.LIST")]
    public class EVoucherLedger : IVoucherLedger
    {

    }
   
    [XmlRoot(ElementName = "ALLLEDGERENTRIES.LIST")]
    public class IVoucherLedger
    {

        [XmlElement(ElementName = "LEDGERNAME")]
        public string LedgerName { get; set; }


        [XmlElement(ElementName = "ISDEEMEDPOSITIVE")]
        public string IsDeemedPositive { get; set; }

        [XmlElement(ElementName = "AMOUNT")]
        public double Amount { get; set; }


        [XmlElement(ElementName = "BILLALLOCATIONS.LIST")]
        public List<BillAllocations> BillAllocations { get; set; }

        [XmlElement(ElementName = "INVENTORYALLOCATIONS.LIST")]
        public List<InventoryAllocations> InventoryAllocations { get; set; }



    }

    [XmlRoot(ElementName = "BILLALLOCATIONS.LIST")]
    public class BillAllocations
    {
        [XmlElement(ElementName = "BILLTYPE")]
        public string BillType { get; set; }

        [XmlElement(ElementName = "NAME")]
        public string Name { get; set; }


        [XmlElement(ElementName = "AMOUNT")]
        public string Amount { get; set; }
    }

    [XmlRoot(ElementName = "INVENTORYALLOCATIONS.LIST")]
    public class InventoryAllocations 
    {
        [XmlElement(ElementName = "STOCKITEMNAME")]
        public string StockItemName { get; set; }

        [XmlElement(ElementName = "ISDEEMEDPOSITIVE")]
        public string DeemedPositive { get; set; }

        [XmlElement(ElementName = "RATE")]
        public string Rate { get; set; }

        [XmlElement(ElementName = "ACTUALQTY")]
        public string Quantity { get; set; }

        [XmlElement(ElementName = "AMOUNT")]
        public double Amount { get; set; }


        [XmlElement(ElementName = "BATCHALLOCATIONS.LIST")]
        public List<BatchAllocations> BacthAllocations { get; set; }

        [XmlElement(ElementName = "CATEGORYALLOCATIONS.LIST")]
        public List<CostCategoryAllocations> CostCategoryAllocations { get; set; }

    }

    [XmlRoot(ElementName = "BATCHALLOCATIONS.LIST")]
    public class BatchAllocations //Godown Allocations
    {
        [XmlElement(ElementName = "GODOWNNAME")]
        public string GodownName { get; set; }

        [XmlElement(ElementName = "ACTUALQTY")]
        public string Quantity { get; set; }

        [XmlElement(ElementName = "AMOUNT")]
        public string Amount { get; set; }


    }

    [XmlRoot(ElementName = "CATEGORYALLOCATIONS.LIST")]
    public class CostCategoryAllocations
    {
        [XmlElement(ElementName = "STOCKITEMNAME")]
        public string CostCategoryName { get; set; }

        [XmlElement(ElementName = "COSTCENTREALLOCATIONS.LIST")]
        public List<CostCenterAllocations> CostCenterAllocations { get; set; }

    }
    [XmlRoot(ElementName = "COSTCENTREALLOCATIONS.LIST")]
    
    public class CostCenterAllocations
    {
        [XmlElement(ElementName = "NAME")]
        public string Name { get; set; }

        [XmlElement(ElementName = "AMOUNT")]
        public double Amount { get; set; }
    }






        /// <summary>
        /// Voucher Message
        /// </summary>




        [XmlRoot(ElementName = "ENVELOPE")]
    public class VoucherEnvelope:TallyXmlJson
    {

        [XmlElement(ElementName = "HEADER")]
        public Header Header { get; set; }

        [XmlElement(ElementName = "BODY")]
        public VBody Body { get; set; } = new VBody();
    }

    [XmlRoot(ElementName = "BODY")]
    public class VBody
    {
        [XmlElement(ElementName = "DESC")]
        public Description Desc { get; set; } = new Description();

        [XmlElement(ElementName = "DATA")]
        public VData Data { get; set; } = new VData();
    }

    [XmlRoot(ElementName = "DATA")]
    public class VData
    {
        [XmlElement(ElementName = "TALLYMESSAGE")]
        public VoucherMessage Message { get; set; } = new VoucherMessage();
    }

    [XmlRoot(ElementName = "TALLYMESSAGE")]
    public class VoucherMessage
    {
        [XmlElement(ElementName = "VOUCHER")]
        public Voucher Voucher { get; set; }
    }


}


