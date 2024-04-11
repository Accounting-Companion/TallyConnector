using System.Diagnostics;
using TallyConnector.Core.Models.Interfaces.Masters;
using TallyConnector.Core.Models.Masters;
using TallyConnector.Services.Models;

namespace TallyConnector.Services;
[GenerateHelperMethod<Currency>(MethodNameSuffix = "Currency", MethodNameSuffixPlural = "Currencies")]
[GenerateHelperMethod<Group>()]
[GenerateHelperMethod<Ledger>()]
[GenerateHelperMethod<VoucherType>()]
[GenerateHelperMethod<Voucher>()]
[SetActivitySource(ActivitySource = nameof(TallyServiceActivitySource))]
public partial class TallyService : BaseTallyService, ITallyService
{
    public TallyService()
    {
    }

    public TallyService(string baseURL, int port, double timeoutMinutes = 3) : base(baseURL, port, timeoutMinutes)
    {
    }

    public TallyService(HttpClient httpClient, ILogger? logger = null, double timeoutMinutes = 3) : base(httpClient, logger, timeoutMinutes)
    {
    }

    public async Task TPostObjectsAsync(IEnumerable<IBaseTallyObject> objects)
    {

        List<IBaseTallyObjectDTO> DTOobjects = [];
        foreach (var item in objects)
        {
            switch (item)
            {
                case Group obj:
                    DTOobjects.Add((GroupDTO)obj);
                    break;
                case GroupDTO obj:
                    DTOobjects.Add(obj);
                    break;
                default:
                    break;
            }
        }
        await TPostObjectsAsync(DTOobjects);

    }
    public async Task TPostObjectsAsync(IEnumerable<IBaseTallyObjectDTO> objects)
    {
        var message = new TallyServicePostRequestEnvelopeMessage();
        foreach (var obj in objects)
        {
            obj.RemoteId ??= Guid.NewGuid().ToString();
            obj.Action = obj.Action is Core.Models.Action.None ? Core.Models.Action.Create : obj.Action;
            switch (obj)
            {
                case GroupDTO groupDTO:
                    message.Groups.Add(groupDTO);
                    break;
                default:
                    break;
            }
        }

    }
}
