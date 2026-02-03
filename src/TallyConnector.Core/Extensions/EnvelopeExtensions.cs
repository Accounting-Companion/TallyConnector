using TallyConnector.Abstractions.Models;
using TallyConnector.Core.Models;
using TallyConnector.Core.Models.Request;

namespace TallyConnector.Core.Extensions;
public static class EnvelopeExtensions
{
    public static RequestEnvelope PopulateOptions(this RequestEnvelope requestEnvelope, BaseRequestOptions? requestOptions)
    {
        if (requestOptions != null)
        {

            switch (requestOptions)
            {
                case RequestOptions options:
                    PopulateOptions(requestEnvelope, options);
                    break;
                case DateFilterRequestOptions options:
                    PopulateOptions(requestEnvelope, options);
                    break;
                default:
                    var sv = requestEnvelope.Body.Desc.StaticVariables ??= new();
                    sv.SVCompany = requestOptions.Company;
                    break;
            }
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
                if (requestOptions.Filters != null && requestOptions.Filters.Count != 0)
                {

                    collection.Filters ??= [];
                    collection.Filters.AddRange([.. requestOptions.Filters.Select(c => c.FilterName!)]);
                }
                collection.Childof = requestOptions.Childof;
                collection.BelongsTo = requestOptions.BelongsTo ?? YesNo.None;
                if (!string.IsNullOrWhiteSpace(requestOptions.CollectionType))
                {
                    collection.Type = requestOptions.CollectionType;
                }
            }

            if (requestOptions.Filters != null && requestOptions.Filters.Count > 0)
            {
                tDLMessage.System ??= [];
                tDLMessage.System.AddRange(requestOptions.Filters.Select(c => new Models.Request.System(c.FilterName!, c.FilterFormulae!)));
            }

        }
        return requestEnvelope;
    }
    public static RequestEnvelope PopulateOptions(this RequestEnvelope requestEnvelope, PaginatedRequestOptions? requestOptions)
    {
        if (requestOptions != null)
        {
            requestOptions.Filters ??= [];
            requestOptions.Compute ??= [];
            requestOptions.ComputeVar ??= [];

            int recordsPerPage = requestOptions.RecordsPerPage ?? 1000;
            int Start = recordsPerPage * (requestOptions.PageNum - 1);
            TDLMessage tDLMessage = requestEnvelope.Body.Desc.TDL.TDLMessage;
            Collection collection = tDLMessage.Collection.First();
            if (!string.IsNullOrWhiteSpace(requestOptions.CollectionType))
            {
                collection.Type = requestOptions.CollectionType;
            }
            if (!requestOptions.DisableCountTag)
            {
                if (!string.IsNullOrWhiteSpace(requestOptions.Childof))
                {
                    collection.Childof = requestOptions.Childof;
                    collection.BelongsTo = requestOptions.BelongsTo ?? YesNo.None;                    
                }
                string CollectionName = $"{collection.Name}_NonPaginated";
                tDLMessage.Collection.Add(new(CollectionName, collection.Type!, filters: [.. requestOptions.Filters.Select(c => c.FilterName!)]) { Childof = requestOptions.Childof, BelongsTo = requestOptions.BelongsTo ?? YesNo.None });
                const string objectCountName = "TC_ObjectsCount";
                Part part = tDLMessage.Part!.First();
                part.Lines = [.. part.Lines ?? [], objectCountName];
                tDLMessage.Part = [.. tDLMessage.Part ?? [], new(objectCountName, null)];
                tDLMessage.Line = [.. tDLMessage.Line ?? [], new(objectCountName, [objectCountName])];
                tDLMessage.Field = [.. tDLMessage.Field ?? [], new Field(objectCountName, "TC_TotalCount", $"$$NUMITEMS:{CollectionName}")];
            }


            requestOptions.Compute = [.. requestOptions.Compute, "LineIndex : ##vLineIndex"];
            requestOptions.ComputeVar = [.. requestOptions.ComputeVar, "vLineIndex: Number : IF $$IsEmpty:##vLineIndex THEN 1 ELSE ##vLineIndex + 1"];
            requestOptions.Filters = [.. requestOptions.Filters, new("TC_PaginationFilter", $"##vLineIndex <= {Start + recordsPerPage} AND ##vLineIndex > {Start}")];
            requestEnvelope.PopulateOptions((RequestOptions)requestOptions);

        }
        return requestEnvelope;
    }
    public static void AddCustomResponseReportForPost(this RequestEnvelope requestEnvelope)
    {
        var tDLMessage = requestEnvelope.Body.Desc.TDL.TDLMessage;

        const string TDLVarName = "CCTotalMisMatch";
        const string TDLObjectTypeVarName = "VTMark";
        const string RemoteIdVarName = "VchType";
        const string guidVarName = "VchDate";
        const string MasterIdVarName = "VchMID";
        const string ActionVarName = "CCCatName";
        const string NameVarName = "CCLedName";
        const string errorvarName = "VchNumber";


        const string dummyVarName = "Dummy";



        const string customReportName = "TC_CustomReportAndEvents";
        const string collectionName = "TC_CustResultsColl";
        const string importStartFunctionName = "TC_OnImportStart";
        const string importObjectFunctionName = "TC_BeforeImportObject";
        const string afterImportObjectFunctionName = "TC_AfterImportObject";
        const string importEndFunctionName = "TC_OnImportEnd";

        const string objectTypeFieldName = "TC_ObjectTypeField";
        const string nameFieldName = "TC_NameField";
        const string masterIdFieldName = "TC_MasterIdField";
        const string guidFieldName = "TC_guidField";
        const string remoteIdFieldName = "TC_RemoteIdField";
        const string actionTypeFieldName = "TC_ActionTypeField";
        const string respMessageFieldName = "TC_RespMsgField";


        const string onKeyword = "On";
        tDLMessage.ImportFile = [new("ALL MASTERS", [customReportName+":Yes"]){ IsModify=YesNo.Yes},
            new(customReportName)
            {
                IsOption = YesNo.Yes,
                ResponseReport = customReportName,
                Delete =[onKeyword],
                Add =
                [
                    $"{onKeyword} : Start Import : Yes : Call : {importStartFunctionName}",
                    $"{onKeyword} : Import Object : Yes : Call : {importObjectFunctionName}",
                    $"{onKeyword} : Import Object : Yes :  Import Object ",
                    $"{onKeyword} : After Import Object  : Yes : Call : {afterImportObjectFunctionName}",
                    //$"{onKeyword} : End Import : Yes : Call : {importEndFunctionName}",
                ]
            }];
        tDLMessage.Report = [new(customReportName)];
        tDLMessage.Form = [new(customReportName) { ReportTag = "RESULTS" }];
        tDLMessage.Part = [new(customReportName, collectionName)];
        tDLMessage.Line = [new(customReportName, [objectTypeFieldName, nameFieldName, masterIdFieldName, guidFieldName, remoteIdFieldName, respMessageFieldName], "RESULT")];
        tDLMessage.Field =
        [
            new(objectTypeFieldName, "ObjectType", $"${TDLObjectTypeVarName}"),
            new(nameFieldName, "Name", $"${NameVarName}"),
            new(masterIdFieldName, "MasterId", $"${MasterIdVarName}"),
            new(guidFieldName, "GUID", $"${guidVarName}"),
            new(remoteIdFieldName, "REMOTEID", $"${RemoteIdVarName}"),
            //new(actionTypeFieldName, "ACTION", $"${ActionVarName}"){Invisible="$$ISEMPTY:$$Value"},
            new(respMessageFieldName, "Error", $"${errorvarName}"),
        ];
        int ifcounter = 1;
        int actionCounter = 2;

        string[] objectTypes = ["Group", "Ledger", "Cost Category", "Cost Centre", "Unit", "Stock Group", "Stock Category", "Stock Item", "Godown", "Attendance Type", "Voucher Type", "Currency"];
        var mastersObjectActions = objectTypes.SelectMany(c =>
        {
            List<string> actions = [$"TC_IF{++ifcounter:00} : IF : ##TC_ObjecType=\"{c}\""];
            actions.Add($"TC_C{++actionCounter:00} : Set Object   : ({c},$Name).");
            actions.Add($"TC_IF{++ifcounter:00} : ENDIF");
            return actions;
        });

        tDLMessage.Functions =
        [
            new(importStartFunctionName){ Actions=[$"01A    : LISTDELETE : {TDLVarName}"]},
            new(importObjectFunctionName){
                Variables =["TC_ObjecType : String:$$type"],
                Actions=
                [
                    $"TC00   : LISTADD   :{TDLVarName} :$REMOTEALTGUID:$REMOTEALTGUID:{RemoteIdVarName}",
                    $"TC01   : LISTADD   :{TDLVarName} :$REMOTEALTGUID:##TC_ObjecType:{TDLObjectTypeVarName}",
                ]},
            new(afterImportObjectFunctionName){
                Variables =["TC_ObjecType : String:$$type", "TC_Action : String:$$ImportAction", "VRemoteId:String:$REMOTEALTGUID", "Dummy : String"],
                Actions=
                [
                    $"TC_C00 : LISTADD : {TDLVarName}:$REMOTEALTGUID:##TC_Action:{ActionVarName}",
                    $"TC_IF{ifcounter:00} : IF : ##TC_ObjecType=\"Voucher\"",
                    $"TC_C{++actionCounter:00} : Set Object   : (Voucher,$$LastCreatedVchId).",
                    $"TC_IF{++ifcounter:00} : Else",
                    ..mastersObjectActions,
                    //$"TC_C{++actionCounter:00} : Set     : {dummyVarName} : $$Evaluate:($$sprintf:\"SetObject:(%s,$Name).\":##TC_ObjecType)",
                    $"TC_IF{++ifcounter:00} : ENDIF",

                    $"TC_IF{++ifcounter:00} : IF :  (not $$IsEmpty:$MasterId) and ((##TC_ObjecType <> \"Voucher\") or  (##TC_ObjecType = \"Voucher\" AND $REMOTEALTGUID = ##VRemoteId))",
                    $"TC_C50 : LISTADD : {TDLVarName}:$REMOTEALTGUID:$MasterId:{MasterIdVarName}",
                    $"TC_C51 : LISTADD : {TDLVarName}:$REMOTEALTGUID:$Name:{NameVarName}",
                    $"TC_C52 : LISTADD : {TDLVarName}:$REMOTEALTGUID:$GUID:{guidVarName}",
                    $"TC_IF{++ifcounter:00} : Else",
                    $"TC_C{++actionCounter:00} : LISTADD : {TDLVarName}:$REMOTEALTGUID: $$LastImportError:{errorvarName} ",
                     $"TC_IF{++ifcounter:00} : ENDIF",

                ],
            },
        ];
        tDLMessage.Collection = [new() { Name = collectionName, DataSource = $"Variable:{TDLVarName}" }];
    }


    public static List<Models.Request.System> ToSystem(this IEnumerable<Filter> filters)
    {
        return [.. filters.Select(c => new Models.Request.System(c.FilterName!, c.FilterFormulae!))];
    }
    public static List<string> GetFilterNames(this IEnumerable<Filter> filters)
    {
        return [.. filters.Select(c => c.FilterName!)];
    }
}
