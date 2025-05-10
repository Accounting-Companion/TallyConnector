using TallyConnector.Core.Attributes;
using TallyConnector.Models.Common;
using TallyConnector.Models.TallyPrime.V6.Masters;

namespace TallyConnector.Services.TallyPrime.V6;


[GenerateHelperMethod<MasterStatistics>(MethodNameSuffixPlural = nameof(MasterStatistics), GenerationMode = GenerationMode.GetMultiple, Args = [typeof(BaseRequestOptions)])]
[GenerateHelperMethod<VoucherStatistics>(MethodNameSuffixPlural = nameof(VoucherStatistics), GenerationMode = GenerationMode.GetMultiple, Args = [typeof(DateFilterRequestOptions)])]
[GenerateHelperMethod<Company>(MethodNameSuffixPlural = "Companies", GenerationMode = GenerationMode.GetMultiple)]
[GenerateHelperMethod<CompanyOnDisk>(MethodNameSuffixPlural = "CompaniesinDefaultPath", GenerationMode = GenerationMode.GetMultiple)]

[GenerateHelperMethod<Currency>(MethodNameSuffix = "Currency", MethodNameSuffixPlural = "Currencies")]
[GenerateHelperMethod<Group>()]
[ImplementTallyService(nameof(_baseHandler))]
public partial class TallyPrimeService : TallyCommonService
{
    public TallyPrimeService()
    {
    }

    public TallyPrimeService(IBaseTallyService baseTallyService) : base(baseTallyService)
    {
    }

    public TallyPrimeService(ILogger logger, IBaseTallyService baseTallyService) : base(logger, baseTallyService)
    {
    }

}