using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;

namespace TallyConnector.SourceGenerators.Extensions.Symbols;
public static class AttributeDataExtensions
{
    public static string GetAttrubuteMetaName(this AttributeData attributeData)
    {
        string attributeMetaName = attributeData.AttributeClass!.OriginalDefinition.ToString();
        return attributeMetaName;
    }
}
