using System.Xml.Serialization;

namespace IntegrationTests.AdvanceTests;
public class ListTests
{
    [Test]
    public void VerifyModelwithSimpleList()
    {

        Type type = typeof(ModelwithSimpleList);

        Assert.That(type.IsAssignableTo(typeof(ITallyRequestableObject)), Is.True);

        var modelName = nameof(ModelwithSimpleList);
        var modelNameUppper = modelName.ToUpper();

        var assemblyName = type.Assembly.GetName().Name;
        var FullName = type.FullName;

        var fieldPrefix = $"{assemblyName}\0{FullName}";

        string fieldName1 = $"Name_{Utils.GenerateUniqueNameSuffix($"{fieldPrefix}\0Name")}";
        string fieldName2 = $"Parent_{Utils.GenerateUniqueNameSuffix($"{fieldPrefix}\0Parent")}";
        string fieldName3 = $"Addreses_{Utils.GenerateUniqueNameSuffix($"{fieldPrefix}\0Addreses")}";
        
        string reportNameSuffix = Utils.GenerateUniqueNameSuffix(fieldPrefix);
        string reportName = $"{modelName}_{reportNameSuffix}";
        string colName = $"{modelName}sCollection_{reportNameSuffix}";
        string[] fetchList = ["NAME", "PARENT", "ADDRESS"];

        Assert.That(ModelwithSimpleList.GetFetchList(), Is.EqualTo(fetchList).AsCollection);

        var collections = ModelwithSimpleList.GetTDLCollections();
        Assert.That(collections, Has.Length.EqualTo(1));

        var collection1 = collections[0];

        Assert.Multiple(() =>
        {
            Assert.That(collection1.Name, Is.EqualTo(colName));
            Assert.That(collection1.Type, Is.EqualTo("Ledger"));
            Assert.That(collection1.NativeFields, Is.Not.Null);
            Assert.That(collection1.NativeFields, Is.EqualTo(fetchList).AsCollection);
        });

        var tdlFields = ModelwithSimpleList.GetTDLFields();
        Assert.That(tdlFields, Has.Length.EqualTo(3));

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
            Assert.That(tdlfield3.Set, Is.EqualTo("$ADDRESS"));

            Assert.That(tdlfield3.XMLTag, Is.EqualTo("ADDRESS"));
        });

        var parts = ModelwithSimpleList.GetTDLParts();
        Assert.That(parts, Has.Length.EqualTo(1));

        var part1 = parts[0];
        Assert.Multiple(() =>
        {
            Assert.That(part1.Name, Is.EqualTo(fieldName3));
            Assert.That(part1.XMLTag, Is.EqualTo("ADDRESS.LIST"));
            Assert.That(part1.Repeat, Is.EqualTo($"{fieldName3} : Address"));
            Assert.That(part1.Lines, Is.EqualTo(new string[] {fieldName3}));
        });

        var mainPart = ModelwithSimpleList.GetMainTDLPart();
        Assert.Multiple(() =>
        {
            Assert.That(mainPart.Name, Is.EqualTo(reportName));
            Assert.That(mainPart.Lines, Is.EqualTo(new string[] { reportName }).AsCollection);
            Assert.That(mainPart.XMLTag, Is.Null);
            Assert.That(mainPart.Repeat, Is.EqualTo($"{reportName} : {colName}"));
        });

        var lines = ModelwithSimpleList.GetTDLLines();
        Assert.That(lines, Has.Length.EqualTo(1));

        var line1 = lines[0];
        Assert.Multiple(() =>
        {
            Assert.That(line1.Name, Is.EqualTo(fieldName3));
            Assert.That(line1.Fields, Is.EqualTo(new string[] { fieldName3 }));
        });
    }
}


[ImplementTallyRequestableObject]
[TDLCollection(Type = "Ledger")]
public partial class ModelwithSimpleList
{
    [XmlElement(ElementName = "NAME")]
    public string Name { get; set; }

    [XmlElement(ElementName = "PARENT")]
    public string Parent { get; set; }

    [XmlArray(ElementName = "ADDRESS.LIST")]
    [XmlArrayItem(ElementName = "ADDRESS")]
    [TDLCollection(CollectionName ="Address")]
    public List<string> Addreses { get; set; }
}