﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TallyConnector.Core.Models.Interfaces.Masters;
using TallyConnector.Core.Models.Masters;

namespace TallyConnector.Core.Models.TallyPrime3;
[XmlRoot("LEDGER")]
[XmlType(AnonymousType = true)]
[TDLCollection(Type ="Ledger")]
public partial class Prime3Ledger : Ledger
{

}