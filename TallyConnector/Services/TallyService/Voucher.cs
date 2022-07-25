namespace TallyConnector.Services;
public partial class TallyService
{
    public async Task<Vchtype> GetVoucherAsync<Vchtype>(string lookupValue,
                                                        VoucherRequestOptions? voucherRequestOptions = null) where Vchtype : Voucher
    {

        return await GetObjectAsync<Vchtype>(lookupValue, voucherRequestOptions);
    }

    public async Task<TallyResult> PostVoucherAsync<TVch>(TVch voucher, PostRequestOptions? postRequestOptions = null) where TVch : Voucher
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