using TallyConnector.Core.Models.Masters;

namespace TallyConnector.Services;
[GenerateHelperMethod<Currency>(MethodNameSuffix = "Currency", MethodNameSuffixPlural = "Currencies")]
[GenerateHelperMethod<Group>()]
[GenerateHelperMethod<Ledger>()]
[GenerateHelperMethod<Voucher>(GenerationMode =GenerationMode.GetMultiple)]
public partial class TallyService : BaseTallyService
{

}
