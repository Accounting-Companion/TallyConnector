using System.IO;
using TallyConnector.Core.Models.Common.Request;

namespace TallyConnector.Services;
//[GenerateHelperMethod<Group, Group, GroupCreate>]
[GenerateHelperMethod<GroupCreate, Group, GroupCreate>(GenerationMode = HelperMethodGenerationMode.Both)]

public partial class TallyService : BaseTallyService
{

}
public partial class TallyService
{
    public static Field[] GetGroupTDLReportFields_()
    {
        TallyConnector.Core.Models.Common.Request.Field[] fields = new TallyConnector.Core.Models.Common.Request.Field[2];
        fields[0] = new("Name", "NAME", "$Name", null);
        fields[1] = new("Parent", "PARENT", "$Parent", null);

        return fields;
    }
    public async Task<string> Getenvelope_2XML(XmlWriter writer, string value)
    {
        await writer.WriteStartDocumentAsync();
        //foreach (var value in Valuess)
        //{
        //    await writer.WriteStartElementAsync(null, "test", null);
        //    await writer.WriteEndElementAsync();
        //}
        await writer.WriteStartElementAsync("dsfdgfg", "test", null);
        await writer.WriteStringAsync("test");
        await writer.WriteElementStringAsync("dsfdgfg", "test", null, "");
        Field[] fields = new Field[5];
        //fields[0] = new("");
        writer.WriteAttributeString("", "", "");
        writer.WriteEndAttribute();
        //bool c = false;
        //c?.ToString();
        await writer.WriteEndElementAsync();
        await writer.WriteEndDocumentAsync();
        await writer.FlushAsync();
        return "";
    }
}
