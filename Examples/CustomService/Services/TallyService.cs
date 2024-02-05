using CustomService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TallyConnector.Core.Attributes;

namespace CustomService.Services;

[GenerateHelperMethod<Bill>()]
[GenerateHelperMethod<Voucher>()]
public partial class TallyService : TallyConnector.Services.BaseTallyService
{

}
