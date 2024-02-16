using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TallyConnector.Core.Models.TallyPrime3;
public class Prime3Voucher :Voucher
{
    [XmlElement(ElementName = "GSTREGISTRATION")]
    public GSTRegistration? GSTRegistration { get; set; }
}
