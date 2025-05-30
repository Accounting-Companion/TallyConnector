using TallyConnector.Models.Common;
using TallyConnector.Models.TallyPrime.V6.Masters;

namespace TallyConnector.Services.TallyPrime.V6;

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