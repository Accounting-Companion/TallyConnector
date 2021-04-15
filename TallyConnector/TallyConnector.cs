using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Windows;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using TallyConnector.Models;

namespace TallyConnector
{

	public class Tally
	{
		static readonly HttpClient client = new HttpClient();
		public static string BASE_URL = "http://localhost";


		//public  int Tally_Port;
			//private string Tally_status;

			//private string Company;

		public static string STATUS
			{
				get;
				set;

			}
		public static string COMPANY { get; set; }
		public static string COMPANY_STARTDATE { get; set; }

		public static int PORT { get; set; }
		public string FULL_URL
			{
				get
				{

					return BASE_URL + ":" + PORT;
				}
			}
		// private List<string> Ledgers_List = new List<string>();
			// //private List<string> Ledgers_List = new List<string>();
			// private List<string> Parents_List = new List<string>();
			// private List<string> Groups_List = new List<string>();

			// private List<string> StockGroups_List = new List<string>();
			//// private List<string> StockCategories_List = new List<string>();
			// private List<string> StockItems_List = new List<string>();
			// //private List<string> Godowns_List = new List<string>();
			// private List<string> VoucherTypes_List = new List<string>();
			// private List<string> Units_List = new List<string>();
			//private List<string> Currencies_List = new List<string>();
		private IDictionary<string, Double> EInfo = new Dictionary<string, double>();



		public static List<string> LEDGERS
			{
				get; private set;
			}
		public static List<string> PARENTS
			{
				get; private set;
			}
		public static List<string> GROUPS
			{
				get; private set;
			}
		public static List<string> COSTCATEGORY
			{
				get; private set;
			}
		public static List<string> COSTCENTER
			{
				get; private set;
			}
		public static List<string> STOCKGROUPS
			{
				get; private set;
			}
		public static List<string> STOCKCATEGORY { get; private set; }

		public static List<string> STOCKITEMS
			{
				get; private set;
			}
		public static List<string> GODOWN
			{
				get; private set;
			}
		public static List<string> VOUCHERTYPES
			{
				get; private set;
			}
		public static List<string> UNITS
			{
				get; private set;
			}
		public static List<string> CURRENCIES
			{
				get; private set;
			}

		public static List<string> ATTENDANCETYPE
			{
				get; private set;
			}
		public static List<string> EMPLOYEEGROUP
			{
				get; private set;
			}
		public static List<string> EMPLOYEE
			{
				get; private set;
			}

		public static IDictionary<string, Double> EINFO
			{
				get; private set;
			}

		//private List<string> Groups_TagList = new List<string>() {"N" };
		public static Dictionary<string, List<string>> COMPANYINFO { get; set; }
		public static List<string> COMPANYLIST { get; set; }

		public static async Task Check()
			{
				XmlDocument xDoc = new XmlDocument();
				Tally Ctally = new Tally();
				try
				{
					HttpResponseMessage response = await client.GetAsync(Ctally.FULL_URL);
					response.EnsureSuccessStatusCode();
					xDoc.LoadXml(await response.Content.ReadAsStringAsync());
					xDoc.GetElementsByTagName("RESPONSE");
					STATUS = "Running";
				}
				catch (HttpRequestException ex)
				{
					HttpRequestException e = ex;
					STATUS = e.Message + "\nTally is not running in given port(" + PORT + ")\n or \n Tally is not opened";
				}
			}

		public static async Task GetCompanies()
			{
				XmlDocument Xmldoc = new XmlDocument();
				Tally tally = new Tally();
				string Xml = "<ENVELOPE><HEADER><VERSION>1</VERSION><TALLYREQUEST>Export</TALLYREQUEST><TYPE>Data</TYPE><ID>List of Companies</ID></HEADER><BODY><DESC><STATICVARIABLES><SVEXPORTFORMAT>$$SysName:XML</SVEXPORTFORMAT></STATICVARIABLES><TDL><TDLMESSAGE><REPORT NAME=\"List of Companies\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <FORMS>List of Companies</FORMS>  </REPORT><FORM NAME=\"List of Companies\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <TOPPARTS>List of Companies</TOPPARTS>  <XMLTAG>\"List of Companies\"</XMLTAG>  </FORM><PART NAME=\"List of Companies\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <TOPLINES>List of Companies</TOPLINES>  <REPEAT>List of Companies : Collection of Companies</REPEAT>  <SCROLLED>Vertical</SCROLLED>  </PART><LINE NAME=\"List of Companies\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <LEFTFIELDS>Name</LEFTFIELDS> <RIGHTFIELDS>StartDate</RIGHTFIELDS>  </LINE><FIELD NAME=\"Name\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <SET>$Name</SET>  <XMLTAG>\"NAME\"</XMLTAG>  </FIELD><FIELD NAME=\"StartDate\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <SET>$StartingFrom</SET>  <XMLTAG>\"StartDate\"</XMLTAG>  </FIELD><COLLECTION NAME=\"Collection of Companies\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <TYPE>Company</TYPE>  </COLLECTION> </TDLMESSAGE> </TDL></DESC></BODY></ENVELOPE>";
				Xmldoc.LoadXml(await tally.SendRequest(Xml));
				XmlNodeList list = Xmldoc.GetElementsByTagName("NAME");
				XmlNodeList list2 = Xmldoc.GetElementsByTagName("STARTDATE");
				List<string> CList = new List<string>();
				List<string> Info = new List<string>();
				Dictionary<string, List<string>> CDic = new Dictionary<string, List<string>>();
				if (list.Count != 0)
				{
					for (int i = 0; i < list.Count; i++)
					{
						CList.Add(list[i].InnerText);
						Info.Add(list2[i].InnerText);
						CDic[list[i].InnerText] = Info;
					}
					COMPANYLIST = CList;
					COMPANYINFO = CDic;
				}
				else
				{
					COMPANYLIST = CList;
				}

			}

		public static async Task GetTallyData()
			{
				Tally tally = new Tally();
				await Check();
				if (STATUS == "Running")
				{
					try
					{
						string LXml = "<ENVELOPE><HEADER><VERSION>1</VERSION><TALLYREQUEST>Export</TALLYREQUEST><TYPE>Data</TYPE><ID>List of Ledgers</ID></HEADER><BODY><DESC><STATICVARIABLES><SVEXPORTFORMAT>$$SysName:XML</SVEXPORTFORMAT><SVCURRENTCOMPANY>" + COMPANY + "</SVCURRENTCOMPANY><SVFROMDATE TYPE=\"Date\">" + COMPANY_STARTDATE + "</SVFROMDATE></STATICVARIABLES><TDL><TDLMESSAGE><REPORT NAME=\"List of Ledgers\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <FORMS>List of Ledgers</FORMS></REPORT><FORM NAME=\"List of Ledgers\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <TOPPARTS>List of Ledgers</TOPPARTS><XMLTAG>\"List of Ledgers\"</XMLTAG></FORM><PART NAME=\"List of Ledgers\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <TOPLINES>List of Ledgers</TOPLINES><REPEAT>List of Ledgers : Collection of Ledgers</REPEAT><SCROLLED>Vertical</SCROLLED></PART><LINE NAME=\"List of Ledgers\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <LEFTFIELDS>Name</LEFTFIELDS><LEFTFIELDS>Parent</LEFTFIELDS></LINE><FIELD NAME=\"Name\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <SET>$Name</SET><XMLTAG>\"NAME\"</XMLTAG></FIELD><FIELD NAME=\"Parent\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <SET>$Parent</SET><XMLTAG>\"Parent\"</XMLTAG></FIELD><COLLECTION NAME=\"Collection of Ledgers\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <TYPE>Ledger</TYPE></COLLECTION> </TDLMESSAGE> </TDL></DESC></BODY></ENVELOPE>";
						string GXml = "<ENVELOPE><HEADER><VERSION>1</VERSION><TALLYREQUEST>Export</TALLYREQUEST><TYPE>Data</TYPE><ID>List of Ledgers</ID></HEADER><BODY><DESC><STATICVARIABLES><SVEXPORTFORMAT>$$SysName:XML</SVEXPORTFORMAT><SVCURRENTCOMPANY>" + COMPANY + "</SVCURRENTCOMPANY><SVFROMDATE TYPE=\"Date\">" + COMPANY_STARTDATE + "</SVFROMDATE></STATICVARIABLES><TDL><TDLMESSAGE><REPORT NAME=\"List of Ledgers\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <FORMS>List of Ledgers</FORMS></REPORT><FORM NAME=\"List of Ledgers\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <TOPPARTS>List of Ledgers</TOPPARTS><XMLTAG>\"List of Ledgers\"</XMLTAG></FORM><PART NAME=\"List of Ledgers\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <TOPLINES>List of Ledgers</TOPLINES><REPEAT>List of Ledgers : Collection of Ledgers</REPEAT><SCROLLED>Vertical</SCROLLED></PART><LINE NAME=\"List of Ledgers\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <LEFTFIELDS>Name</LEFTFIELDS></LINE><FIELD NAME=\"Name\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <SET>$Name</SET><XMLTAG>\"GROUPNAME\"</XMLTAG></FIELD><COLLECTION NAME=\"Collection of Ledgers\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <TYPE>GROUP</TYPE></COLLECTION> </TDLMESSAGE> </TDL></DESC></BODY></ENVELOPE>";
						string CCatXml = "<ENVELOPE> <HEADER><VERSION>1</VERSION><TALLYREQUEST>Export</TALLYREQUEST><TYPE>Data</TYPE><ID>List of Cost Category</ID> </HEADER> <BODY><DESC> <STATICVARIABLES><SVEXPORTFORMAT>$$SysName:XML</SVEXPORTFORMAT><SVCURRENTCOMPANY>" + COMPANY + "</SVCURRENTCOMPANY><SVFROMDATE TYPE=\"Date\">" + COMPANY_STARTDATE + "</SVFROMDATE> </STATICVARIABLES> <TDL><TDLMESSAGE> <REPORT NAME=\"List of Cost Category\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"><FORMS>List of Cost Category</FORMS> </REPORT> <FORM NAME=\"List of Cost Category\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"><TOPPARTS>List of Cost Category</TOPPARTS><XMLTAG>\"List of Cost Category\"</XMLTAG> </FORM> <PART NAME=\"List of Cost Category\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"><TOPLINES>List of Cost Category</TOPLINES><REPEAT>List of Cost Category : Collection of Cost Category</REPEAT><SCROLLED>Vertical</SCROLLED> </PART> <LINE NAME=\"List of Cost Category\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"><LEFTFIELDS>Name</LEFTFIELDS> </LINE> <FIELD NAME=\"Name\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"><SET>$Name</SET><XMLTAG>\"COSTCATEGORY\"</XMLTAG> </FIELD> <COLLECTION NAME=\"Collection of Cost Category\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"><TYPE>COSTCATEGORY</TYPE> </COLLECTION></TDLMESSAGE> </TDL></DESC> </BODY></ENVELOPE>";
						string CCenXml = "<ENVELOPE><HEADER><VERSION>1</VERSION><TALLYREQUEST>Export</TALLYREQUEST><TYPE>Data</TYPE><ID>List of Cost Center</ID></HEADER><BODY><DESC><STATICVARIABLES><SVEXPORTFORMAT>$$SysName:XML</SVEXPORTFORMAT><SVCURRENTCOMPANY>" + COMPANY + "</SVCURRENTCOMPANY><SVFROMDATE TYPE=\"Date\">" + COMPANY_STARTDATE + "</SVFROMDATE></STATICVARIABLES><TDL><TDLMESSAGE><REPORT NAME=\"List of Cost Center\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"><FORMS>List of Cost Center</FORMS></REPORT><FORM NAME=\"List of Cost Center\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"><TOPPARTS>List of Cost Center</TOPPARTS><XMLTAG>\"List of Cost Centers\"</XMLTAG></FORM><PART NAME=\"List of Cost Center\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"><TOPLINES>List of Cost Center</TOPLINES><REPEAT>List of Cost Center : Collection of Cost Center</REPEAT><SCROLLED>Vertical</SCROLLED></PART><LINE NAME=\"List of Cost Center\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"><LEFTFIELDS>Name</LEFTFIELDS></LINE><FIELD NAME=\"Name\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"><SET>$Name</SET><XMLTAG>\"COSTCENTER\"</XMLTAG></FIELD><COLLECTION NAME=\"Collection of Cost Center\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"><TYPE>COSTCENTER</TYPE><TYPE>COSTCENTER</TYPE><FILTERS>IsEmployeeGroup</FILTERS><FILTERS>payroll</FILTERS></COLLECTION><SYSTEM TYPE=\"Formulae\" NAME=\"IsEmployeeGroup\"> Not $ISEMPLOYEEGROUP</SYSTEM><SYSTEM TYPE=\"Formulae\" NAME=\"payroll\"> Not $FORPAYROLL</SYSTEM></TDLMESSAGE></TDL></DESC></BODY></ENVELOPE>";
						string SGXml = "<ENVELOPE> <HEADER><VERSION>1</VERSION><TALLYREQUEST>Export</TALLYREQUEST><TYPE>Data</TYPE><ID>List of Stock Groups</ID> </HEADER><BODY><DESC><STATICVARIABLES><SVEXPORTFORMAT>$$SysName:XML</SVEXPORTFORMAT><SVCURRENTCOMPANY>" + COMPANY + "</SVCURRENTCOMPANY><SVFROMDATE TYPE=\"Date\">" + COMPANY_STARTDATE + "</SVFROMDATE></STATICVARIABLES><TDL><TDLMESSAGE><REPORT NAME=\"List of Stock Groups\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <FORMS>List of Stock Groups</FORMS></REPORT><FORM NAME=\"List of Stock Groups\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <TOPPARTS>List of Stock Groups</TOPPARTS><XMLTAG>\"List of STOCK GROUPS\"</XMLTAG></FORM><PART NAME=\"List of Stock Groups\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <TOPLINES>List of Stock Groups</TOPLINES><REPEAT>List of Stock Groups : Collection of Stock Groups</REPEAT><SCROLLED>Vertical</SCROLLED></PART><LINE NAME=\"List of Stock Groups\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <LEFTFIELDS>Name</LEFTFIELDS></LINE><FIELD NAME=\"Name\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <SET>$Name</SET><XMLTAG>\"STOCKGROUPNAME\"</XMLTAG></FIELD><COLLECTION NAME=\"Collection of Stock Groups\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <TYPE>STOCK GROUPS</TYPE></COLLECTION> </TDLMESSAGE> </TDL></DESC></BODY></ENVELOPE>";
						string SCXml = "<ENVELOPE><HEADER> <VERSION>1</VERSION> <TALLYREQUEST>Export</TALLYREQUEST> <TYPE>Data</TYPE> <ID>List of Stock Category</ID></HEADER><BODY><DESC><STATICVARIABLES><SVEXPORTFORMAT>$$SysName:XML</SVEXPORTFORMAT><SVCURRENTCOMPANY>" + COMPANY + "</SVCURRENTCOMPANY><SVFROMDATE TYPE=\"Date\">" + COMPANY_STARTDATE + "</SVFROMDATE></STATICVARIABLES><TDL><TDLMESSAGE><REPORT NAME=\"List of Stock Category\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <FORMS>List of Stock Category</FORMS></REPORT><FORM NAME=\"List of Stock Category\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <TOPPARTS>List of Stock Category</TOPPARTS><XMLTAG>\"List of Stock Category\"</XMLTAG></FORM><PART NAME=\"List of Stock Category\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <TOPLINES>List of Stock Category</TOPLINES><REPEAT>List of Stock Category : Collection of Stock Category</REPEAT><SCROLLED>Vertical</SCROLLED></PART><LINE NAME=\"List of Stock Category\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <LEFTFIELDS>Name</LEFTFIELDS> </LINE><FIELD NAME=\"Name\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <SET>$Name</SET><XMLTAG>\"STOCKCATEGORY\"</XMLTAG></FIELD><COLLECTION NAME=\"Collection of Stock Category\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <TYPE>STOCKCATEGORY</TYPE></COLLECTION> </TDLMESSAGE> </TDL></DESC></BODY></ENVELOPE>";
						string SIXml = "<ENVELOPE> <HEADER><VERSION>1</VERSION><TALLYREQUEST>Export</TALLYREQUEST><TYPE>Data</TYPE><ID>List of Stock Groups</ID> </HEADER><BODY><DESC><STATICVARIABLES><SVEXPORTFORMAT>$$SysName:XML</SVEXPORTFORMAT><SVCURRENTCOMPANY>" + COMPANY + "</SVCURRENTCOMPANY><SVFROMDATE TYPE=\"Date\">" + COMPANY_STARTDATE + "</SVFROMDATE></STATICVARIABLES><TDL><TDLMESSAGE><REPORT NAME=\"List of Stock Groups\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <FORMS>List of Stock Groups</FORMS></REPORT><FORM NAME=\"List of Stock Groups\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <TOPPARTS>List of Stock Groups</TOPPARTS><XMLTAG>\"List of STOCK GROUPS\"</XMLTAG></FORM><PART NAME=\"List of Stock Groups\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <TOPLINES>List of Stock Groups</TOPLINES><REPEAT>List of Stock Groups : Collection of Stock Groups</REPEAT><SCROLLED>Vertical</SCROLLED></PART><LINE NAME=\"List of Stock Groups\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <LEFTFIELDS>Name</LEFTFIELDS></LINE><FIELD NAME=\"Name\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <SET>$Name</SET><XMLTAG>\"STOCKITEM\"</XMLTAG></FIELD><COLLECTION NAME=\"Collection of Stock Groups\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <TYPE>STOCK ITEMS</TYPE></COLLECTION> </TDLMESSAGE> </TDL></DESC></BODY></ENVELOPE>";
						string GoXml = "<ENVELOPE><HEADER><VERSION>1</VERSION><TALLYREQUEST>Export</TALLYREQUEST><TYPE>Data</TYPE><ID>List of Godown</ID></HEADER><BODY><DESC><STATICVARIABLES><SVEXPORTFORMAT>$$SysName:XML</SVEXPORTFORMAT><SVCURRENTCOMPANY>" + COMPANY + "</SVCURRENTCOMPANY><SVFROMDATE TYPE=\"Date\">" + COMPANY_STARTDATE + "</SVFROMDATE></STATICVARIABLES><TDL><TDLMESSAGE><REPORT NAME=\"List of Godown\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"><FORMS>List of Godown</FORMS></REPORT><FORM NAME=\"List of Godown\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"><TOPPARTS>List of Godown</TOPPARTS><XMLTAG>\"List of Godowns\"</XMLTAG></FORM><PART NAME=\"List of Godown\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"><TOPLINES>List of Godown</TOPLINES><REPEAT>List of Godown : Collection of Godown</REPEAT><SCROLLED>Vertical</SCROLLED></PART><LINE NAME=\"List of Godown\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"><LEFTFIELDS>Name</LEFTFIELDS></LINE><FIELD NAME=\"Name\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"><SET>$Name</SET><XMLTAG>\"GODOWN\"</XMLTAG></FIELD><COLLECTION NAME=\"Collection of Godown\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"><TYPE>GODOWN</TYPE></COLLECTION></TDLMESSAGE></TDL></DESC></BODY></ENVELOPE>";
						string VXml = "<ENVELOPE> <HEADER><VERSION>1</VERSION><TALLYREQUEST>Export</TALLYREQUEST><TYPE>Data</TYPE><ID>List of VOUCHERS</ID> </HEADER><BODY><DESC><STATICVARIABLES><SVEXPORTFORMAT>$$SysName:XML</SVEXPORTFORMAT><SVCURRENTCOMPANY>" + COMPANY + "</SVCURRENTCOMPANY><SVFROMDATE TYPE=\"Date\">" + COMPANY_STARTDATE + "</SVFROMDATE></STATICVARIABLES><TDL><TDLMESSAGE><REPORT NAME=\"List of VOUCHERS\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <FORMS>List of VOUCHERS</FORMS></REPORT><FORM NAME=\"List of VOUCHERS\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <TOPPARTS>List of VOUCHERS</TOPPARTS><XMLTAG>\"List of VOUCHERS\"</XMLTAG></FORM><PART NAME=\"List of VOUCHERS\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <TOPLINES>List of VOUCHERS</TOPLINES><REPEAT>List of VOUCHERS : Collection of VOUCHERS</REPEAT><SCROLLED>Vertical</SCROLLED></PART><LINE NAME=\"List of VOUCHERS\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <LEFTFIELDS>Name</LEFTFIELDS></LINE><FIELD NAME=\"Name\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <SET>$Name</SET><XMLTAG>\"VOUCHERNAME\"</XMLTAG></FIELD><COLLECTION NAME=\"Collection of VOUCHERS\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <TYPE>VOUCHER TYPES</TYPE></COLLECTION> </TDLMESSAGE> </TDL></DESC></BODY></ENVELOPE>";
						string UXml = "<ENVELOPE> <HEADER><VERSION>1</VERSION><TALLYREQUEST>Export</TALLYREQUEST><TYPE>Data</TYPE><ID>List of VOUCHERS</ID> </HEADER><BODY><DESC><STATICVARIABLES><SVEXPORTFORMAT>$$SysName:XML</SVEXPORTFORMAT><SVCURRENTCOMPANY>" + COMPANY + "</SVCURRENTCOMPANY><SVFROMDATE TYPE=\"Date\">" + COMPANY_STARTDATE + "</SVFROMDATE></STATICVARIABLES><TDL><TDLMESSAGE><REPORT NAME=\"List of VOUCHERS\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <FORMS>List of VOUCHERS</FORMS></REPORT><FORM NAME=\"List of VOUCHERS\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <TOPPARTS>List of VOUCHERS</TOPPARTS><XMLTAG>\"List of VOUCHERS\"</XMLTAG></FORM><PART NAME=\"List of VOUCHERS\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <TOPLINES>List of VOUCHERS</TOPLINES><REPEAT>List of VOUCHERS : Collection of VOUCHERS</REPEAT><SCROLLED>Vertical</SCROLLED></PART><LINE NAME=\"List of VOUCHERS\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <LEFTFIELDS>Name</LEFTFIELDS></LINE><FIELD NAME=\"Name\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <SET>$Name</SET><XMLTAG>\"UNITS\"</XMLTAG></FIELD><COLLECTION NAME=\"Collection of VOUCHERS\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <TYPE>UNITS</TYPE></COLLECTION> </TDLMESSAGE> </TDL></DESC></BODY></ENVELOPE>";
						string CXml = "<ENVELOPE><HEADER> <VERSION>1</VERSION> <TALLYREQUEST>Export</TALLYREQUEST> <TYPE>Data</TYPE> <ID>List of Currencies</ID></HEADER><BODY><DESC><STATICVARIABLES><SVEXPORTFORMAT>$$SysName:XML</SVEXPORTFORMAT><SVCURRENTCOMPANY>" + COMPANY + "</SVCURRENTCOMPANY><SVFROMDATE TYPE=\"Date\">" + COMPANY_STARTDATE + "</SVFROMDATE></STATICVARIABLES><TDL><TDLMESSAGE><REPORT NAME=\"List of Currencies\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <FORMS>List of Currencies</FORMS></REPORT><FORM NAME=\"List of Currencies\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <TOPPARTS>List of Currencies</TOPPARTS><XMLTAG>\"List of Currencies\"</XMLTAG></FORM><PART NAME=\"List of Currencies\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <TOPLINES>List of Currencies</TOPLINES><REPEAT>List of Currencies : Collection of Currencies</REPEAT><SCROLLED>Vertical</SCROLLED></PART><LINE NAME=\"List of Currencies\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <LEFTFIELDS>Name</LEFTFIELDS> </LINE><FIELD NAME=\"Name\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <SET>$EXPANDEDSYMBOL</SET><XMLTAG>\"CURRENCIES\"</XMLTAG></FIELD><COLLECTION NAME=\"Collection of Currencies\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"> <TYPE>CURRENCIES</TYPE></COLLECTION> </TDLMESSAGE> </TDL></DESC></BODY></ENVELOPE>";
						string AXml = "<ENVELOPE><HEADER><VERSION>1</VERSION><TALLYREQUEST>Export</TALLYREQUEST><TYPE>Data</TYPE><ID>List of AttendanceType</ID></HEADER><BODY><DESC><STATICVARIABLES><SVEXPORTFORMAT>$$SysName:XML</SVEXPORTFORMAT><SVCURRENTCOMPANY>" + COMPANY + "</SVCURRENTCOMPANY><SVFROMDATE TYPE=\"Date\">" + COMPANY_STARTDATE + "</SVFROMDATE></STATICVARIABLES><TDL><TDLMESSAGE><REPORT NAME=\"List of AttendanceType\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"><FORMS>List of AttendanceType</FORMS></REPORT><FORM NAME=\"List of AttendanceType\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"><TOPPARTS>List of AttendanceType</TOPPARTS><XMLTAG>\"List of AttendanceTypes\"</XMLTAG></FORM><PART NAME=\"List of AttendanceType\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"><TOPLINES>List of AttendanceType</TOPLINES><REPEAT>List of AttendanceType : Collection of AttendanceType</REPEAT><SCROLLED>Vertical</SCROLLED></PART><LINE NAME=\"List of AttendanceType\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"><LEFTFIELDS>Name</LEFTFIELDS></LINE><FIELD NAME=\"Name\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"><SET>$Name</SET><XMLTAG>\"ATTENDANCETYPE\"</XMLTAG></FIELD><COLLECTION NAME=\"Collection of AttendanceType\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"><TYPE>AttendanceType</TYPE></COLLECTION></TDLMESSAGE></TDL></DESC></BODY></ENVELOPE>";
						string EGXml = "<ENVELOPE><HEADER><VERSION>1</VERSION><TALLYREQUEST>Export</TALLYREQUEST><TYPE>Data</TYPE><ID>List of Cost Center</ID></HEADER><BODY><DESC><STATICVARIABLES><SVEXPORTFORMAT>$$SysName:XML</SVEXPORTFORMAT><SVCURRENTCOMPANY>" + COMPANY + "</SVCURRENTCOMPANY><SVFROMDATE TYPE=\"Date\">" + COMPANY_STARTDATE + "</SVFROMDATE></STATICVARIABLES><TDL><TDLMESSAGE><REPORT NAME=\"List of Cost Center\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"><FORMS>List of Cost Center</FORMS></REPORT><FORM NAME=\"List of Cost Center\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"><TOPPARTS>List of Cost Center</TOPPARTS><XMLTAG>\"List of Cost Centers\"</XMLTAG></FORM><PART NAME=\"List of Cost Center\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"><TOPLINES>List of Cost Center</TOPLINES><REPEAT>List of Cost Center : Collection of Cost Center</REPEAT><SCROLLED>Vertical</SCROLLED></PART><LINE NAME=\"List of Cost Center\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"><LEFTFIELDS>Name</LEFTFIELDS></LINE><FIELD NAME=\"Name\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"><SET>$Name</SET><XMLTAG>\"EMPLOYEEGROUP\"</XMLTAG></FIELD><COLLECTION NAME=\"Collection of Cost Center\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"><TYPE>COSTCENTER</TYPE><TYPE>COSTCENTER</TYPE><FILTERS>IsEmployeeGroup</FILTERS></COLLECTION><SYSTEM TYPE=\"Formulae\" NAME=\"IsEmployeeGroup\">$ISEMPLOYEEGROUP</SYSTEM></TDLMESSAGE></TDL></DESC></BODY></ENVELOPE>";
						string EmeeXml = "<ENVELOPE><HEADER><VERSION>1</VERSION><TALLYREQUEST>Export</TALLYREQUEST><TYPE>Data</TYPE><ID>List of Cost Center</ID></HEADER><BODY><DESC><STATICVARIABLES><SVEXPORTFORMAT>$$SysName:XML</SVEXPORTFORMAT><SVCURRENTCOMPANY>" + COMPANY + "</SVCURRENTCOMPANY><SVFROMDATE TYPE=\"Date\">" + COMPANY_STARTDATE + "</SVFROMDATE></STATICVARIABLES><TDL><TDLMESSAGE><REPORT NAME=\"List of Cost Center\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"><FORMS>List of Cost Center</FORMS></REPORT><FORM NAME=\"List of Cost Center\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"><TOPPARTS>List of Cost Center</TOPPARTS><XMLTAG>\"List of Cost Centers\"</XMLTAG></FORM><PART NAME=\"List of Cost Center\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"><TOPLINES>List of Cost Center</TOPLINES><REPEAT>List of Cost Center : Collection of Cost Center</REPEAT><SCROLLED>Vertical</SCROLLED></PART><LINE NAME=\"List of Cost Center\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"><LEFTFIELDS>Name</LEFTFIELDS></LINE><FIELD NAME=\"Name\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"><SET>$Name</SET><XMLTAG>\"EMPLOYEE\"</XMLTAG></FIELD><COLLECTION NAME=\"Collection of Cost Center\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"><TYPE>COSTCENTER</TYPE><TYPE>COSTCENTER</TYPE><FILTERS>IsEmployeeGroup</FILTERS><FILTERS>payroll</FILTERS></COLLECTION><SYSTEM TYPE=\"Formulae\" NAME=\"IsEmployeeGroup\"> Not $ISEMPLOYEEGROUP</SYSTEM><SYSTEM TYPE=\"Formulae\" NAME=\"payroll\">$FORPAYROLL</SYSTEM></TDLMESSAGE></TDL></DESC></BODY></ENVELOPE>";
						Dictionary<int, List<string>> Tags = new Dictionary<int, List<string>>();
					List<string> XmlList = new List<string>()

						{
						LXml,
						GXml,
						CCatXml,
						CCenXml,
						SGXml,
						SCXml,
						SIXml,
						GoXml,
						VXml,
						UXml,
						CXml,
						AXml,
						EGXml,
						EmeeXml
					};
						List<string> Ledger_Tags = new List<string>
					{
						"NAME",
						"PARENT"
					};
						List<string> Group_Tags = new List<string>
					{
						"GROUPNAME"
					};
						Tags[0] = Ledger_Tags;
						Tags[1] = Group_Tags;
						List<string> CostCategory_Tags = (Tags[2] = new List<string>
					{
						"COSTCATEGORY"
					});
						List<string> CostCenters_Tags = (Tags[3] = new List<string>
					{
						"COSTCENTER"
					});
						List<string> StockGroup_Tags = (Tags[4] = new List<string>
					{
						"STOCKGROUPNAME"
					});
						List<string> StockCategory_Tags = (Tags[5] = new List<string>
					{
						"STOCKCATEGORY"
					});
						List<string> StockItem_Tags = (Tags[6] = new List<string>
					{
						"STOCKITEM"
					});
						List<string> Godown_Tags = (Tags[7] = new List<string>
					{
						"GODOWN"
					});
						List<string> VoucherType_Tags = (Tags[8] = new List<string>
					{
						"VOUCHERNAME"
					});
						List<string> Units_Tags = (Tags[9] = new List<string>
					{
						"UNITS"
					});
						List<string> Currency_Tags = (Tags[10] = new List<string>
					{
						"CURRENCIES"
					});
						List<string> Attendance_Tags = (Tags[11] = new List<string>
					{
						"ATTENDANCETYPE"
					});
						List<string> EmployeeGroups_Tags = (Tags[12] = new List<string>
					{
						"EMPLOYEEGROUP"
					});
						List<string> Employees_Tags = (Tags[13] = new List<string>
					{
						"EMPLOYEE"
					});
						Dictionary<string, List<string>> data = await tally.Fetchdata(XmlList, Tags);
						LEDGERS = data["NAME"];
						PARENTS = data["PARENT"];
						GROUPS = data["GROUPNAME"];
						COSTCATEGORY = data["COSTCATEGORY"];
						COSTCENTER = data["COSTCENTER"];
						STOCKGROUPS = data["STOCKGROUPNAME"];
						STOCKCATEGORY = data["STOCKCATEGORY"];
						STOCKITEMS = data["STOCKITEM"];
						GODOWN = data["GODOWN"];
						VOUCHERTYPES = data["VOUCHERNAME"];
						UNITS = data["UNITS"];
						CURRENCIES = data["CURRENCIES"];
						ATTENDANCETYPE = data["ATTENDANCETYPE"];
						EMPLOYEEGROUP = data["EMPLOYEEGROUP"];
						EMPLOYEE = data["EMPLOYEE"];
					}
					catch (Exception ex)
					{
						Exception e = ex;
						STATUS = "Error in Fetching data from Tally\n" + e.Message;
						//MessageBox.Show(STATUS);
					}
				}
				else
				{
					//MessageBox.Show(STATUS);
				}
			}

		public async Task<VouchersList> GetVoucherListByType(string VchTypeName,string FromDate = null,String ToDate = null)
        {
			string Xml = "<ENVELOPE><HEADER><VERSION>1</VERSION><TALLYREQUEST>Export</TALLYREQUEST><TYPE>Data</TYPE><ID>List of Vouchers</ID></HEADER><BODY><DESC>";
			Xml += "<STATICVARIABLES><SVEXPORTFORMAT>$$SysName:XML</SVEXPORTFORMAT>";
			Xml = Tally.COMPANY == null? Xml: Xml += "<SVCURRENTCOMPANY>" + Tally.COMPANY + "</SVCURRENTCOMPANY>";
			Xml = FromDate == null ? Xml : Xml += "<SVFROMDATE TYPE=\"Date\">"+FromDate+"</SVFROMDATE>";
			Xml = ToDate == null? Xml : Xml += "<SVTODATE TYPE=\"Date\">" + ToDate + "</SVTODATE>";
			Xml +="</STATICVARIABLES><TDL><TDLMESSAGE><REPORT NAME=\"List of Vouchers\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"><FORMS>List of Voucher</FORMS></REPORT><FORM NAME=\"List of Voucher\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"><TOPPARTS>List of Vouchers</TOPPARTS><XMLTAG>\"List of Vouchers\"</XMLTAG></FORM><PART NAME=\"List of Vouchers\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"><TOPLINES>List of Vouchers</TOPLINES><REPEAT>List of Vouchers : Collection of Vouchers</REPEAT><SCROLLED>Vertical</SCROLLED></PART><LINE NAME=\"List of Vouchers\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"><LEFTFIELDS>Number</LEFTFIELDS><LEFTFIELDS>MasterID</LEFTFIELDS></LINE><FIELD NAME=\"Number\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"><SET>$VoucherNumber</SET><XMLTAG>\"Number\"</XMLTAG></FIELD><FIELD NAME=\"MasterID\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"><SET>$MASTERID</SET><XMLTAG>\"MasterID\"</XMLTAG></FIELD><COLLECTION NAME=\"Collection of Vouchers\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\"><TYPE>Voucher</TYPE><FILTERS>IsVchType</FILTERS></COLLECTION><SYSTEM TYPE=\"Formulae\" NAME=\"IsVchType\">$VoucherTypeName=\"" + VchTypeName + "\"</SYSTEM></TDLMESSAGE></TDL></DESC></BODY></ENVELOPE>";
			string RespXML = await SendRequest(Xml);
			XmlSerializer serlizer = new XmlSerializer(typeof(VouchersList));
			StringReader XmlString = new StringReader(RespXML);
			VouchersList VList = (VouchersList)serlizer.Deserialize(XmlString);
			return VList;

		}

		private async Task<Dictionary<string, List<string>>> Fetchdata(List<string> Xml_L, Dictionary<int, List<string>> TagsDic)
			{
				XmlDocument xml = new XmlDocument();
				Dictionary<string, List<string>> Dic = new Dictionary<string, List<string>>();
				try
				{
					int length = TagsDic.Count;
					for (int i = 0; i < length; i++)
					{
						string SXml = Xml_L[i];
						xml.LoadXml(await SendRequest(SXml));
						List<string> Tags = TagsDic[i];
						foreach (string tag in Tags)
						{
							List<string> TempList = new List<string>();
							XmlNodeList Nodes = xml.GetElementsByTagName(tag);
							int Count = Nodes.Count;
							for (int j = 0; j < Count; j++)
							{
								TempList.Add(Nodes[j].InnerText);
							}
							Dic.Add(tag, TempList);
						}
					}
					return Dic;
				}
				catch (Exception)
				{
					throw;
				}
			}

		public async Task<string> SendRequest(string SXml)
			{
				string Resxml = "";
				await Check();
				if (STATUS == "Running")
				{
					try
					{
						StringContent TXML = new StringContent(SXml, Encoding.Default, "application/xml");
						HttpResponseMessage Res = await client.PostAsync(FULL_URL, TXML);
						Res.EnsureSuccessStatusCode();
						Resxml = await Res.Content.ReadAsStringAsync();
						//_ = NumberFormatInfo.CurrentInfo.CurrencySymbol;
						return Resxml;
					}
					catch (Exception e)
					{
						STATUS = e.Message;
						return Resxml;
					}
				}
				return Resxml;
			}

		public Task<string> GetLXml(string _Action, string Company, string Name, string Parent, string alias = null, string OpeningBalance = null, string Currency = null, string BillWise = null, string CreditPeriod = null, string Creditdayscheck = null, string CreditLimit = null, string EffectStock = null, string MailingName = null, List<string> Address = null, string Country = null, string State = null, string PinCode = null, string ContactPerson = null, string PhoneNo = null, string MobileNo = null, string FaxNo = null, string Mail = null, string CCMail = null, string Website = null, string ITNum = null, string GSTRegType = null, string ISOtherTerritory = null, string IsEcommerce = null, string DeemedExporter = null, string PartyType = null, string GSTIN = null, string IsTransporter = null, string TransporterID = null, string Description = null, string Notes = null)
			{
				string str = "<ENVELOPE>\n<HEADER>\n<VERSION>1</VERSION>\n<TALLYREQUEST>Import</TALLYREQUEST>\n<TYPE>Data</TYPE>\n<ID>All Masters</ID>\n</HEADER>\n<BODY>\n<DESC>\n<STATICVARIABLES>\n<SVCURRENTCOMPANY>" + Company + "</SVCURRENTCOMPANY>\n</STATICVARIABLES>\n</DESC>\n<DATA>\n<TALLYMESSAGE>\n";
				str += ((_Action != "Create") ? ("<LEDGER NAME=\"" + Name + "\" ACTION=\"" + _Action + "\">\n") : "<LEDGER>\n");
				str = str + "<NAME.LIST>\n<NAME>" + Name + "</NAME>\n<NAME>" + alias + "</NAME>\n</NAME.LIST>\n<PARENT>" + Parent + "</PARENT>\n";
				str = ((OpeningBalance == null) ? str : (str + "<OPENINGBALANCE>" + OpeningBalance + "</OPENINGBALANCE>\n"));
				str = ((Currency == null) ? str : (str + "<CURRENCYNAME>" + Currency + "</CURRENCYNAME>\n"));
				str = ((BillWise == null) ? str : (str + "<ISBILLWISEON>" + BillWise + "</ISBILLWISEON>\n"));
				str = ((CreditPeriod == null) ? str : (str + "<BILLCREDITPERIOD>" + CreditPeriod + "</BILLCREDITPERIOD>\n"));
				str = ((Creditdayscheck == null) ? str : (str + "<ISCREDITDAYSCHKON>" + Creditdayscheck + "</ISCREDITDAYSCHKON>\n"));
				str = ((CreditLimit == null) ? str : (str + "<CREDITLIMIT>" + CreditLimit + "</CREDITLIMIT>\n"));
				str = ((EffectStock == null) ? str : (str + "<AFFECTSSTOCK>" + EffectStock + "</AFFECTSSTOCK>\n"));
				str = ((MailingName == null) ? str : (str + "<ADDITIONALNAME>" + MailingName + "</ADDITIONALNAME>\n"));
				if (Address != null)
				{
					str += "<ADDRESS.LIST>\n";
					foreach (string item in Address)
					{
						str = str + "<ADDRESS>" + item + "</ADDRESS>\n";
					}
					str += "</ADDRESS.LIST>\n";
				}
				str = ((Country == null) ? str : (str + "<COUNTRYNAME>" + Country + "</COUNTRYNAME>\n"));
				str = ((State == null) ? str : (str + "<STATENAME>" + State + "</STATENAME>\n"));
				str = ((PinCode == null) ? str : (str + "<PINCODE>" + PinCode + "</PINCODE>\n"));
				str = ((ContactPerson == null) ? str : (str + "<LEDGERCONTACT>" + ContactPerson + "</LEDGERCONTACT>\n"));
				str = ((PhoneNo == null) ? str : (str + "<LEDGERPHONE>" + PhoneNo + "</LEDGERPHONE>\n"));
				str = ((MobileNo == null) ? str : (str + "<LEDGERMOBILE>" + MobileNo + "</LEDGERMOBILE>\n"));
				str = ((FaxNo == null) ? str : (str + "<LEDGERFAX>" + FaxNo + "</LEDGERFAX>\n"));
				str = ((Mail == null) ? str : (str + "<EMAIL>" + Mail + "</EMAIL>\n"));
				str = ((CCMail == null) ? str : (str + "<EMAILCC>" + CCMail + "</EMAILCC>\n"));
				str = ((Website == null) ? str : (str + "<WEBSITE>" + Website + "</WEBSITE>\n"));
				str = ((ITNum == null) ? str : (str + "<INCOMETAXNUMBER>" + ITNum + "</INCOMETAXNUMBER>\n"));
				str = ((GSTRegType == null) ? str : (str + "<GSTREGISTRATIONTYPE>" + GSTRegType + "</GSTREGISTRATIONTYPE>\n"));
				str = ((ISOtherTerritory == null) ? str : (str + "<ISOTHTERRITORYASSESSEE>" + ISOtherTerritory + "</ISOTHTERRITORYASSESSEE>\n"));
				str = ((GSTIN == null) ? str : (str + "<PARTYGSTIN>" + GSTIN + "</PARTYGSTIN>\n"));
				str = ((IsEcommerce == null) ? str : (str + "<ISECOMMOPERATOR>" + IsEcommerce + "</ISECOMMOPERATOR>\n"));
				str = ((DeemedExporter == null) ? str : (str + "<CONSIDERPURCHASEFOREXPORT>" + DeemedExporter + "</CONSIDERPURCHASEFOREXPORT>\n"));
				str = ((PartyType == null) ? str : (str + "<GSTNATUREOFSUPPLY>" + PartyType + "</GSTNATUREOFSUPPLY>\n"));
				str = ((IsTransporter == null) ? str : (str + "<ISTRANSPORTER>" + IsTransporter + "</ISTRANSPORTER>\n"));
				str = ((TransporterID == null) ? str : (str + "<TRANSPORTERID>" + TransporterID + "</TRANSPORTERID>\n"));
				str = ((Description == null) ? str : (str + "<DESCRIPTION>" + Description + "</DESCRIPTION>\n"));
				str = ((Notes == null) ? str : (str + "<NARRATION>" + Notes + "</NARRATION>\n"));
				str += "</LEDGER>\n</TALLYMESSAGE>\n</DATA>\n</BODY>\n</ENVELOPE>";
				return Task.FromResult(str);
			}

		public async Task<string> Send(string xml, string EType, string Name = null)
			{
				XmlDocument LXml = new XmlDocument();
				try
				{
					string res = await SendRequest(xml);
					LXml.LoadXml(res);
					XmlNodeList Nodes = LXml.GetElementsByTagName("LINEERROR");
					XmlNodeList SNodes = LXml.GetElementsByTagName("IMPORTRESULT");
					XmlNodeList RNodes = LXml.GetElementsByTagName("CMPINFO");
					if (SNodes.Count != 0)
					{
						XmlNodeList RList = RNodes.Item(0).ChildNodes;
						XmlNodeList IList = SNodes.Item(0).ChildNodes;
						foreach (XmlElement RItem in RList)
						{
							if (EInfo.ContainsKey(RItem.Name))
							{
								EInfo[RItem.Name] = double.Parse(RItem.InnerText);
							}
							else
							{
								EInfo.Add(RItem.Name, double.Parse(RItem.InnerText));
							}
						}
						foreach (XmlElement item in IList)
						{
							if (item.InnerXml == "1")
							{
								if (EType != "Voucher")
								{
									STATUS = (EType + " (" + Name + ") " + item.Name + " Succesfully").ToUpper();
								}
								else
								{
									XmlNodeList RVNodes = LXml.GetElementsByTagName("LASTCREATEDVCHID");
									string ID = RVNodes.Item(0).InnerText;
									STATUS = (EType + "  " + item.Name + " Succesfully with ID - " + ID).ToUpper();
								}
							}
						}
					}
					else
					{
						STATUS = Nodes[0].InnerText.ToUpper();
					}
				}
				catch (Exception ex)
				{
					Exception e = ex;
					STATUS = e.Message;
				}
				return STATUS;
			}

		public string ReplaceXML(string strXmlText)
			{
				string result = null;
				if (strXmlText != null)
				{
					result = strXmlText.Replace("&", "&amp;");
					result = result.Replace("'", "&apos;");
					result = result.Replace("\"\"", "&quot;");
					result = result.Replace(">", "&gt;");
					result = result.Replace("<", "&lt;");
				}
				return result;
			}
		public string ReplaceXMLText(string strXmlText)
		{
			string result = null;
			if (strXmlText != null)
			{
				//result = strXmlText.Replace("&amp;", "&");
				//result = result.Replace( "&apos;", "'");
				//result = result.Replace("&quot;","\"\"");
				//result = result.Replace("&gt;", ">");
				result = strXmlText.Replace("&#x4;", "");
				result = result.Replace("&#4;", "");
			}
			return result;
		}

		public Task<string> GetGXml(string _Action, string Company, string GName, string GParent, string Alias, string IsSubLedger, string IsCalculable, string AllocType)
			{
				string str = "<ENVELOPE>\n<HEADER>\n<VERSION>1</VERSION>\n<TALLYREQUEST>Import</TALLYREQUEST>\n<TYPE>Data</TYPE>\n<ID>All Masters</ID>\n</HEADER>\n<BODY>\n<DESC>\n<STATICVARIABLES>\n<SVCURRENTCOMPANY>" + Company + "</SVCURRENTCOMPANY>\n</STATICVARIABLES>\n</DESC>\n<DATA>\n<TALLYMESSAGE>\n";
				str += ((_Action != "Create") ? ("<GROUP NAME=\"" + GName + "\" ACTION=\"" + _Action + "\">\n") : "<GROUP>\n");
				str = str + "<NAME.LIST>\n<NAME>" + GName + "</NAME>\n<NAME>" + Alias + "</NAME>\n</NAME.LIST>\n<PARENT>" + GParent + "</PARENT>\n";
				str = ((IsSubLedger == null) ? str : (str + "<ISSUBLEDGER>" + IsSubLedger + "</ISSUBLEDGER>\n"));
				str = ((IsCalculable == null) ? str : (str + "<BASICGROUPISCALCULABLE>" + IsCalculable + "</BASICGROUPISCALCULABLE>\n"));
				str = ((AllocType == null) ? str : (str + "<ADDLALLOCTYPE>" + AllocType + "</ADDLALLOCTYPE>\n"));
				str += "</GROUP>\n</TALLYMESSAGE>\n</DATA>\n</BODY>\n</ENVELOPE>";
				return Task.FromResult(str);
			}

		public Task<string> GetVXml(string _Action, string Company, string VCHTYPE, string Date, string VoucherID = null, string EffectiveDate = null, string Narration = null, Dictionary<int, Dictionary<string, List<Dictionary<string, string>>>> Ledger = null)
			{
				string str = "<ENVELOPE>\n<HEADER>\n<VERSION>1</VERSION>\n<TALLYREQUEST>Import</TALLYREQUEST>\n<TYPE>Data</TYPE>\n<ID>Vouchers</ID>\n</HEADER>\n<BODY>\n<DESC>\n<STATICVARIABLES>\n<SVCURRENTCOMPANY>" + Company + "</SVCURRENTCOMPANY>\n</STATICVARIABLES>\n</DESC>\n<DATA>\n<TALLYMESSAGE>\n<VOUCHER Action=\"" + _Action + "\" VCHTYPE =\"" + VCHTYPE + "\">\n<DATE>" + Date + "</DATE>\n<NARRATION>" + Narration + "</NARRATION>\n<VOUCHERTYPENAME>" + VCHTYPE + "</VOUCHERTYPENAME>\n<EFFECTIVEDATE>" + EffectiveDate + "</EFFECTIVEDATE>\n";
				str += ((VoucherID != null) ? ("<VOUCHERNUMBER>" + VoucherID + "</VOUCHERNUMBER>\n") : null);
				foreach (Dictionary<string, List<Dictionary<string, string>>> value in Ledger.Values)
				{
					str += "<ALLLEDGERENTRIES.LIST>\n";
					if (value.ContainsKey("INFO"))
					{
						List<Dictionary<string, string>> list = value["INFO"];
						Dictionary<string, string> dictionary = list[0];
						str = str + "<LEDGERNAME>" + dictionary["Name"] + "</LEDGERNAME>\n<ISDEEMEDPOSITIVE>" + dictionary["IsDeemedPositive"] + "</ISDEEMEDPOSITIVE>\n<AMOUNT>" + dictionary["Amount"] + "</AMOUNT>\n";
					}
					if (value.ContainsKey("BILLALLOC"))
					{
						List<Dictionary<string, string>> list2 = value["BILLALLOC"];
						foreach (Dictionary<string, string> item in list2)
						{
							str += "<BILLALLOCATIONS.LIST>\n";
							str += (item.ContainsKey("Name") ? ("<NAME>" + item["Name"] + "</NAME>\n") : null);
							str += (item.ContainsKey("BillType") ? ("<BILLTYPE>" + item["BillType"] + "</BILLTYPE>\n") : null);
							str += (item.ContainsKey("Amount") ? ("<AMOUNT>" + item["Amount"] + "</AMOUNT>\n") : null);
							str += "</BILLALLOCATIONS.LIST>\n";
						}
					}
					str += "</ALLLEDGERENTRIES.LIST>\n";
				}
				str += "</VOUCHER>\n</TALLYMESSAGE>\n</DATA>\n</BODY>\n</ENVELOPE>";
				return Task.FromResult(str);
			}

		public Task<string> GetVchXml(string _Action, string VCHTYPE, string Date, List<VLedgers> LedgersList, string VoucherID, string Narration = null, string EffectiveDate = null, string PartyName = null, string PartyMailingName = null, List<string> Address = null, string State = null, string Country = null, string GSTIN = null, string POS = null)
			{
				Voucher voucher = new Voucher();
				Header header2 = (voucher.Header = new Header
				{
					Version = 1,
					Request = "Import",
					Type = "Data",
					ID = "Vouchers"
				});
				StaticVariables staticVariables = new StaticVariables
				{
					SVCompany = COMPANY,
					SVFromDate = COMPANY_STARTDATE
				};
				Description desc = new Description
				{
					StaticVariables = staticVariables
				};
				VData voucher2 = new VData
				{
					Action = _Action,
					Date = Date,
					EffectiveDate = EffectiveDate,
					VoucherNumber = VoucherID,
					Narration = Narration,
					VCHTYPE = VCHTYPE,
					VoucherType = VCHTYPE,
					PartyName = PartyName,
					PartyMailingName = PartyMailingName,
					Address = Address,
					State = State,
					Country = Country,
					GSTIN = GSTIN,
					PlaceOfSupply = POS,
					LedgersList = LedgersList
				};
				Message message = new Message
				{
					Voucher = voucher2
				};
				Data data = new Data
				{
					Message = message
				};
				Body body2 = (voucher.Body = new Body
				{
					Data = data,
					Desc = desc
				});
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(Voucher));
				TextWriter textWriter = new StringWriter();
				xmlSerializer.Serialize(textWriter, voucher);
				return Task.FromResult(textWriter.ToString());
			}
	}


	[XmlRoot(ElementName = "ALLLEDGERENTRIES.LIST")]
	public class VLedgers
	{
		[XmlElement(ElementName = "LEDGERNAME")]
		public string LedgerName
			{
				get;
				set;
			}

		[XmlElement(ElementName = "ISDEEMEDPOSITIVE")]
		public string IsdeemedPositive
			{
				get;
				set;
			}

		[XmlElement(ElementName = "AMOUNT")]
		public string Amount
			{
				get;
				set;
			}

		[XmlElement(ElementName = "BILLALLOCATIONS.LIST")]
		public List<BillAllocations> BillAllocationsList
			{
				get;
				set;
			}
	}
	[XmlRoot(ElementName = "BILLALLOCATIONS.LIST")]
	public class BillAllocations
	{
		[XmlElement(ElementName = "NAME")]
		public string Name
			{
				get;
				set;
			}

		[XmlElement(ElementName = "BILLTYPE")]
		public string BillType
			{
				get;
				set;
			}

		[XmlElement(ElementName = "AMOUNT")]
		public string Amount
			{
				get;
				set;
			}
	}
	[XmlRoot(ElementName = "ENVELOPE")]
	public class Voucher
	{
		[XmlElement(ElementName = "HEADER")]
		public Header Header
			{
				get;
				set;
			}

		[XmlElement(ElementName = "BODY")]
		public Body Body
			{
				get;
				set;
			}
	}
	[XmlRoot(ElementName = "VOUCHER")]
	public class VData
	{
		[XmlElement(ElementName = "DATE")]
		public string Date
			{
				get;
				set;
			}

		[XmlElement(ElementName = "VOUCHERNUMBER")]
		public string VoucherNumber
			{
				get;
				set;
			}

		[XmlElement(ElementName = "NARRATION")]
		public string Narration
			{
				get;
				set;
			}

		[XmlElement(ElementName = "VOUCHERTYPENAME")]
		public string VoucherType
			{
				get;
				set;
			}

		[XmlElement(ElementName = "EFFECTIVEDATE")]
		public string EffectiveDate
			{
				get;
				set;
			}

		[XmlElement(ElementName = "PARTYNAME")]
		public string PartyName
			{
				get;
				set;
			}

		[XmlElement(ElementName = "PARTYMAILINGNAME")]
		public string PartyMailingName
			{
				get;
				set;
			}

		[XmlElement(ElementName = "ADDRESS")]
		public List<string> Address
			{
				get;
				set;
			}

		[XmlElement(ElementName = "STATENAME")]
		public string State
			{
				get;
				set;
			}

		[XmlElement(ElementName = "PARTYGSTIN")]
		public string GSTIN
			{
				get;
				set;
			}

		[XmlElement(ElementName = "PLACEOFSUPPLY")]
		public string PlaceOfSupply
			{
				get;
				set;
			}

		[XmlElement(ElementName = "COUNTRYOFRESIDENCE")]
		public string Country
			{
				get;
				set;
			}

		[XmlElement(ElementName = "ALLLEDGERENTRIES.LIST")]
		public List<VLedgers> LedgersList
			{
				get;
				set;
			}

		[XmlAttribute(AttributeName = "DATE")]
		public string Dt
			{
				get
				{
					return Date;
				}
				set
				{
					Date = value;
				}
			}

		[XmlAttribute(AttributeName = "TAGNAME")]
		public string TAGNAME
			{
				get
				{
					return "Voucher Number";
				}
				set
				{
					value = "Voucher Number";
				}
			}

		[XmlAttribute(AttributeName = "TAGVALUE")]
		public string TAGVALUE
			{
				get
				{
					return VoucherNumber;
				}
				set
				{
					VoucherNumber = value;
				}
			}

		[XmlAttribute(AttributeName = "Action")]
		public string Action
			{
				get;
				set;
			}

		[XmlAttribute(AttributeName = "VCHTYPE")]
		public string VCHTYPE
			{
				get
				{
					return VoucherType;
				}
				set
				{
					VoucherType = value;
				}
			}
	}
	[XmlRoot(ElementName = "STATICVARIABLES")]
	public class StaticVariables
	{
		[XmlElement(ElementName = "SVCURRENTCOMPANY")]
		public string SVCompany
			{
				get;
				set;
			}

		[XmlElement(ElementName = "SVFROMDATE")]
		public string SVFromDate
			{
				get;
				set;
			}

		[XmlElement(ElementName = "SVTODATE")]
		public string SVToDate
			{
				get;
				set;
			}
	}
	[XmlRoot(ElementName = "HEADER")]
	public class Header
	{
		[XmlElement(ElementName = "VERSION")]
		public int Version
			{
				get;
				set;
			}

		[XmlElement(ElementName = "TALLYREQUEST")]
		public string Request
			{
				get;
				set;
			}

		[XmlElement(ElementName = "TYPE")]
		public string Type
			{
				get;
				set;
			}

		[XmlElement(ElementName = "ID")]
		public string ID
			{
				get;
				set;
			}
	}
	[XmlRoot(ElementName = "DESC")]
	public class Description
	{
		[XmlElement(ElementName = "STATICVARIABLES")]
		public StaticVariables StaticVariables
			{
				get;
				set;
			}
	}
	[XmlRoot(ElementName = "DATA")]
	public class Data
	{
		[XmlElement(ElementName = "TALLYMESSAGE")]
		public Message Message
			{
				get;
				set;
			}
	}
	[XmlRoot(ElementName = "BODY")]
	public class Body
	{
		[XmlElement(ElementName = "DESC")]
		public Description Desc
			{
				get;
				set;
			}

		[XmlElement(ElementName = "DATA")]
		public Data Data
			{
				get;
				set;
			}
	}
	[XmlRoot(ElementName = "TALLYMESSAGE")]
	public class Message
	{
		[XmlElement(ElementName = "VOUCHER")]
		public VData Voucher
			{
				get;
				set;
			}
	}



	}

