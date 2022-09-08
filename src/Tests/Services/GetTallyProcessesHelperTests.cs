using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Services;
internal class GetTallyProcessesHelperTests
{
    [Test]
    public async Task CheckGettallyprocesses()
    {
        List<TallyProcessInfo> tallyProcessInfos = GetTallyProcessesHelper.GetTallyProcesses();
        ConfigureServerPortHelper.ConfigureTallyServerPort(tallyProcessInfos[1], 9001);
    }
}
