using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace TallyConnector.Models
{
	//[XmlRoot(ElementName = "ENVELOPE")]
	//public class Envelope
	//{

	//	[XmlElement(ElementName = "HEADER")]
	//	public Header Header { get; set; }

	//	[XmlElement(ElementName = "BODY")]
	//	public Body Body { get; set; }
	//}

	//[XmlRoot(ElementName = "BODY")]
	//public class Body
	//{
	//	[XmlElement(ElementName = "DESC")]
	//	public Description Desc { get; set; }

	//	[XmlElement(ElementName = "DATA")]
	//	public Data Data { get; set; }
	//}


	[XmlRoot(ElementName = "HEADER")]
	public class Header
	{
		public Header(string Request, string Type, string ID)
		{
			this._request = Request;
			this._type = Type;
			this._Id = ID;
		}
		public Header() { }
		private int _version = 1;
		private string _request;
		private string _type;
		private string _Id;
		[XmlElement(ElementName = "VERSION")]
		public int Version { get { return _version; } set { _version = value; } }

		[XmlElement(ElementName = "TALLYREQUEST")]
		public string Request { get { return _request; } set { _request = value; } }

		[XmlElement(ElementName = "TYPE")]
		public string Type { get { return _type; } set { _type = value; } }

		[XmlElement(ElementName = "ID")]
		public string ID { get { return _Id; } set { _Id = value; } }
	}


	[XmlRoot(ElementName = "DESC")]
	public class Description
	{
		[XmlElement(ElementName = "STATICVARIABLES")]
		public StaticVariables StaticVariables { get; set; } = new();

	}

	[XmlRoot(ElementName = "STATICVARIABLES")]
	public class StaticVariables
	{
		private string _ExportFormat;

        public StaticVariables()
        {
			
        }

        [XmlElement(ElementName = "SVEXPORTFORMAT")]
		public string SVExportFormat { get { return _ExportFormat; } set { _ExportFormat = $"$$SysName:{value}"; } }

		[XmlElement(ElementName = "SVCURRENTCOMPANY")]
		public string SVCompany { get; set; }

		[XmlIgnore]
		public string SVFromDate { get { return SVFrom.Text; } set {SVFrom = new(); SVFrom.Text = value; } }

		[XmlElement(ElementName = "SVFROMDATE")]
		public SVFrom SVFrom { get; set; }

		[XmlIgnore]
		public string SVToDate { get { return SVTo.Text; } set { SVTo = new(); SVTo.Text = value; } }

		[XmlElement(ElementName = "SVTODATE")]
		public SVTo SVTo { get; set; }

		[XmlElement(ElementName = "SVViewName")]
		public string ViewName { get; set; }

		[XmlElement(ElementName = "EXPLODEFLAG")]
		public string ExplodeFlag { get; set; }

	}
	[XmlRoot(ElementName = "SVFROMDATE")]
	public class SVFrom
	{
		[XmlAttribute(AttributeName = "TYPE")]
		public string Type { get; set; } = "Date";

		[XmlText]
		public string Text { get; set; }
	}
	[XmlRoot(ElementName = "SVTODATE")]
	public class SVTo
	{
		[XmlAttribute(AttributeName = "TYPE")]
		public string Type { get; set; } = "Date";

		[XmlText]
		public string Text { get; set; }
	}







}
