using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TallyConnector.Services;
public partial class TallyService
{
    public async Task<Voucher> GetVoucherAsync<Vchtype>(string LookupValue,
                                                        VoucherRequestOptions? voucherRequestOptions = null) where Vchtype : Voucher
    {

        return new();
    }

    public async Task<TallyResult> PostVoucher<TVch>(TVch voucher, PostRequestOptions? postRequestOptions = null) where TVch : Voucher
    {
        postRequestOptions ??= new();
        postRequestOptions.XMLAttributeOverrides ??= new();

        if (voucher.View != VoucherViewType.AccountingVoucherView)
        {
            XmlAttributes xmlattribute = new();
            xmlattribute.XmlElements.Add(new XmlElementAttribute() { ElementName = "LEDGERENTRIES.LIST" });
            postRequestOptions.XMLAttributeOverrides.Add(typeof(Voucher), "Ledgers", xmlattribute);
        }

        var result = await PostObjectToTallyAsync(Object: voucher, postRequestOptions);

        return result;
    }
}