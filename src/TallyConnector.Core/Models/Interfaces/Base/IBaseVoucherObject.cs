
namespace TallyConnector.Core.Models.Interfaces;

public interface IBaseVoucherObject : IBaseObject
{
   
}
public interface IVoucherObject :IBaseVoucherObject
{
    DateTime Date { get; set; }
    string VoucherType { get; set; }
    bool IsCancelled { get; set; }
    bool IsOptional { get; set; }
}
