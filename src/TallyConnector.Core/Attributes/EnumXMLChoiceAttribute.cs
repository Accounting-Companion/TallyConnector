using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TallyConnector.Core.Attributes;
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class EnumXMLChoiceAttribute : Attribute
{
    public EnumXMLChoiceAttribute()
    {
    }

    public EnumXMLChoiceAttribute(string choice)
    {
        Choice = choice;
    }

    public string Choice { get; set; }
    public string Version { get; set; }
}
