using TallyConnector.Core.Models.Request;
using TallyConnector.Core.Models.TallyPrime.V4.Masters;

namespace TallyConnector.Services.TallyPrime.V4;


[GenerateHelperMethod<MasterStatistics>(MethodNameSuffixPlural = nameof(MasterStatistics), GenerationMode = GenerationMode.GetMultiple, Args = [typeof(BaseRequestOptions)])]
[GenerateHelperMethod<VoucherStatistics>(MethodNameSuffixPlural = nameof(VoucherStatistics), GenerationMode = GenerationMode.GetMultiple, Args = [typeof(DateFilterRequestOptions)])]
[GenerateHelperMethod<Company>(MethodNameSuffixPlural = "Companies", GenerationMode = GenerationMode.GetMultiple)]
[GenerateHelperMethod<CompanyOnDisk>(MethodNameSuffixPlural = "CompaniesinDefaultPath", GenerationMode = GenerationMode.GetMultiple)]

[GenerateHelperMethod<Currency>(MethodNameSuffix = "Currency", MethodNameSuffixPlural = "Currencies")]
[GenerateHelperMethod<Group>()]
[ImplementTallyService(nameof(_baseHandler))]
public partial class TallyPrimeService : TallyCommonService
{
    private readonly ILogger _logger;
    private readonly IBaseTallyService _baseHandler;

    public TallyPrimeService()
    {
        _baseHandler = new BaseTallyService();
        _logger = NullLogger.Instance;
    }
    public TallyPrimeService(IBaseTallyService baseTallyService)
    {
        _baseHandler = baseTallyService;
        _logger = NullLogger.Instance;
    }

    public TallyPrimeService(ILogger logger, IBaseTallyService baseTallyService)
    {
        _logger = logger;
        _baseHandler = baseTallyService;
    }

    public void SetupTallyService(string url, int port)
    {
        _baseHandler.Setup(url, port);
    }

}