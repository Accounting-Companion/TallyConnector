using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TallyConnector.Core.Models;
public class TallyConstants
{
    public class OBJECTTYPES
    {
        public const string CURRENCY = "CURRENCY";
        public const string GROUP = "GROUP";
        public const string LEDGER = "LEDGER";
        public const string COSTCATEGORY = "COSTCATEGORY";
        public const string COSTCENTRE = "COSTCENTRE";
        public const string STOCKCATEGORY = "STOCKCATEGORY";
        public const string STOCKGROUP = "STOCKGROUP";
        public const string STOCKITEM = "STOCKITEM";
        public const string UNIT = "UNIT";
        public const string GODOWN = "GODOWN";
        public const string VOUCHERTYPE = "VOUCHERTYPE";
        public const string VOUCHER = "VOUCHERS";
        public class Currency
        {
            public const string CollectionType = CURRENCY;
            public const string XMLTAG = CURRENCY;
        }
        public class Group
        {
            public const string CollectionType = GROUP;
            public const string XMLTAG = GROUP;
        }
    }
}
