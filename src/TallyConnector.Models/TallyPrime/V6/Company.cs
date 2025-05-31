using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TallyConnector.Models.TallyPrime.V6;
[XmlRoot(ElementName = "COMPANY")]
[XmlType(AnonymousType = true)]
[ImplementTallyRequestableObject]
public partial class Company : Base.Company
{
}
