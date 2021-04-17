using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using TallyConnector.Models;

namespace TallyConnector
{
    public class Tally
    {
		static readonly HttpClient client = new();
		XmlDocument xmldoc = new();
		private int Port;
		private string BaseURL;

		public string Status;
		public string ReqStatus;

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

		public Dictionary<string,string> CompaniesInfo { get; private set; }

		//Set URL and port during Intialisation
		public Tally(string baseURL, int port)
		{
			Port = port;
			BaseURL = baseURL;

		}

		//If nothing Specified during Intialisation default Url will be localhost running on port 9000
		public Tally()
		{
			Setup("http://localhost", 9000);
		}

		//sets Tally URL with Port
		public void Setup(string baseURL, int port)
        {
			this.BaseURL = baseURL;
			this.Port = port;
			
        }


		//Check whether Tally is running in given Port
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
				Status = $"Tally is not opened \n or Tally is not running in given port - { Port} )\n or Given URL - {BaseURL} \n" +
					e.Message;
			}
		}


		//Gets List of Companies opened in tally and saves in CompaniesInfo Dic
		public async Task<CompaniesList> GetCompaniesList()
		{
			CompaniesList CompList = new();
			await Check();
            if (Status == "Running")
            {
				Models.CusColEnvelope ColEnvelope = new(); //Collection Envelope
				string RName = "List of Companies";

				ColEnvelope.Header = new("Export","Data", RName);  //Configuring Header To get Export data

				Dictionary<string,string> LeftFields = new () //Left Fields
				{
					{ "$NAME", "NAME" }
					
				};
				Dictionary<string, string> RightFields = new() //Right Fields
				{
					{ "$STARTINGFROM", "STARDATE" }
				};

				ColEnvelope.Body.Desc.TDL.TDLMessage = new(rName: RName, fName: RName, topPartName: RName,
                    rootXML: "LISTOFCOMPANIES", colName: $"Form{RName}", lineName: RName, leftFields: LeftFields,
                    rightFields: RightFields, colType: "Company");

				string Reqxml = ColEnvelope.GetXML(true); //Gets XML from Object
				string Resxml = await SendRequest(Reqxml);
                try
                {
					CompList =  Tally.GetObjfromXml<CompaniesList>(Resxml);

                }
                catch (Exception e)
                {

					//throw;
					//return CompList;
				}
				
			}
			CompaniesInfo = CompList.Dic;
			return CompList;
		}

		//Gets all Masters like,Ledgers,Groups  ...etc
		public async Task FetchAllTallyData(string Company = null)
        {
			//If Company Name is provided, fetch information related to particular company
			//- Useful when multiple companies are opened in Tally
			StaticVariables staticVariables = new()
			{
				SVCompany = Company,
			};

			//Gets Groups from Tally
			Dictionary<string, string> fields = new() { { "$NAME", "NAME" } };
			string GrpXml = await GetReportXML("List Of Groups", fields, "Group", staticVariables);
			Groups = GetObjfromXml<GroupsList>(GrpXml).GroupNames;

			//Gets Ledger from Tally
			string LedXml = await  GetReportXML("List Of Ledgers", fields, "Ledger",staticVariables);
			Ledgers = GetObjfromXml<LedgersList>(LedXml).LedgerNames;

			//Gets Cost Categories from Tally
			string CostCategoryXml = await GetReportXML("List Of CostCategories", fields, "CostCategory", staticVariables);
			CostCategories	 = GetObjfromXml<CostCategoriesList>(CostCategoryXml).CostCategories;

			//Gets Cost Centers from Tally
			List<string> Filters = new() { "IsEmployeeGroup", "Payroll" };
			List<string> SystemFilters = new() { "Not $ISEMPLOYEEGROUP", "Not $FORPAYROLL" };
			string CostCenetrXml = await GetReportXML("List Of CostCenters", fields, "CostCenter",
				staticVariables,Filters,SystemFilters);
			CostCenters = GetObjfromXml<CostCentersList>(CostCenetrXml).CostCenters;

			//Gets Stock Groups from Tally
			string StockGroupXml = await GetReportXML("List Of StockGroups", fields, "StockGroups", staticVariables);
			StockGroups = GetObjfromXml<StockGroupsList>(StockGroupXml).StockGroups;

			//Gets Stock Categories from Tally
			string StockCategoryXml = await GetReportXML("List Of StockCategories", fields, "StockCategory", staticVariables);
			StockCategories = GetObjfromXml<StockCategoriesList>(StockCategoryXml).StockCategories;

			//Gets Stock Items from Tally
			string StockItemsXml = await GetReportXML("List Of StockItems", fields, "StockItems", staticVariables);
			StockItems = GetObjfromXml<StockItemsList>(StockItemsXml).StockItems;

			//Gets Godowns from Tally
			string GodownsXml = await GetReportXML("List Of Godowns", fields, "Godown", staticVariables);
			Godowns = GetObjfromXml<GodownsList>(GodownsXml).Godowns;


			//Gets Voucher Types from Tally
			string VoucherTypesXml = await GetReportXML("List Of VoucherTypes", fields, "VoucherTypes", staticVariables);
			VoucherTypes = GetObjfromXml<VoucherTypesList>(VoucherTypesXml).VoucherTypes;

			//Gets Voucher Types from Tally
			string UnitsXml = await GetReportXML("List Of Units", fields, "Units", staticVariables);
			Units = GetObjfromXml<UnitsList>(UnitsXml).Units;

			//Gets Currencies from Tally
			Dictionary<string, string> Currenciesfields = new() { { "$EXPANDEDSYMBOL", "NAME" } };
			string CurrenciesXml = await GetReportXML("List Of Currencies", Currenciesfields, "Currencies", staticVariables);
			Currencies = GetObjfromXml<CurrenciesList>(CurrenciesXml).Currencies;



			//Gets AttendanceType from Tally
			string AttendanceTypesXml = await GetReportXML("List Of AttendanceTypes", fields, "AttendanceType", staticVariables);
			AttendanceTypes = GetObjfromXml<AttendanceTypesList>(AttendanceTypesXml).AttendanceTypes;

			//Gets EmployeeGroups from Tally
			List<string> EmployeeGroupFilters = new() { "IsEmployeeGroup"};
			List<string> EmployeeGroupSystemFilters = new() { "$ISEMPLOYEEGROUP"};
			string EmployeeGroupsXml = await GetReportXML("List Of EmployeeGroups", fields, "CostCenter", staticVariables,
				EmployeeGroupFilters, EmployeeGroupSystemFilters);
			EmployeeGroups = GetObjfromXml<EmployeeGroupList>(EmployeeGroupsXml).EmployeeGroups;

			//Gets Employeees from Tally
			List<string> EmployeeFilters = new() { "IsEmployeeGroup", "Payroll" };
			List<string> EmployeeSystemFilters = new() { "Not $ISEMPLOYEEGROUP", "$FORPAYROLL" };
			string EmployeeesXml = await GetReportXML("List Of Employees", fields, "CostCenter", staticVariables,
				EmployeeFilters, EmployeeSystemFilters);
			Employees = GetObjfromXml<EmployeesList>(EmployeeesXml).Employees;


		}



		//Gets Group From Tally by Name
		public async Task<Group> GetGroup(String GroupName,string Company = null, string FromDate = null, string ToDate = null, string Format=null)
        {
			Group group = (await GetObjFromTally<GroupEnvelope>(GroupName, "Group", Company, FromDate, ToDate, Format)).Body.Data.Message.Group;

			return group;
        }

		//Gets Ledger from Tally by Name
		public async Task<Ledger> GetLedger(String ledgerName, string Company = null, string FromDate = null, string ToDate = null, string Format = null)
		{
			Ledger ledger = (await GetObjFromTally<LedgerEnvelope>(ledgerName, "Group", Company, FromDate, ToDate, Format)).Body.Data.Message.Ledger;

			return ledger;
		}

		//Gets any Tally Object
		public async Task<T> GetObjFromTally<T>(string ObjName, string ObjType,
			string Company = null, string FromDate = null, string ToDate = null, string Format = "XML")
        {
			T Obj;
            try
            {
				string ReqXml = GetObjXML(ObjType, ObjName,Company,FromDate,ToDate,Format);
				string ResXml = await SendRequest(ReqXml);
				Obj = GetObjfromXml<T>(ResXml);
			}
            catch (Exception e)
            {
                throw;
            }
			return Obj;
		}

        private string GetObjXML(string objType, string ObjName, string Company = null,
			string FromDate = null, string ToDate = null, string Format = "XML")
        {
			ObjEnvelope Obj = new();
			string Name = ReplaceXML(ObjName);
			Obj.Header = new(objType, Name);
			StaticVariables staticVariables = new()
            {
				SVCompany= Company,
				SVFromDate= FromDate,
				SVToDate= ToDate,
				SVExportFormat= Format
			};
			Obj.Body.Desc.StaticVariables = staticVariables;

			Obj.Body.Desc.FetchList = new();
			string ObjXML = Obj.GetXML();
			return ObjXML;
		}




        /// <summary>
        /// 
        /// Helper Functions
        /// 
        /// </summary>


        //Helper function to Geberate Report XML
        public async Task<string> GetReportXML(string rName,Dictionary<string,string> Fields,string colType,
			StaticVariables Sv = null, List<string> Filters=null, List<string> SystemFilters = null)
		{
			//LedgersList LedgList = new();
			string Resxml = null;
			if (Status == "Running")
			{
				Models.CusColEnvelope ColEnvelope = new(); //Collection Envelope
				string RName = rName;

				ColEnvelope.Header = new("Export", "Data", RName);  //Configuring Header To get Export data
                if (Sv!=null)
                {
					ColEnvelope.Body.Desc.StaticVariables = Sv;
				}
				
				Dictionary<string, string> LeftFields = Fields;
				Dictionary<string, string> RightFields = new();

				ColEnvelope.Body.Desc.TDL.TDLMessage = new(rName: RName, fName: RName, topPartName: RName,
					rootXML: rName.Replace(" ",""), colName: $"Form{RName}", lineName: RName, leftFields: LeftFields,
					rightFields: RightFields, colType: colType,Filters,SystemFilters);

				string Reqxml = ColEnvelope.GetXML(true); //Gets XML from Object
				Resxml = await SendRequest(Reqxml);
			}
			return Resxml;
		}


		public async Task<LedgersList> GetLedgersList()
		{
			LedgersList LedgList = new();

			if (Status == "Running")
			{
				Models.CusColEnvelope ColEnvelope = new(); //Collection Envelope
				string RName = "List of Ledgers";

				ColEnvelope.Header = new("Export", "Data", RName);  //Configuring Header To get Export data

				Dictionary<string, string> LeftFields = new() //Left Fields
				{
					{ "$NAME", "NAME" }

				};
				Dictionary<string, string> RightFields = new() //Right Fields
				{

				};

				ColEnvelope.Body.Desc.TDL.TDLMessage = new(rName: RName, fName: RName, topPartName: RName,
					rootXML: "LISTOFLEDGERS", colName: $"Form{RName}", lineName: RName, leftFields: LeftFields,
					rightFields: RightFields, colType: "Ledger");

				string Reqxml = ColEnvelope.GetXML(true); //Gets XML from Object
				string Resxml = await SendRequest(Reqxml);

                try
                {
					LedgList = Tally.GetObjfromXml<LedgersList>(Resxml);
					Ledgers = LedgList.LedgerNames;

				}
                catch (Exception e)
                {

                    //throw;
                }
			}
			return LedgList;
		}


		//Posts Xml to Tally
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
					Resxml = ReplaceXMLText(Resxml);
					return Resxml;
				}
				catch (Exception e)
				{
					ReqStatus = e.Message;
					return ReqStatus;
				}
			}
			return Resxml;
		}

		//Helper method to escape text for xml
		public static string ReplaceXML(string strText)
		{
			string result = null;
			if (strText != null)
			{
				result = strText.Replace("&", "&amp;");
				result = result.Replace("'", "&apos;");
				result = result.Replace("\"\"", "&quot;");
				result = result.Replace(">", "&gt;");
				result = result.Replace("<", "&lt;");
			}
			return result;
		}
		
		//Helper method to convert escaped characters to text
		public static string ReplaceXMLText(string strXmlText)
		{
			string result = null;
			if (strXmlText != null)
			{
				result = strXmlText.Replace("&#x4;", "");
				result = result.Replace("&#4;", "");
				//result = strXmlText.Replace("&amp;", "&");
				//result = result.Replace( "&apos;", "'");
				//result = result.Replace("&quot;","\"\"");
				//result = result.Replace("&gt;", ">");
			}
			return result;
		}

		//Converts to given object from Xml
		public static T GetObjfromXml<T>(string Xml)
        {
			XmlSerializer XMLSer = new XmlSerializer(typeof(T));
			StringReader XmlStream = new StringReader(Xml);
            T obj = (T)XMLSer.Deserialize(XmlStream);
			return obj;
		}
	}
}
