﻿using System.Globalization;

namespace TallyConnector.Core.Models.TallyComplexObjects;
[TDLCollection(ExplodeCondition = "NOT $$IsEmpty:{0}")]
[MaptoDTO<DueDateDTO>]
[GenerateMeta]
public partial class DueDate : ITallyComplexObject, IBaseObject
{
    [TDLField(Set = "$$string:{0}:UniversalDate")]
    [XmlElement(ElementName = "DUEONDATE")]
    public DateTime DueOnDate { get; set; }

    [TDLField(Set = "$$ExtractNumbers:$$DueDateInDays:{0}", Invisible = "$$ISEMPTY:$$Value")]
    [XmlElement(ElementName = "INDAYS")]
    public int InDays { get; set; }

    [TDLField(Set = "{0}")]
    [XmlElement(ElementName = "INTEXT")]
    public string InText { get; set; } = null!;

    public override string ToString()
    {
        var duedate = DueOnDate.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);

        if (duedate == InText)
        {
            return duedate;
        }
        else
        {
            return $"{duedate} ({InText})";
        }

    }
}
public class DueDateDTO
{
    [XmlAttribute("JD")]
    public string JD { get; set; } = null!;
    [XmlAttribute("P")]
    public string P { get; set; } = null!;
    [XmlText]
    public string Value { get; set; } = null!;

    public static implicit operator DueDateDTO?(DueDate? dueDate)
    {
        if (dueDate == null)
        {
            return null;
        }
        DueDateDTO dueDateDTO = new();
        DateTime dateTime = dueDate.DueOnDate.AddDays(dueDate.InDays);
        dueDateDTO.JD = (dateTime.Subtract(new DateTime(1900, 1, 1)).TotalDays + 1).ToString();
        dueDateDTO.Value = dueDateDTO.P = dueDate.InText ?? dueDate.DueOnDate.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
        return dueDateDTO;
    }
}