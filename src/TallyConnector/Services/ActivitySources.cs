using System.Diagnostics;
using TallyConnector.Services.TallyPrime;

namespace TallyConnector.Services;
public static class ActivitySources
{
    public const string BaseServiceActivityName = $"TallyConnector.{nameof(BaseTallyService)}";
    public const string TallyServiceActivityName = $"TallyConnector.{nameof(TallyService)}";
    
    public static ActivitySource BaseTallyServiceActivitySource = new(BaseServiceActivityName);
    public static ActivitySource TallyServiceActivitySource = new(TallyServiceActivityName);
    public static ActivitySource TallyPrime3ServiceActivitySource = new($"TallyConnector.{nameof(TallyPrime3Service)}");
}
