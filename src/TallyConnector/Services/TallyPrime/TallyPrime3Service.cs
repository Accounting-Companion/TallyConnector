using TallyConnector.Core.Models.Masters;
using TallyConnector.Core.Models.TallyPrime3;

namespace TallyConnector.Services.TallyPrime;
[GenerateHelperMethod<Currency>(MethodNameSuffix = "Currency", MethodNameSuffixPlural = "Currencies")]
[GenerateHelperMethod<Prime3Ledger>(MethodNameSuffix = "Ledger")]
[GenerateHelperMethod<Prime3Voucher>(MethodNameSuffix = "Voucher")]
public partial class TallyPrime3Service : BaseTallyService, ITallyService
{

}
