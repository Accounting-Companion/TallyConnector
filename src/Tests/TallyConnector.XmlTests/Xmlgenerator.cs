using System;
using System.Collections.Generic;
using System.Text;
using TallyConnector.Core.Extensions;
using TallyConnector.Core.Models.Interfaces;
using TallyConnector.Core.Models.Request;
using TallyConnector.Core.Models.Response;
using TallyConnector.Models.Common.Pagination;
using TallyConnector.Models.TallyPrime.V6;
using TallyConnector.Models.TallyPrime.V6.Masters;
using TallyConnector.Services;
using TallyConnector.Services.TallyPrime.V6;

namespace TallyConnector.XmlTests
{
    public class Xmlgenerator
    {
        public const string ResourceBasePath = "D:\\SourceCode\\AccountingCompanion\\TallyConnector_New\\src\\Tests\\TallyConnector.XmlTests\\Resources\\TallyPrime";
        private BaseTallyService service;

        public Xmlgenerator()
        {
            service = new BaseTallyService();
        }
        //[Test]
        public async Task GenerateXmls()
        {
            await GetObjectsAsync<Currency>(new PaginatedRequestOptions { RecordsPerPage = 100 });

            await GetObjectsAsync<Group>(new PaginatedRequestOptions { RecordsPerPage = 100 });
            await GetObjectsAsync<Ledger>(new PaginatedRequestOptions { RecordsPerPage = 100 });
            await GetObjectsAsync<CostCentre>(new PaginatedRequestOptions { RecordsPerPage = 100 });
            foreach (var item in await new  TallyPrimeService().GetVoucherStatisticsAsync(new()))
            {
                await GetObjectsAsync<Voucher>(new PaginatedRequestOptions {FromDate=new(2000,04,1),DisableCountTag=true, RecordsPerPage = 100,CollectionType="Vouchers:VoucherType",Childof=item.Name },name:$"Vouchers_{item.Name}", version: "V6", token: default);
            }
            //await GetObjectsAsync<Voucher>(new PaginatedRequestOptions { RecordsPerPage = 100 });
        }
        public async Task GetObjectsAsync<T>(PaginatedRequestOptions options, string? name=null,string version = "V6",CancellationToken token = default) where T : ITallyRequestableObject, IBaseObject
        {
            var reqEnvelope = T.GetRequestEnvelope();
            reqEnvelope.PopulateOptions(options);
            //await _baseHandler.PopulateDefaultOptions(reqEnvelope, token);
            var reqXml = reqEnvelope.GetXML();
            var resp = await service.SendRequestAsync(reqXml, "Getting Objects", token);
            var objectName = typeof(T).Name;
            name ??= $"{objectName}s";
            string path = Path.Join(ResourceBasePath, version, objectName);
            if(!Path.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            await File.WriteAllTextAsync(Path.Join(path, $"{name}_complete.xml"), resp.Response);

        }
    }
}
