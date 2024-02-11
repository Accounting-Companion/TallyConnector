using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TallyConnector.Core.Attributes;
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class IgnoreForCreateDTOAttribute : Attribute
{
}
