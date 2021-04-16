using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TallyConnector.Models;

namespace TallyConnector
{
    public class Tally
    {
		static readonly HttpClient client = new();
		private int Port;
		private string BaseURL;

		public string Status;
        public Tally(string baseURL, int port)
        {
            Port = port;
            BaseURL = baseURL;
			
        }
		public Tally()
        {
			Setup("http://localhost", 9000);
        }

     

        //Gets Full Url from Baseurl and Port
        private string FullURL { get { return BaseURL + ":" + Port; } }

		public List<string> Groups { get; private set; }
		public List<string> Ledgers { get; private set; }
		public List<string> Parents { get; private set; }
		public List<string> CostCategories { get; private set; }
		public List<string> CostCenters { get; private set; }
		public List<string> StockGroups { get; private set; }
		public List<string> StockCategories { get; private set; }

		public List<string> StockItems { get; private set; }
		public List<string> Godowns { get; private set; }
		public List<string> VoucherTypes { get; private set; }
		public List<string> Units { get; private set; }
		public List<string> Currencies { get; private set; }

		public List<string> AttendanceTypes { get; private set; }
		public List<string> EmployeeGroups { get; private set; }
		public List<string> Employees { get; private set; }


		//sets Tally URL with Port
		public void Setup(string baseURL, int port)
        {
			this.BaseURL = baseURL;
			this.Port = port;
			
        }

		public async Task Check()
        {
			try
			{
				HttpResponseMessage response = await client.GetAsync(FullURL);
				response.EnsureSuccessStatusCode();
				string res = await response.Content.ReadAsStringAsync();
				
				Status = "Running";
			}
			catch (HttpRequestException ex)
			{
				HttpRequestException e = ex;
				Status = $"Tally is not opened \nor Tally is not running in given port - { Port} )\nor Given URL - {BaseURL} \n" +
					e.Message;
			}
		}

		public async Task GetCompaniesList()
		{
			await Check();
            if (Status == "Running")
            {
				Models.CusColEnvelope ColEnvelope = new(); //Collection Envelope
				string RName = "List of Companies";

				ColEnvelope.Header = new("Export","Data", RName);  //Configuring Header To get Export data

				Dictionary<string,string> LeftFields = new ()
				{
					{ "$NAME", "NAME" },
					{ "$STARTINGFROM", "STARDATE" }
				};
				Dictionary<string, string> RightFields = new()
				{
					
				};
				ColEnvelope.Body.Desc.TDL.TDLMessage = new(rName: RName, fName: RName, topPartName: RName,
                    rootXML: "LISTOFCOMPANIES", colName: $"Form{RName}", lineName: RName, leftFields: LeftFields,
                    rightFields: RightFields, colType: "Company");

				string Reqxml = ColEnvelope.GetXML(true); //Gets XML from Object
				string Resxml = await SendRequest(Reqxml);
				CompaniesList Cl = new();
				Cl = (CompaniesList)Cl.GetObj(Resxml);
				//return Cl;
			}
		}

		public async Task<string> SendRequest(string SXml)
		{
			string Resxml = "";
			await Check();
			if (Status == "Running")
			{
				try
				{
					StringContent TXML = new StringContent(SXml, Encoding.Default, "application/xml");
					HttpResponseMessage Res = await client.PostAsync(FullURL, TXML);
					Res.EnsureSuccessStatusCode();
					Resxml = await Res.Content.ReadAsStringAsync();
					//_ = NumberFormatInfo.CurrentInfo.CurrencySymbol;
					return Resxml;
				}
				catch (Exception e)
				{
					Status = e.Message;
					return Resxml;
				}
			}
			return Resxml;
		}

	}
}
