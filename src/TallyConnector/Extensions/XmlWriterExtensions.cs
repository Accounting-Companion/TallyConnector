using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TallyConnector.Extensions;
public static class XmlWriterExtensions
{
    public static async Task CreateTallyHeader(this XmlWriter writer,
                                               string requestType,
                                               string requestDataType,
                                               string id)
    {
        await writer.WriteStartElementAsync("HEADER");
        await writer.WriteStartElementAsync("VERSION");
        await writer.WriteStringAsync("1");
        await writer.WriteEndElementAsync();
        await writer.WriteStartElementAsync("TALLYREQUEST");

        await writer.WriteStringAsync(requestType);
        await writer.WriteEndElementAsync();
        await writer.WriteStartElementAsync("TYPE");

        await writer.WriteStringAsync(requestDataType);
        await writer.WriteEndElementAsync();
        await writer.WriteStartElementAsync("ID");
        await writer.WriteStringAsync(id);
        await writer.WriteEndElementAsync();
        await writer.WriteEndElementAsync();
    }
    public static async Task CreateExportCollectionHeader(this XmlWriter writer,
                                                          string id)
    {
        await writer.CreateTallyHeader("EXPORT", "COLLECTION", id);
    }
    public static async Task CreateExportReportHeader(this XmlWriter writer,
                                                      string id)
    {
        await writer.CreateTallyHeader("EXPORT", "DATA", id);
    }
    public static async Task WriteStartElementAsync(this XmlWriter writer,
                                                      string name)
    {
        await writer.WriteStartElementAsync(null, name, null);
    }
    public static async Task CreateStaticVariables(this XmlWriter writer,
                                                    string? companyName,
                                                    DateTime? fromDate,
                                                    DateTime? toDate)
    {
        await writer.WriteStartElementAsync("STATICVARIABLES");
        if (!string.IsNullOrEmpty(companyName))
        {
            await writer.WriteStartElementAsync("SVEXPORTFORMAT");
            await writer.WriteStringAsync("$$SysName:XML");
            await writer.WriteEndElementAsync();
        }
        await writer.WriteEndElementAsync();
    }

}
