using System.Xml.Serialization;
using TallyConnector.TDLReportSourceGenerator;

namespace IntegrationTests.Basic;

public class BasicTestsWithSimpleProperties
{
    //[Test]
    //public void VerifyBasicClassWithNoInheritance()
    //{
    //    DTOType type = typeof(ModelWithSimpleProperties);

    //    Assert.That(type.IsAssignableTo(typeof(ITallyRequestableObject)), Is.True);

    //    var modelName = nameof(ModelWithSimpleProperties);
    //    var modelNameUppper = modelName.ToUpper();

    //    var assemblyName = type.Assembly.GetName().Name;
    //    var FullName = type.FullName;

    //    var fieldPrefix = $"{assemblyName}\0{FullName}";

    //    string fieldName1 = $"Name_{Utils.GenerateUniqueNameSuffix($"{fieldPrefix}\0Name")}";
    //    string fieldName2 = $"Parent_{Utils.GenerateUniqueNameSuffix($"{fieldPrefix}\0Parent")}";

    //    string reportNameSuffix = Utils.GenerateUniqueNameSuffix(fieldPrefix);
    //    string reportName = $"{modelName}_{reportNameSuffix}";
    //    string colName = $"{modelName}sCollection_{reportNameSuffix}";
    //    string[] fetchList = ["NAME", "PARENT"];

    //    Assert.That(ModelWithSimpleProperties.GetFetchList(), Is.EqualTo(fetchList).AsCollection);

    //    var collections = ModelWithSimpleProperties.GetTDLCollections();
    //    Assert.That(collections, Has.Length.EqualTo(1));

    //    var collection1 = collections[0];

    //    Assert.Multiple(() =>
    //    {
    //        Assert.That(collection1.Name, Is.EqualTo(colName));
    //        Assert.That(collection1.DTOType, Is.EqualTo("Ledger"));
    //        Assert.That(collection1.NativeFields, Is.Not.Null);
    //        Assert.That(collection1.NativeFields, Is.EqualTo(fetchList).AsCollection);
    //    });
    //    var tdlFields = ModelWithSimpleProperties.GetTDLFields();
    //    Assert.That(tdlFields, Has.Length.EqualTo(2));

    //    var tdlfield1 = tdlFields[0];
    //    Assert.Multiple(() =>
    //    {
    //        Assert.That(tdlfield1.Name,
    //                        Is.EqualTo(fieldName1));
    //        Assert.That(tdlfield1.Set, Is.EqualTo("$NAME"));

    //        Assert.That(tdlfield1.XMLTag, Is.EqualTo("NAME"));
    //    });

    //    var tdlfield2 = tdlFields[1];
    //    Assert.Multiple(() =>
    //    {
    //        Assert.That(tdlfield2.Name,
    //                        Is.EqualTo(fieldName2));
    //        Assert.That(tdlfield2.Set, Is.EqualTo("$PARENT"));

    //        Assert.That(tdlfield2.XMLTag, Is.EqualTo("PARENT"));
    //    });
    //    Assert.That(ModelWithSimpleProperties.GetTDLParts(), Is.EqualTo(Array.Empty<Part>()));

    //    var mainPart = ModelWithSimpleProperties.GetMainTDLPart();
    //    Assert.Multiple(() =>
    //    {
    //        Assert.That(mainPart.Name, Is.EqualTo(reportName));
    //        Assert.That(mainPart.Lines, Is.EqualTo(new string[] { reportName }).AsCollection);
    //        Assert.That(mainPart.XMLTag, Is.Null);
    //        Assert.That(mainPart.Repeat, Is.EqualTo($"{reportName} : {colName}"));
    //    });

    //    Assert.That(ModelWithSimpleProperties.GetTDLLines(), Is.EqualTo(Array.Empty<Line>()));

    //    var mainLine = ModelWithSimpleProperties.GetMainTDLLine();
    //    Assert.Multiple(() =>
    //    {
    //        Assert.That(mainLine.Name, Is.EqualTo(reportName));
    //        Assert.That(mainLine.Fields, Is.EqualTo(new string[] { fieldName1, fieldName2 }).AsCollection);
    //        Assert.That(mainLine.XMLTag, Is.EqualTo(modelNameUppper));
    //    });

    //    var xmlAttributeOverrides = ModelWithSimpleProperties.GetXMLAttributeOverides();
    //    var rootAttr = xmlAttributeOverrides[ReportResponseEnvelope<ModelWithSimpleProperties>.DTOTypeInfo, "Objects"];

    //    Assert.That(rootAttr, Is.Not.Null);
    //    Assert.That(rootAttr.XmlElements, Has.Count.EqualTo(1));

    //    var xmlElem = rootAttr.XmlElements[0];
    //    Assert.That(xmlElem!.ElementName, Is.EqualTo(modelNameUppper));

    //    var envelope = ModelWithSimpleProperties.GetRequestEnvelope();
    //    var tdlMsg = envelope.Body.Desc.TDL.TDLMessage;

    //    Assert.That(tdlMsg.Report, Has.Count.EqualTo(1));
    //    var report1 = tdlMsg.Report[0];
    //    Assert.Multiple(() =>
    //    {
    //        Assert.That(report1.Name, Is.EqualTo(reportName));
    //        Assert.That(report1.FormName, Is.EqualTo(reportName));
    //    });
    //    Assert.That(tdlMsg.Form, Has.Count.EqualTo(1));
    //    var form1 = tdlMsg.Form[0];
    //    Assert.Multiple(() =>
    //    {
    //        Assert.That(form1.Name, Is.EqualTo(reportName));
    //        Assert.That(form1.PartName, Is.EqualTo(reportName));
    //    });
    //    Assert.Multiple(() =>
    //    {
    //        Assert.That(tdlMsg.Part, Has.Count.EqualTo(1));
    //        Assert.That(tdlMsg.Form, Has.Count.EqualTo(1));
    //        Assert.That(tdlMsg.Field, Has.Count.EqualTo(2));
    //        Assert.That(tdlMsg.Collection, Has.Count.EqualTo(1));
    //    });
    //}

    //[Test]
    //public void VerifyBasicClassWithInheritance()
    //{
    //    DTOType baseType = typeof(ModelWithSimpleProperties);
    //    DTOType type = typeof(ModelWithSimplePropertiesandInheritance);

    //    Assert.That(type.IsAssignableTo(typeof(ITallyRequestableObject)), Is.True);

    //    var modelName = nameof(ModelWithSimplePropertiesandInheritance);
    //    var modelNameUppper = modelName.ToUpper();

    //    var baseAssemblyName = baseType.Assembly.GetName().Name;
    //    var assemblyName = type.Assembly.GetName().Name;
    //    var baseFullName = baseType.FullName;
    //    var FullName = type.FullName;

    //    var baseFieldPrefix = $"{baseAssemblyName}\0{baseFullName}";
    //    var fieldPrefix = $"{assemblyName}\0{FullName}";



    //    string fieldName1 = $"Name_{Utils.GenerateUniqueNameSuffix($"{baseFieldPrefix}\0Name")}";
    //    string fieldName2 = $"Parent_{Utils.GenerateUniqueNameSuffix($"{baseFieldPrefix}\0Parent")}";
    //    string fieldName3 = $"IsBillWise_{Utils.GenerateUniqueNameSuffix($"{fieldPrefix}\0IsBillWise")}";

    //    string reportNameSuffix = Utils.GenerateUniqueNameSuffix(fieldPrefix);
    //    string reportName = $"{modelName}_{reportNameSuffix}";
    //    string colName = $"{modelName}sCollection_{reportNameSuffix}";
    //    string[] fetchList = ["NAME", "PARENT", "ISBILLWISEON"];

    //    Assert.That(ModelWithSimplePropertiesandInheritance.GetFetchList(), Is.EqualTo(fetchList).AsCollection);

    //    var collections = ModelWithSimplePropertiesandInheritance.GetTDLCollections();
    //    Assert.That(collections, Has.Length.EqualTo(1));

    //    var collection1 = collections[0];

    //    Assert.Multiple(() =>
    //    {
    //        Assert.That(collection1.Name, Is.EqualTo(colName));
    //        Assert.That(collection1.DTOType, Is.EqualTo("Ledger"));
    //        Assert.That(collection1.NativeFields, Is.Not.Null);
    //        Assert.That(collection1.NativeFields, Is.EqualTo(fetchList).AsCollection);
    //    });
    //    var tdlFields = ModelWithSimplePropertiesandInheritance.GetTDLFields();
    //    Assert.That(tdlFields, Has.Length.EqualTo(3));

    //    var tdlfield1 = tdlFields[0];
    //    Assert.Multiple(() =>
    //    {
    //        Assert.That(tdlfield1.Name,
    //                        Is.EqualTo(fieldName1));
    //        Assert.That(tdlfield1.Set, Is.EqualTo("$NAME"));

    //        Assert.That(tdlfield1.XMLTag, Is.EqualTo("NAME"));
    //    });

    //    var tdlfield2 = tdlFields[1];
    //    Assert.Multiple(() =>
    //    {
    //        Assert.That(tdlfield2.Name,
    //                        Is.EqualTo(fieldName2));
    //        Assert.That(tdlfield2.Set, Is.EqualTo("$PARENT"));

    //        Assert.That(tdlfield2.XMLTag, Is.EqualTo("PARENT"));
    //    });

    //    var tdlfield3 = tdlFields[2];
    //    Assert.Multiple(() =>
    //    {
    //        Assert.That(tdlfield3.Name,
    //                        Is.EqualTo(fieldName3));
    //        Assert.That(tdlfield3.Set, Is.EqualTo($"$${Constants.GetBooleanFromLogicFieldFunctionName}:$ISBILLWISEON"));

    //        Assert.That(tdlfield3.XMLTag, Is.EqualTo("ISBILLWISEON"));
    //    });

    //    Assert.That(ModelWithSimplePropertiesandInheritance.GetTDLParts(), Is.EqualTo(Array.Empty<Part>()));

    //    var mainPart = ModelWithSimplePropertiesandInheritance.GetMainTDLPart();
    //    Assert.Multiple(() =>
    //    {
    //        Assert.That(mainPart.Name, Is.EqualTo(reportName));
    //        Assert.That(mainPart.Lines, Is.EqualTo(new string[] { reportName }).AsCollection);
    //        Assert.That(mainPart.XMLTag, Is.Null);
    //        Assert.That(mainPart.Repeat, Is.EqualTo($"{reportName} : {colName}"));
    //    });

    //    Assert.That(ModelWithSimplePropertiesandInheritance.GetTDLLines(), Is.EqualTo(Array.Empty<Line>()));

    //    var mainLine = ModelWithSimplePropertiesandInheritance.GetMainTDLLine();
    //    Assert.Multiple(() =>
    //    {
    //        Assert.That(mainLine.Name, Is.EqualTo(reportName));
    //        Assert.That(mainLine.Fields, Is.EqualTo(new string[] { fieldName1, fieldName2, fieldName3 }).AsCollection);
    //        Assert.That(mainLine.XMLTag, Is.EqualTo(modelNameUppper));
    //    });

    //    var xmlAttributeOverrides = ModelWithSimplePropertiesandInheritance.GetXMLAttributeOverides();
    //    var rootAttr = xmlAttributeOverrides[ReportResponseEnvelope<ModelWithSimplePropertiesandInheritance>.DTOTypeInfo, "Objects"];

    //    Assert.That(rootAttr, Is.Not.Null);
    //    Assert.That(rootAttr.XmlElements, Has.Count.EqualTo(1));

    //    var xmlElem = rootAttr.XmlElements[0];
    //    Assert.That(xmlElem!.ElementName, Is.EqualTo(modelNameUppper));

    //    var envelope = ModelWithSimplePropertiesandInheritance.GetRequestEnvelope();
    //    var tdlMsg = envelope.Body.Desc.TDL.TDLMessage;

    //    Assert.That(tdlMsg.Report, Has.Count.EqualTo(1));
    //    var report1 = tdlMsg.Report[0];
    //    Assert.Multiple(() =>
    //    {
    //        Assert.That(report1.Name, Is.EqualTo(reportName));
    //        Assert.That(report1.FormName, Is.EqualTo(reportName));
    //    });
    //    Assert.That(tdlMsg.Form, Has.Count.EqualTo(1));
    //    var form1 = tdlMsg.Form[0];
    //    Assert.Multiple(() =>
    //    {
    //        Assert.That(form1.Name, Is.EqualTo(reportName));
    //        Assert.That(form1.PartName, Is.EqualTo(reportName));
    //    });
    //    Assert.Multiple(() =>
    //    {
    //        Assert.That(tdlMsg.Part, Has.Count.EqualTo(1));
    //        Assert.That(tdlMsg.Form, Has.Count.EqualTo(1));
    //        Assert.That(tdlMsg.Field, Has.Count.EqualTo(3));
    //        Assert.That(tdlMsg.Collection, Has.Count.EqualTo(1));
    //    });
    //}
    //[Test]
    //public void VerifyBasicClassWithInheritanceandOverridenProp()
    //{
    //    DTOType baseType = typeof(ModelWithSimpleProperties);
    //    DTOType type = typeof(ModelWithSimplePropertiesInheritanceandOveridden);

    //    Assert.That(type.IsAssignableTo(typeof(ITallyRequestableObject)), Is.True);

    //    var modelName = nameof(ModelWithSimplePropertiesInheritanceandOveridden);
    //    var modelNameUppper = modelName.ToUpper();

    //    var baseAssemblyName = baseType.Assembly.GetName().Name;
    //    var assemblyName = type.Assembly.GetName().Name;
    //    var baseFullName = baseType.FullName;
    //    var FullName = type.FullName;

    //    var baseFieldPrefix = $"{baseAssemblyName}\0{baseFullName}";
    //    var fieldPrefix = $"{assemblyName}\0{FullName}";



    //    string fieldName2 = $"Name_{Utils.GenerateUniqueNameSuffix($"{fieldPrefix}\0Name")}";
    //    string fieldName1 = $"Parent_{Utils.GenerateUniqueNameSuffix($"{baseFieldPrefix}\0Parent")}";
    //    string fieldName3 = $"IsBillWise_{Utils.GenerateUniqueNameSuffix($"{fieldPrefix}\0IsBillWise")}";

    //    string reportNameSuffix = Utils.GenerateUniqueNameSuffix(fieldPrefix);
    //    string reportName = $"{modelName}_{reportNameSuffix}";
    //    string colName = $"{modelName}sCollection_{reportNameSuffix}";
    //    string[] fetchList = ["PARENT", "OVERIDDENNAME", "ISBILLWISEON"];

    //    Assert.That(ModelWithSimplePropertiesInheritanceandOveridden.GetFetchList(), Is.EqualTo(fetchList).AsCollection);

    //    var collections = ModelWithSimplePropertiesInheritanceandOveridden.GetTDLCollections();
    //    Assert.That(collections, Has.Length.EqualTo(1));

    //    var collection1 = collections[0];

    //    Assert.Multiple(() =>
    //    {
    //        Assert.That(collection1.Name, Is.EqualTo(colName));
    //        Assert.That(collection1.DTOType, Is.EqualTo("Ledger"));
    //        Assert.That(collection1.NativeFields, Is.Not.Null);
    //        Assert.That(collection1.NativeFields, Is.EqualTo(fetchList).AsCollection);
    //    });
    //    var tdlFields = ModelWithSimplePropertiesInheritanceandOveridden.GetTDLFields();
    //    Assert.That(tdlFields, Has.Length.EqualTo(3));

    //    var tdlfield1 = tdlFields[0];
    //    Assert.Multiple(() =>
    //    {
    //        Assert.That(tdlfield1.Name,
    //                        Is.EqualTo(fieldName1));
    //        Assert.That(tdlfield1.Set, Is.EqualTo("$PARENT"));

    //        Assert.That(tdlfield1.XMLTag, Is.EqualTo("PARENT"));
    //    });

    //    var tdlfield2 = tdlFields[1];
    //    Assert.Multiple(() =>
    //    {
    //        Assert.That(tdlfield2.Name,
    //                        Is.EqualTo(fieldName2));
    //        Assert.That(tdlfield2.Set, Is.EqualTo("$OVERIDDENNAME"));

    //        Assert.That(tdlfield2.XMLTag, Is.EqualTo("OVERIDDENNAME"));
    //    });

    //    var tdlfield3 = tdlFields[2];
    //    Assert.Multiple(() =>
    //    {
    //        Assert.That(tdlfield3.Name,
    //                        Is.EqualTo(fieldName3));
    //        Assert.That(tdlfield3.Set, Is.EqualTo($"$${Constants.GetBooleanFromLogicFieldFunctionName}:$ISBILLWISEON"));

    //        Assert.That(tdlfield3.XMLTag, Is.EqualTo("ISBILLWISEON"));
    //    });

    //    Assert.That(ModelWithSimplePropertiesInheritanceandOveridden.GetTDLParts(), Is.EqualTo(Array.Empty<Part>()));

    //    var mainPart = ModelWithSimplePropertiesInheritanceandOveridden.GetMainTDLPart();
    //    Assert.Multiple(() =>
    //    {
    //        Assert.That(mainPart.Name, Is.EqualTo(reportName));
    //        Assert.That(mainPart.Lines, Is.EqualTo(new string[] { reportName }).AsCollection);
    //        Assert.That(mainPart.XMLTag, Is.Null);
    //        Assert.That(mainPart.Repeat, Is.EqualTo($"{reportName} : {colName}"));
    //    });

    //    Assert.That(ModelWithSimplePropertiesInheritanceandOveridden.GetTDLLines(), Is.EqualTo(Array.Empty<Line>()));

    //    var mainLine = ModelWithSimplePropertiesInheritanceandOveridden.GetMainTDLLine();
    //    Assert.Multiple(() =>
    //    {
    //        Assert.That(mainLine.Name, Is.EqualTo(reportName));
    //        Assert.That(mainLine.Fields, Is.EqualTo(new string[] { fieldName1, fieldName2, fieldName3 }).AsCollection);
    //        Assert.That(mainLine.XMLTag, Is.EqualTo(modelNameUppper));
    //    });

    //    var xmlAttributeOverrides = ModelWithSimplePropertiesInheritanceandOveridden.GetXMLAttributeOverides();
    //    var rootAttr = xmlAttributeOverrides[ReportResponseEnvelope<ModelWithSimplePropertiesInheritanceandOveridden>.DTOTypeInfo, "Objects"];
        
    //    Assert.That(rootAttr, Is.Not.Null);
    //    Assert.That(rootAttr.XmlElements, Has.Count.EqualTo(1));

    //    var xmlElem = rootAttr.XmlElements[0];
    //    Assert.That(xmlElem!.ElementName, Is.EqualTo(modelNameUppper));

    //    var overridenAttr = xmlAttributeOverrides[typeof(ModelWithSimpleProperties), "Name"];
    //    Assert.Multiple(() =>
    //    {
    //        Assert.That(overridenAttr, Is.Not.Null);
    //        Assert.That(overridenAttr.XmlIgnore, Is.True);
    //    });

    //    var envelope = ModelWithSimplePropertiesInheritanceandOveridden.GetRequestEnvelope();
    //    var tdlMsg = envelope.Body.Desc.TDL.TDLMessage;

    //    Assert.That(tdlMsg.Report, Has.Count.EqualTo(1));
    //    var report1 = tdlMsg.Report[0];
    //    Assert.Multiple(() =>
    //    {
    //        Assert.That(report1.Name, Is.EqualTo(reportName));
    //        Assert.That(report1.FormName, Is.EqualTo(reportName));
    //    });
    //    Assert.That(tdlMsg.Form, Has.Count.EqualTo(1));
    //    var form1 = tdlMsg.Form[0];
    //    Assert.Multiple(() =>
    //    {
    //        Assert.That(form1.Name, Is.EqualTo(reportName));
    //        Assert.That(form1.PartName, Is.EqualTo(reportName));
    //    });
    //    Assert.Multiple(() =>
    //    {
    //        Assert.That(tdlMsg.Part, Has.Count.EqualTo(1));
    //        Assert.That(tdlMsg.Form, Has.Count.EqualTo(1));
    //        Assert.That(tdlMsg.Field, Has.Count.EqualTo(3));
    //        Assert.That(tdlMsg.Collection, Has.Count.EqualTo(1));
    //    });
    //}

}
[ImplementTallyRequestableObject]
[TDLCollection(Type = "Ledger")]
partial class ModelWithSimpleProperties
{
    [XmlElement(ElementName = "NAME")]
    public string Name { get; set; }

    [XmlElement(ElementName = "PARENT")]
    public string Parent { get; set; }
}

[TDLCollection(Type = "Ledger")]

[ImplementTallyRequestableObject]
partial class ModelWithSimplePropertiesandInheritance : ModelWithSimpleProperties
{
    [XmlElement(ElementName = "ISBILLWISEON")]
    public bool IsBillWise { get; set; }
}

[ImplementTallyRequestableObject]
[TDLCollection(Type = "Ledger")]
partial class ModelWithSimplePropertiesInheritanceandOveridden : ModelWithSimpleProperties
{

    [XmlElement(ElementName = "OVERIDDENNAME")]
    public new string Name { get; set; }

    [XmlElement(ElementName = "ISBILLWISEON")]
    public bool IsBillWise { get; set; }
}