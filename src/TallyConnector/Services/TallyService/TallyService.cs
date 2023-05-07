namespace TallyConnector.Services;
[GenerateHelperMethod<Group, Group, GroupCreate>]
[GenerateHelperMethod<GroupCreate, BaseTallyGroup, GroupCreate>]

public partial class TallyService : BaseTallyService
{

}
public partial class TallyService
{
    public async Task<string> Getenvelope_2XML(XmlWriter writer, string value)
    {
        await writer.WriteStartDocumentAsync();
        //foreach (var value in Valuess)
        //{
        //    await writer.WriteStartElementAsync(null, "test", null);
        //    await writer.WriteEndElementAsync();
        //}
        await writer.WriteStartElementAsync(null, "test", null);
        await writer.WriteStringAsync("test");
        writer.WriteStartAttribute( "test");
        writer.WriteEndAttribute();
        //writer.writestr()
        await writer.WriteEndElementAsync();
        await writer.WriteEndDocumentAsync();
        await writer.FlushAsync();
        return "";
    }
}
