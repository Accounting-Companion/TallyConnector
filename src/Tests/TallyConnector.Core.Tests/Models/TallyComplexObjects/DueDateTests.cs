using NUnit.Framework;
using TallyConnector.Core.Models.TallyComplexObjects;
using System;

namespace TallyConnector.Core.Tests.Models.TallyComplexObjects;

[TestFixture]
public class DueDateTests
{
    [Test]
    public void ToString_DateMatchesText_ReturnsDateOnly()
    {
        var date = new DateTime(2022, 04, 10);
        var dueDate = new DueDate
        {
            DueOnDate = date,
            InText = "10-Apr-2022"
        };
        
        Assert.That(dueDate.ToString(), Is.EqualTo("10-Apr-2022"));
    }

    [Test]
    public void ToString_DateDiffersFromText_ReturnsComposite()
    {
        var date = new DateTime(2022, 04, 10);
        var dueDate = new DueDate
        {
            DueOnDate = date,
            InText = "10 Days"
        };
        
        // Code: $"{duedate} ({InText})"
        // Expected: "10-Apr-2022 (10 Days)"
        Assert.That(dueDate.ToString(), Is.EqualTo("10-Apr-2022 (10 Days)"));
    }

    [Test]
    public void ImplicitOperator_ToDTO_ReturnsCorrectDTO()
    {
        var date = new DateTime(2022, 04, 10);
        var dueDate = new DueDate
        {
            DueOnDate = date,
            InDays = 5,
            InText = "15-Apr-2022" // Usually implies the resulting date
        };

        DueDateDTO? dto = dueDate;

        Assert.That(dto, Is.Not.Null);
        Assert.That(dto!.Value, Is.EqualTo("15-Apr-2022"));
        
        // JD Logic: (dateTime.Subtract(new DateTime(1900, 1, 1)).TotalDays + 1)
        // DateTime = 10-Apr + 5 days = 15-Apr-2022.
        // 1900-01-01 to 2022-04-15
        var targetDate = new DateTime(2022, 04, 15);
        var expectedJD = (targetDate.Subtract(new DateTime(1900, 1, 1)).TotalDays + 1).ToString();
        
        Assert.That(dto.JD, Is.EqualTo(expectedJD));
    }
}
