using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace TallyConnector.Models
{
	[XmlRoot(ElementName = "ENVELOPE")]
	public class Envelope
	{

		[XmlElement(ElementName = "HEADER")]
		public Header Header { get; set; }

		[XmlElement(ElementName = "BODY")]
		public Body Body { get; set; }
	}

	[XmlRoot(ElementName = "BODY")]
	public class Body
	{
		//[XmlElement(ElementName = "DESC")]
		//public Desc Desc { get; set; }

		[XmlElement(ElementName = "DATA")]
		public LedgerData Data { get; set; }
	}


	/// <summary>
	/// Ledger Message
	/// </summary>

	[XmlRoot(ElementName = "TALLYMESSAGE")]
	public class LedgerMessage
	{
		[XmlElement(ElementName = "LEDGER")]
		public Ledger Ledger { get; set; }
	}


	[XmlRoot(ElementName = "DATA")]
	public class LedgerData
	{

		[XmlElement(ElementName = "TALLYMESSAGE")]
		public LedgerMessage TallyMessage { get; set; }
	}

	/// <summary>
	/// Group Message
	/// </summary>
	[XmlRoot(ElementName = "TALLYMESSAGE")]
	public class GroupMessage
	{
		[XmlElement(ElementName = "LEDGER")]
		public Group Group { get; set; }
	}

	[XmlRoot(ElementName = "DATA")]
	public class GroupData
	{

		[XmlElement(ElementName = "TALLYMESSAGE")]
		public GroupMessage TallyMessage { get; set; }
	}

	/// <summary>
	/// Voucher Message
	/// </summary>
	[XmlRoot(ElementName = "TALLYMESSAGE")]
	public class VoucherMessage
	{
		[XmlElement(ElementName = "LEDGER")]
		public Voucher Voucher { get; set; }
	}

	[XmlRoot(ElementName = "DATA")]
	public class VoucherData
	{

		[XmlElement(ElementName = "TALLYMESSAGE")]
		public VoucherMessage TallyMessage { get; set; }
	}

}
