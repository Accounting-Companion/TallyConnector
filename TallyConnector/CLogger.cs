using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TallyConnector
{
    public class CLogger
    {
        private ILogger Logger;

        public CLogger(ILogger logger = null)
        {
            this.Logger = logger?? NullLogger.Instance;
        }

        public void SetupLog(string baseURL, int port, string company, string fromDate, string toDate)
        {
            Logger.LogInformation($"Setting up Tally to connect at \"{baseURL}:{port}\"");
            if (company != null)
            {
                Logger.LogInformation($"Setting up Tally get data of Company - \"{company}\"{(fromDate != null ? $" From Date - {fromDate}" : null)} {(toDate != null ? $" To Date - {toDate}" : null)}");

            }
        }

        public void TallyCheck(string fullURL)
        {
            Logger.LogTrace($"Sending test request to tally ... at \"{fullURL}\"");
        }

        public void TallyNotRunning(string fullURL)
        {
            Logger.LogError($"Tally is not running at \"{fullURL}\" at {DateTime.Now}");
        }

        public void TallyError(string fullURL, string message)
        {
            Logger.LogError($"Unable to send request to tally at \"{fullURL}\" at {DateTime.Now} Error occured");
        }

        public void TallyRunning(string fullURL)
        {
            Logger.LogTrace($"Tally is running at \"{fullURL}\"");
        }

        internal void TallyReqStart(string Reqtype)
        {
            Logger.LogTrace($"Sending Request to tally to get {Reqtype}");
        }

        internal void TallyReqCompleted(string Reqtype)
        {
            Logger.LogTrace($"Sucessfully Send Request to tally and  {Reqtype} received succesfuly");
        }

        internal void RequestError(string Reqtype, string message)
        {
            Logger.LogError($"Unable to get {Reqtype} in tally Error - {message}");
        }

        internal void TallyRequest(string sXml)
        {
            Logger.LogDebug($"Sending request to tally witth payload - {sXml}");
        }

        internal void TallyResponse(string resxml)
        {
            Logger.LogDebug($"Received respose from tally  - {resxml.Replace("\r\n","")}");
        }

        internal void TallyReqError(string message)
        {
            Logger.LogError($"Error while sending request to tally - {message}");
        }
    }
}
