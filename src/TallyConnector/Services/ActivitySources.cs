using System.Diagnostics;
using TallyConnector.Services.TallyPrime.V4;

namespace TallyConnector.Services;
public static class ActivitySources
{
    public const string BaseServiceActivityName = $"TallyConnector.{nameof(BaseTallyService)}";

    public static ActivitySource BaseTallyServiceActivitySource = new(BaseServiceActivityName);
    public static ActivitySource TallyPrime4ServiceActivitySource = new($"TallyConnector.{nameof(TallyPrimeService)}");
}
