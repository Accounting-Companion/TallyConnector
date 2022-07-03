using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TallyConnector.Services;
public partial class TallyService
{
    public async Task<List<MasterTypeStat>> GetMasterStatisticsAsync()
    {
        return new();
    }
}
