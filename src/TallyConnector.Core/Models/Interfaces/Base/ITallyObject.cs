namespace TallyConnector.Core.Models;

public interface ITallyObject : IBaseTallyObject
{
    ulong AlterId { get; set; }
    ulong MasterId { get; set; }

}


/// <summary>
/// Interface for BaseTallyService
/// </summary>
public interface IBaseTallyService
{
    /// <summary>
    /// Coonfigure Tally Url and port
    /// </summary>
    /// <param name="url">Url on which Tally is running</param>
    /// <param name="port">Port on Which Tally is runnig</param>
    void Setup(string url, int port);

    /// <summary>
    /// Checks Whether Tally is running at Given Url and port
    /// </summary>
    /// <returns>If Tally is running - true<br/>If Tally is not running - false</returns>
    Task<bool> CheckAsync(CancellationToken token = default);

    /// <summary>
    /// All future requests will be send to company mentioned here
    /// irrespective of active company in Tally <br/>
    /// you can also overide company by mentioning in request options in Request
    /// </summary>
    /// <param name="company">instance of company</param>
    void SetCompany(Company company);


    /// <summary>
    /// A helper function to send request to Tally
    /// </summary>
    /// <param name="xml">xml that is required to send to Tally</param>
    /// <param name="requestType">xml that is required to send to Tally</param>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="TallyConnectivityException"></exception>
    Task<TallyResult> SendRequestAsync(string? xml = null, string? requestType = null, CancellationToken token = default);

    /// <summary>
    /// Checks whether xml has linerror and returns error
    /// if noerror returns null
    /// </summary>
    /// <param name="ResXml"></param>
    /// <returns></returns>
    string? CheckTallyError(string ResXml);

    /// <summary>
    /// Parse response in XML and return TallyResult
    /// </summary>
    /// <param name="tallyResult"></param>
    /// <returns></returns>
    TallyResult ParseResponse(TallyResult tallyResult);

    /// <summary>
    /// Get License Information and Other Basic Info from Tally
    /// </summary>
    /// <returns></returns>
    Task<LicenseInfo> GetLicenseInfoAsync(CancellationToken token = default);

    /// <summary>
    /// Gets Active Simple Company name from Tally
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<string> GetActiveSimpleCompanyNameAsync(CancellationToken token = default);



    ///// <summary>
    ///// Get Masters Statistics form Tally
    ///// </summary>
    ///// <param name="requestOptions">Request options to configure tally</param>
    ///// <param name="token">Cancellation Token</param>
    ///// <returns>List of Master Statistics</returns>
    //Task<List<MasterStatistics>> GetMasterStatisticsAsync(BaseRequestOptions? requestOptions = null, CancellationToken token = default);

    ///// <summary>
    ///// Get Statistics of Vouchers from Tally <br/>
    ///// We can metion specific period using From and To Date in request options
    ///// </summary>
    ///// <param name="requestOptions">Request options to configure tally</param>
    ///// <param name="token">Cancellation Token</param>
    ///// <returns></returns>
    //Task<List<VoucherStatistics>> GetVoucherStatisticsAsync(DateFilterRequestOptions? requestOptions = null, CancellationToken token = default);


    Task PopulateDefaultOptions(RequestEnvelope requestEnvelope, CancellationToken token = default);

    ///// <summary>
    ///// 
    ///// </summary>
    ///// <param name="requestOprions"></param>
    ///// <param name="token"></param>
    ///// <returns></returns>
    //Task<List<AutoColVoucherTypeStat>> GetVoucherStatisticsAsync(AutoColumnReportPeriodRequestOptions requestOptions, CancellationToken token = default);

}