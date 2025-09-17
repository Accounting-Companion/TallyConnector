namespace TallyConnector.Services.TallyPrime.V5;

public partial class TallyPrimeService : TallyAbstractClient
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