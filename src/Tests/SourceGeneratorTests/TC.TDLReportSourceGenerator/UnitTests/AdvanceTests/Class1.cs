
using TallyConnector.Services;
using TallyConnector.Core.Models;
using static TallyConnector.Core.Constants;
using TallyConnector.Core.Models.Common;
using TallyConnector.Core.Attributes.SourceGenerator;
using TallyConnector.Core.Attributes;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
namespace TallyConnector.Services.Temp
{
    //[GenerateHelperMethod<Ledger>(GenerationMode = GenerationMode.GetMultiple)]
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

}