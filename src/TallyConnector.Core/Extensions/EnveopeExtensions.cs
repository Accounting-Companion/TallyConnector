using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TallyConnector.Core.Models;

namespace TallyConnector.Core.Extensions;
public static class EnveopeExtensions
{
    public static RequestEnvelope PopulateOptions(this RequestEnvelope requestEnvelope, BaseRequestOptions? requestOptions)
    {
        if (requestOptions != null)
        {
            var sv = requestEnvelope.Body.Desc.StaticVariables ??= new();
            sv.SVCompany = requestOptions.Company;
        }
        return requestEnvelope;
    }
    public static RequestEnvelope PopulateOptions(this RequestEnvelope requestEnvelope, DateFilterRequestOptions? requestOptions)
    {
        if (requestOptions != null)
        {
            var sv = requestEnvelope.Body.Desc.StaticVariables ??= new();
            sv.SVCompany = requestOptions.Company;
            sv.SVFromDate = requestOptions.FromDate;
            sv.SVToDate = requestOptions.ToDate;
        }
        return requestEnvelope;
    }
    public static RequestEnvelope PopulateOptions(this RequestEnvelope requestEnvelope, RequestOptions? requestOptions)
    {
        if (requestOptions != null)
        {
            var sv = requestEnvelope.Body.Desc.StaticVariables ??= new();
            sv.SVCompany = requestOptions.Company;
            sv.SVFromDate = requestOptions.FromDate;
            sv.SVToDate = requestOptions.ToDate;
            TDLMessage tDLMessage = requestEnvelope.Body.Desc.TDL.TDLMessage;
            Collection? collection = tDLMessage.Collection.FirstOrDefault();
            if (collection != null)
            {
                collection.Compute = requestOptions.Compute;
                collection.ComputeVar = requestOptions.ComputeVar;
                collection.Filters = requestOptions.Filters?.Select(c => c.FilterName!).ToList();
            }
            
            if (requestOptions.Filters != null && requestOptions.Filters.Count > 0)
            {
                tDLMessage.System ??= [];
                tDLMessage.System.AddRange(requestOptions.Filters.Select(c=>new Models.System(c.FilterName!,c.FilterFormulae!)));
            }
           
        }
        return requestEnvelope;
    }
}
