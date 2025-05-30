namespace TallyConnector.Services.TallyPrime.V5;

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