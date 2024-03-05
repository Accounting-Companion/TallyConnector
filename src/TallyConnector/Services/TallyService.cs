﻿using TallyConnector.Core.Models.Masters;

namespace TallyConnector.Services;
[GenerateHelperMethod<Currency>(MethodNameSuffix = "Currency", MethodNameSuffixPlural = "Currencies")]
[GenerateHelperMethod<Group>()]
[GenerateHelperMethod<Ledger>()]
[GenerateHelperMethod<VoucherType>()]
[GenerateHelperMethod<Voucher>()]
public partial class TallyService : BaseTallyService
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
}