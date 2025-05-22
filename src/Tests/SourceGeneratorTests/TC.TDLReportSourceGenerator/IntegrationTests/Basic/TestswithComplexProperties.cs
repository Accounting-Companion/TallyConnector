using System.Xml.Serialization;
using TallyConnector.TDLReportSourceGenerator;

namespace IntegrationTests.Basic;
public class TestswithComplexProperties
{
    [Test]
    public void VerifyComplexClass()
    {
        Type type = typeof(ModelWithComplexProperties);
        Type complexPropType = typeof(ComplexPropertyModel);

        Assert.That(type.IsAssignableTo(typeof(ITallyRequestableObject)), Is.True);

        var modelName = nameof(ModelWithComplexProperties);

        var complexPropTypeName = nameof(ComplexPropertyModel);
        var modelNameUppper = modelName.ToUpper();
        var complexPropTypeNameUppper = complexPropTypeName.ToUpper();

        var assemblyName = type.Assembly.GetName().Name;
        var FullName = type.FullName;
        var complexPropTypeFullName = complexPropType.FullName;

        var fieldPrefix = $"{assemblyName}\0{FullName}";

        var complexfieldPrefix = $"{assemblyName}\0{complexPropTypeFullName}";

        string reportNameSuffix = Utils.GenerateUniqueNameSuffix(fieldPrefix);

        string reportName = $"{modelName}_{reportNameSuffix}";
        string colName = $"{modelName}sCollection_{reportNameSuffix}";
        string[] fetchList = ["NAME", "PARENT", "LEDGSTREGDETAILS.APPLICABLEFROM,LEDGSTREGDETAILS.STATE,LEDGSTREGDETAILS.PLACEOFSUPPLY"];

        Assert.That(ModelWithComplexProperties.GetFetchList(), Is.EqualTo(fetchList).AsCollection);

        var collections = ModelWithComplexProperties.GetTDLCollections();
        Assert.That(collections, Has.Length.EqualTo(1));

        var collection1 = collections[0];

        Assert.Multiple(() =>
        {
            Assert.That(collection1.Name, Is.EqualTo(colName));
            Assert.That(collection1.Type, Is.EqualTo("Ledger"));
            Assert.That(collection1.NativeFields, Is.Not.Null);
            Assert.That(collection1.NativeFields, Is.EqualTo(fetchList).AsCollection);
        });
        var tdlFields = ModelWithComplexProperties.GetTDLFields();
        Assert.That(tdlFields, Has.Length.EqualTo(5));

        string fieldName1 = $"Name_{Utils.GenerateUniqueNameSuffix($"{fieldPrefix}\0Name")}";
        string fieldName2 = $"Parent_{Utils.GenerateUniqueNameSuffix($"{fieldPrefix}\0Parent")}";

        string fieldName3 = $"ApplicableFrom_{Utils.GenerateUniqueNameSuffix($"{complexfieldPrefix}\0ApplicableFrom")}";
        string fieldName4 = $"State_{Utils.GenerateUniqueNameSuffix($"{complexfieldPrefix}\0State")}";
        string fieldName5 = $"PlaceOfSupply_{Utils.GenerateUniqueNameSuffix($"{complexfieldPrefix}\0PlaceOfSupply")}";


        string part1Name = $"GSTRegistrationDetails_{Utils.GenerateUniqueNameSuffix($"{fieldPrefix}\0GSTRegistrationDetails")}";
        string part1Coll = "LEDGSTREGDETAILS";

        var tdlfield1 = tdlFields[0];
        Assert.Multiple(() =>
        {
            Assert.That(tdlfield1.Name,
                            Is.EqualTo(fieldName1));
            Assert.That(tdlfield1.Set, Is.EqualTo("$NAME"));

            Assert.That(tdlfield1.XMLTag, Is.EqualTo("NAME"));
        });

        var tdlfield2 = tdlFields[1];
        Assert.Multiple(() =>
        {
            Assert.That(tdlfield2.Name,
                            Is.EqualTo(fieldName2));
            Assert.That(tdlfield2.Set, Is.EqualTo("$PARENT"));
            Assert.That(tdlfield2.XMLTag, Is.EqualTo("PARENT"));
        });

        var tdlfield3 = tdlFields[2];
        Assert.Multiple(() =>
        {
            Assert.That(tdlfield3.Name,
                            Is.EqualTo(fieldName3));
            Assert.That(tdlfield3.Set, Is.EqualTo($"$${Constants.GetTransformDateToXSDFunctionName}:$APPLICABLEFROM"));
            Assert.That(tdlfield3.XMLTag, Is.EqualTo("APPLICABLEFROM"));
        });

        var tdlfield4 = tdlFields[3];
        Assert.Multiple(() =>
        {
            Assert.That(tdlfield4.Name,
                            Is.EqualTo(fieldName4));
            Assert.That(tdlfield4.Set, Is.EqualTo("$STATE"));
            Assert.That(tdlfield4.XMLTag, Is.EqualTo("STATE"));
        });

        var tdlfield5 = tdlFields[4];
        Assert.Multiple(() =>
        {
            Assert.That(tdlfield5.Name,
                            Is.EqualTo(fieldName5));
            Assert.That(tdlfield5.Set, Is.EqualTo("$PLACEOFSUPPLY"));
            Assert.That(tdlfield5.XMLTag, Is.EqualTo("PLACEOFSUPPLY"));
        });

        var mainReport = ModelWithComplexProperties.GetMainTDLPart();
        Assert.Multiple(() =>
        {
            Assert.That(mainReport.Name, Is.EqualTo(reportName));
            Assert.That(mainReport.XMLTag, Is.Null);
            Assert.That(mainReport.Repeat, Is.EqualTo($"{reportName} : {colName}"));
            Assert.That(mainReport.Lines, Is.EqualTo(new string[] {reportName}).AsCollection);
        });

        var parts = ModelWithComplexProperties.GetTDLParts();
        Assert.That(parts, Has.Length.EqualTo(1));

        var tdlpart1 = parts[0];

        Assert.Multiple(() =>
        {
            Assert.That(tdlpart1.Name, Is.EqualTo(part1Name));
            Assert.That(tdlpart1.Repeat, Is.EqualTo($"{part1Name} : {part1Coll}"));
            Assert.That(tdlpart1.Lines, Is.EqualTo(new string[] {part1Name}).AsCollection);
        });

        var mainLine = ModelWithComplexProperties.GetMainTDLLine();
        Assert.Multiple(() =>
        {
            Assert.That(mainLine.Name, Is.EqualTo(reportName));
            Assert.That(mainLine.XMLTag, Is.EqualTo("MODELWITHCOMPLEXPROPERTIES"));
            Assert.That(mainLine.Explode, Is.EqualTo(new string[] {$"{part1Name}:$$NUMITEMS: LEDGSTREGDETAILS > 0" }).AsCollection);
            Assert.That(mainLine.Fields, Is.EqualTo(new string[] {fieldName1,fieldName2}).AsCollection);
        });

        var lines = ModelWithComplexProperties.GetTDLLines();
        Assert.That(lines, Has.Length.EqualTo(1));
        var tdlline1 = lines[0];
        Assert.Multiple(() =>
        {
            Assert.That(tdlline1.Name, Is.EqualTo(part1Name));
            Assert.That(tdlline1.XMLTag, Is.EqualTo("LEDGSTREGDETAILS.LIST"));
            Assert.That(tdlline1.Fields, Is.EqualTo(new string[] { fieldName3, fieldName4,fieldName5 }).AsCollection);
        });

        var xmlAttributeOverrides = ModelWithComplexProperties.GetXMLAttributeOverides();
        var rootAttr = xmlAttributeOverrides[ReportResponseEnvelope<ModelWithComplexProperties>.TypeInfo, "Objects"];

        Assert.That(rootAttr, Is.Not.Null);
        Assert.That(rootAttr.XmlElements, Has.Count.EqualTo(1));

        var xmlElem = rootAttr.XmlElements[0];
        Assert.That(xmlElem!.ElementName, Is.EqualTo(modelNameUppper));

        var envelope = ModelWithComplexProperties.GetRequestEnvelope();
        var tdlMsg = envelope.Body.Desc.TDL.TDLMessage;

        Assert.That(tdlMsg.Report, Has.Count.EqualTo(1));
        var report1 = tdlMsg.Report[0];
        Assert.Multiple(() =>
        {
            Assert.That(report1.Name, Is.EqualTo(reportName));
            Assert.That(report1.FormName, Is.EqualTo(reportName));
        });
        Assert.That(tdlMsg.Form, Has.Count.EqualTo(1));
        var form1 = tdlMsg.Form[0];
        Assert.Multiple(() =>
        {
            Assert.That(form1.Name, Is.EqualTo(reportName));
            Assert.That(form1.PartName, Is.EqualTo(reportName));
        });
    }
}

[ImplementTallyRequestableObject]
[TDLCollection(Type = "Ledger")]
public partial class ModelWithComplexProperties
{
    [XmlElement(ElementName = "NAME")]
    public string Name { get; set; }

    [XmlElement(ElementName = "PARENT")]
    public string Parent { get; set; }

    [XmlElement(ElementName = "LEDGSTREGDETAILS.LIST")]
    [TDLCollection(CollectionName = "LEDGSTREGDETAILS", ExplodeCondition = "$$NUMITEMS: LEDGSTREGDETAILS > 0")]
    public List<ComplexPropertyModel> GSTRegistrationDetails { get; set; }
}

public class ComplexPropertyModel
{
    [XmlElement("APPLICABLEFROM")]
    public DateTime ApplicableFrom { get; set; }

    [XmlElement("STATE")]
    public string State { get; set; }

    [XmlElement("PLACEOFSUPPLY")]
    public string PlaceOfSupply { get; set; }
}