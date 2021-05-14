using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace TallyConnector.Models
{
    [Serializable]
    [XmlRoot(ElementName = "VOUCHER")]
    public class Voucher : TallyXmlJson
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

        [XmlElement(ElementName = "NARRATION")]
        public string Narration { get; set; }


        [XmlElement(ElementName = "ISCANCELLED")]
        public string IsCancelled { get; set; }

        [XmlElement(ElementName = "ALLLEDGERENTRIES.LIST")]
        public List<IVoucherLedger> Ledgers { get; set; }

        [JsonIgnore]
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
        
        [JsonIgnore]
        [XmlAttribute(AttributeName = "TAGNAME")]
        public string TAGNAME
        {
            get
            {

                return "Voucher Number";
            }
            set
            {
            }
        }

        [JsonIgnore]
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


        /// <summary>
        /// Accepted Values //Create, Alter, Delete
        /// </summary>
        [JsonIgnore]
        [XmlAttribute(AttributeName = "Action")]
        public String Action { get; set; }

        [JsonIgnore]
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


        public void OrderLedgers()
        {
            if (VCHTYPE != "Contra" && VCHTYPE != "Purchase" && VCHTYPE != "Receipt" && VCHTYPE != "Credit Note" )
            {

                Ledgers.Sort((x, y) => x.LedgerName.CompareTo(y.LedgerName));//First Sort Ledger list Using Ledger Names
                Ledgers.Sort((x, y) => x.Amount.CompareTo(y.Amount)); //Next sort Ledger List Using Ledger Amounts



            }
            else
            {
                Ledgers.Sort((x, y) => y.LedgerName.CompareTo(x.LedgerName));//First Sort Ledger list Using Ledger Names
                Ledgers.Sort((x, y) => y.Amount.CompareTo(x.Amount)); //Next sort Ledger List Using Ledger Amounts

            }

            //Looop Through all Ledgers
            Ledgers.ForEach(c =>
            {
                //Sort Bill Allocations
                c.BillAllocations.Sort((x, y) => x.Name.CompareTo(y.Name)); //First Sort BillAllocations Using Bill Numbers
                c.BillAllocations.Sort((x, y) => x.Amount.CompareTo(y.Amount));//Next sort BillAllocationst Using  Amounts

                c.CostCategoryAllocations.Sort((x, y) => x.CostCategoryName.CompareTo(y.CostCategoryName));

                c.CostCategoryAllocations.ForEach(cc =>
                {
                    cc.CostCenterAllocations.Sort((x, y) => x.Name.CompareTo(y.Name));
                    cc.CostCenterAllocations.Sort((x, y) => x.Amount.CompareTo(y.Amount));
                });
                //sort Inventory Allocations
                c.InventoryAllocations.Sort((x, y) => x.ActualQuantity.CompareTo(y.ActualQuantity));
                c.InventoryAllocations.Sort((x, y) => x.Amount.CompareTo(y.Amount));

                c.InventoryAllocations.ForEach(inv =>
                {
                    inv.BacthAllocations.Sort((x, y) => x.GodownName.CompareTo(y.GodownName));
                    inv.BacthAllocations.Sort((x, y) => x.Amount.CompareTo(y.Amount));

                    inv.CostCategoryAllocations.Sort((x, y) => x.CostCategoryName.CompareTo(y.CostCategoryName));

                    inv.CostCategoryAllocations.ForEach(cc =>
                    {
                        cc.CostCenterAllocations.Sort((x, y) => x.Name.CompareTo(y.Name));
                        cc.CostCenterAllocations.Sort((x, y) => x.Amount.CompareTo(y.Amount));
                    });
                });

            });
        }

        public new string GetJson()
        {
            OrderLedgers();

            return base.GetJson();
        }

        public new string GetXML()
        {
            OrderLedgers();

            return base.GetXML();
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

        
        private string _Amount;

        public string ForexAmount { get;  set; }

        public string RateofExchange { get;  set; }

        [XmlElement(ElementName = "AMOUNT")]
        public string Amount
        {
            get { return _Amount; }
            set
            {
                if (value.ToString().Contains("="))
                {
                    var s = value.ToString().Split('=');
                    var k = s[0].Split('@');
                    ForexAmount = k[0];
                    RateofExchange = k[1].Split()[2];
                    _Amount = s[1].Split()[2];
                }
                else
                {
                    _Amount = value;
                }

            }
        }



        [XmlElement(ElementName = "BILLALLOCATIONS.LIST")]
        public List<BillAllocations> BillAllocations { get; set; }

        [XmlElement(ElementName = "INVENTORYALLOCATIONS.LIST")]
        public List<InventoryAllocations> InventoryAllocations { get; set; }

        [XmlElement(ElementName = "CATEGORYALLOCATIONS.LIST")]
        public List<CostCategoryAllocations> CostCategoryAllocations { get; set; }
        
    }

    [XmlRoot(ElementName = "BILLALLOCATIONS.LIST")]
    public class BillAllocations
    {
        [XmlElement(ElementName = "BILLTYPE")]
        public string BillType { get; set; }

        [XmlElement(ElementName = "NAME")]
        public string Name { get; set; }


        private string _Amount;
        public string ForexAmount { get; set; }
        public string RateofExchange { get; set; }
        [XmlElement(ElementName = "AMOUNT")]
        public string Amount
        {
            get { return _Amount; }
            set
            {
                if (value.ToString().Contains("="))
                {
                    var s = value.ToString().Split('=');
                    var k = s[0].Split('@');
                    ForexAmount = k[0];
                    RateofExchange = k[1].Split()[2];
                    _Amount = s[1].Split()[2];
                }
                else
                {
                    _Amount = value;
                }

            }
        }


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
        public string ActualQuantity { get; set; }

        [XmlElement(ElementName = "BILLEDQTY")]
        public string BilledQuantity { get; set; }

        private string _Amount;
        public string ForexAmount { get; set; }
        public string RateofExchange { get; set; }
        [XmlElement(ElementName = "AMOUNT")]
        public string Amount
        {
            get { return _Amount; }
            set
            {
                if (value.ToString().Contains("="))
                {
                    var s = value.ToString().Split('=');
                    var k = s[0].Split('@');
                    ForexAmount = k[0];
                    RateofExchange = k[1].Split()[2];
                    _Amount = s[1].Split()[2];
                }
                else
                {
                    _Amount = value;
                }

            }
        }




        [XmlElement(ElementName = "BATCHALLOCATIONS.LIST")]
        public List<BatchAllocations> BacthAllocations { get; set; }

        [XmlElement(ElementName = "CATEGORYALLOCATIONS.LIST")]
        public List<CostCategoryAllocations> CostCategoryAllocations { get; set; }

    }

    [XmlRoot(ElementName = "BATCHALLOCATIONS.LIST")]
    public class BatchAllocations //Godown Allocations
    {

        [XmlElement(ElementName = "TRACKINGNUMBER")]
        public string TrackingNo { get; set; }

        [XmlElement(ElementName = "ORDERNO")]
        public string OrderNo { get; set; }

        [XmlElement(ElementName = "GODOWNNAME")]
        public string GodownName { get; set; }

        [XmlElement(ElementName = "ACTUALQTY")]
        public string Quantity { get; set; }

        private string _Amount;
        public string ForexAmount { get; set; }
        public string RateofExchange { get; set; }
        [XmlElement(ElementName = "AMOUNT")]
        public string Amount
        {
            get { return _Amount; }
            set
            {
                if (value.ToString().Contains("="))
                {
                    var s = value.ToString().Split('=');
                    var k = s[0].Split('@');
                    ForexAmount = k[0];
                    RateofExchange = k[1].Split()[2];
                    _Amount = s[1].Split()[2];
                }
                else
                {
                    _Amount = value;
                }

            }
        }



    }

    [XmlRoot(ElementName = "CATEGORYALLOCATIONS.LIST")]
    public class CostCategoryAllocations
    {
        [XmlElement(ElementName = "CATEGORY")]
        public string CostCategoryName { get; set; }

        [XmlElement(ElementName = "COSTCENTREALLOCATIONS.LIST")]
        public List<CostCenterAllocations> CostCenterAllocations { get; set; }

    }
    [XmlRoot(ElementName = "COSTCENTREALLOCATIONS.LIST")]

    public class CostCenterAllocations
    {
        [XmlElement(ElementName = "NAME")]
        public string Name { get; set; }

        private string _Amount;
        public string ForexAmount { get; set; }
        public string RateofExchange { get; set; }
        [XmlElement(ElementName = "AMOUNT")]
        public string Amount
        {
            get { return _Amount; }
            set
            {
                if (value.ToString().Contains("="))
                {
                    var s = value.ToString().Split('=');
                    var k = s[0].Split('@');
                    ForexAmount = k[0];
                    RateofExchange = k[1].Split()[2];
                    _Amount = s[1].Split()[2];
                }
                else
                {
                    _Amount = value;
                }

            }
        }


    }






    /// <summary>
    /// Voucher Message
    /// </summary>




    [XmlRoot(ElementName = "ENVELOPE")]
    public class VoucherEnvelope : TallyXmlJson
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


