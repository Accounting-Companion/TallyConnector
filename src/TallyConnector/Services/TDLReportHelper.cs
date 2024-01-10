using System.Reflection;

namespace TallyConnector.Services;
public static class TDLReportHelper
{
    private static Dictionary<Type, List<TDLReport>> SubFieldsList { get; set; } = new();
    public static TDLReport CreateTDLReport(Type type, TDLReport? rootReport = null)
    {
        rootReport ??= new(type.Name);
        TDLCollectionAttribute? tDLCollectionAttribute = AttributeHelper.GetTDLCollectionAttributeValue(type);
        if (tDLCollectionAttribute != null)
        {
            rootReport.CollectionName = tDLCollectionAttribute.CollectionName;
            rootReport.CollectionType = tDLCollectionAttribute.Type ?? tDLCollectionAttribute.CollectionName;

            rootReport.CreateCollectionTag = tDLCollectionAttribute.Include;
        }
        GenerateSubFields(type, rootReport);
        return rootReport;
    }

    private static void GenerateSubFields(Type type, TDLReport reportField)
    {
        var HaveSubfields = SubFieldsList.TryGetValue(type, out List<TDLReport>? SubFields);
        if (HaveSubfields && SubFields != null)
        {
            reportField.SubFields = SubFields;
        }
        else
        {
            PropertyInfo[] propertyInfoList = PropertyHelper.GetPropertyInfo(type);
            //For Caching
            List<TDLReport> reportFields = new();
            foreach (PropertyInfo propertyInfo in propertyInfoList)
            {
                Type propertyType = propertyInfo.PropertyType;

                //Check whether property is type generic list
                //else check whether object is of complex type
                if (propertyType.IsGenericType && (propertyType.GetGenericTypeDefinition() == typeof(List<>)))
                {
                    Type ChildType = propertyType.GetGenericArguments()[0];
                    if (IsComplexType(ChildType))
                    {
                        GenerateFieldsForComplexProperty(reportField, ChildType);
                    }
                    else
                    {
                        TDLCollectionAttribute? childCollectionAttribute = AttributeHelper.GetTDLCollectionAttributeValue(propertyInfo);
                        string elemName = AttributeHelper.GetXmlElement(propertyInfo)!;
                        TDLReport ChildreportField = new(elemName)
                        {
                            CollectionName = childCollectionAttribute?.CollectionName
                        };
                        reportField.SubFields.Add(ChildreportField);
                        GenerateReportField(propertyInfo, ChildreportField);
                    }


                }
                else if (IsComplexType(propertyType) && propertyType != typeof(XmlElement[]) && propertyType != typeof(XmlAttribute[]))
                {
                    GenerateFieldsForComplexProperty(reportField, propertyType);
                }
                else
                {
                    TDLReport? childReportField = GenerateReportField(propertyInfo, reportField);
                    if (childReportField != null)
                    {
                        reportFields.Add(childReportField);
                    }
                }
            }

            SubFieldsList[type] = reportFields;

        }

    }
    public static TDLReport? GenerateReportField(PropertyInfo propertyInfo,
                                            TDLReport reportField)
    {
        //Gets Xml Tag from Property Info if Exists
        string? xmlTag = AttributeHelper.GetXmlElement(propertyInfo);
        if (xmlTag != null)
        {
            TDLReport ChildField = new(xmlTag);
            TDLFieldAttribute? tDLXMLSet = AttributeHelper.GetTDLXMLSetAttributeValue(propertyInfo);
            if (tDLXMLSet != null)
            {
                if (tDLXMLSet.Set != string.Empty)
                {
                    ChildField.SetExp = tDLXMLSet.Set;
                }
                ChildField.IncludeinFetch = tDLXMLSet.IncludeInFetch;

            }

            reportField.SubFields.Add(ChildField);
            return ChildField;

        }
        return null;
    }
    private static void GenerateFieldsForComplexProperty(TDLReport reportField, Type ChildType)
    {
        TDLReport ChildreportField = new(ChildType.Name);
        TDLCollectionAttribute? childCollectionAttribute = AttributeHelper.GetTDLCollectionAttributeValue(ChildType);
        if (childCollectionAttribute == null)
        {
            ChildreportField.CollectionName = ChildType.Name;
        }
        else
        {
            ChildreportField.CollectionName = childCollectionAttribute.CollectionName;
        }
        reportField.SubFields.Add(ChildreportField);
        GenerateSubFields(ChildType, ChildreportField);
    }
    public static bool IsComplexType(Type propertyType)
    {
        List<Type> NonPrmitiveSimpleTypes = new() { typeof(string), typeof(int), typeof(int?), typeof(TallyYesNo), typeof(TallyDate) };
        return !propertyType.IsEnum && !propertyType.IsPrimitive && !NonPrmitiveSimpleTypes.Contains(propertyType);
    }
}
