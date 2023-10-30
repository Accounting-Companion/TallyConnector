using System.Globalization;
using System.Xml.Schema;

namespace TallyConnector.Core.Converters.XMLConverterHelpers;
[JsonConverter(typeof(TallyDueDateJsonConverter))]
public class TallyDueDate : IXmlSerializable
{
    public TallyDueDate()
    {
    }


    public TallyDueDate(DateTime dueDate, DateTime? billDate = null)
    {
        DueDate = dueDate;
        BillDate = billDate ?? DateTime.Now;
    }
    public TallyDueDate(int value, DueDateFormat suffix, DateTime? billDate = null)
    {
        Value = value;
        Suffix = suffix;
        BillDate = billDate ?? DateTime.Now;
    }

    public DateTime BillDate { get; set; }
    public int Value { get; private set; }
    public DueDateFormat? Suffix { get; private set; }
    public DateTime? DueDate { get; private set; }

    public XmlSchema? GetSchema()
    {
        return null;
    }

    public void ReadXml(XmlReader reader)
    {
        var JD = reader.GetAttribute("JD");
        var tValue = reader.ReadElementContentAsString();

        if (!string.IsNullOrEmpty(tValue))
        {
            if (JD != null)
            {
                BillDate = new DateTime(1900, 1, 1).AddDays(int.Parse(JD) - 1);
            }

            if (tValue.Contains('-'))
            {
                Suffix = DueDateFormat.Date;
                bool v = DateTime.TryParseExact(tValue, "d-MMM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date);
                bool sdate = DateTime.TryParseExact(tValue, "d-MMM-yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime ShrtDate);
                if (v)
                {
                    DueDate = date;
                }
                else if (sdate)
                {
                    DueDate = ShrtDate;
                }
            }
            else
            {
               
                var splittedvalues = tValue.Split(' ');
                var suffix = splittedvalues.Last().Trim();
                Value = int.Parse(splittedvalues.First());
                if (suffix.Contains("Days"))
                {
                    Suffix = DueDateFormat.Day;
                }
                else if (suffix.Contains("Weeks"))
                {
                    Suffix = DueDateFormat.Week;
                }
                else if (suffix.Contains("Months"))
                {
                    Suffix = DueDateFormat.Month;
                }
                else if (suffix.Contains("Years"))
                {
                    Suffix = DueDateFormat.Year;
                }

                DueDate = Suffix == DueDateFormat.Month ?
                    BillDate.AddMonths(Value) : Suffix == DueDateFormat.Year ?
                    BillDate.AddYears(Value) : Suffix == DueDateFormat.Week ? BillDate.AddDays(Value * 7) : BillDate.AddDays(Value);


            }

        }

    }

    public void WriteXml(XmlWriter writer)
    {
        if (this != null)
        {
            writer.WriteAttributeString("TYPE", "Due Date");
            writer.WriteAttributeString("JD", (Math.Abs(((new DateTime(1900, 1, 1) - BillDate).Days))+1).ToString());
            if (Value != 0 && Suffix != null)
            {
                writer.WriteString($"{Value} {Suffix}s");
            }
            else if (DueDate != null)
            {
                writer.WriteString(DueDate?.ToString("dd-MMM-yyyy"));
            }

        }
    }

    public static implicit operator TallyDueDate(DateTime dueDate)
    {
        return new(dueDate);
    }
}

public enum DueDateFormat
{
    Day,
    Week,
    Month,
    Year,
    Date,
}
