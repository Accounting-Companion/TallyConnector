namespace TallyConnector.Core.Models.Interfaces;
public interface ICreateTallyObject
{
    public string? RemoteId { get; set; }


}
public interface ITallyObject
{
    public Action Action { get; set; }

   // public   List<string> FetchList { get; set; }

    void PrepareForExport();
}
public interface ITallyReport
{

}