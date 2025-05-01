

using TallyConnector.Core.Attributes.SourceGenerator;

namespace UnitTests.BasicTestsV2;

[TestClass]
public class BasicTestsWithSimpleProperties
{
  
    [TestMethod]
    public async Task VerifyBasicClassWithNoInheritance()
    {
        var src = @"

using TallyConnector.Core.Attributes.SourceGenerator;

namespace UnitTests.TestBasic;

[ImplementTallyRequestableObject]
public partial class Ledger
{
    public string Name { get; set; }
    public string Parent { get; set; }
}
";
        await VerifyTDLReportV2.VerifyGeneratorAsync(src, 
            ("UnitTests.TestBasic.Ledger.cs", @"using TallyConnector.Core.Extensions;

#nullable enable
namespace UnitTests.TestBasic;
/*
* Generated based on UnitTests.TestBasic.Ledger
*/
partial class Ledger : global::TallyConnector.Core.Models.Interfaces.ITallyRequestableObject
{
    internal const string Name_FieldName = ""Name_39DV"";
    internal const string Parent_FieldName = ""Parent_Dr40"";
    public const int SimpleFieldsCount = 2;
    public static async global::System.Threading.Tasks.Task<global::TallyConnector.Core.Models.Request.RequestEnvelope> GetRequestEnvelope()
    {
        return new();
    }

    public static global::TallyConnector.Core.Models.Request.Field[] GetTDLFields()
    {
        var _fields = new global::TallyConnector.Core.Models.Request.Field[SimpleFieldsCount];
        _fields[0] = new(Name_FieldName);
        _fields[1] = new(Parent_FieldName);
        return _fields;
    }
}"));
    }
    [TestMethod]
    public async Task VerifyBasicClassWithInheritance()
    {
        var src = @"

using TallyConnector.Core.Attributes.SourceGenerator;
using System.Xml;
using System.Xml.Serialization;
namespace UnitTests.TestBasic;

[ImplementTallyRequestableObject]
public partial class LedgerBase
{
  [XmlElement(ElementName = ""NAME"")]
    public string Name { get; set; }
  [XmlElement(ElementName = ""PARENT"")]
    public string Parent { get; set; }
}

[ImplementTallyRequestableObject]
public partial class Ledger : LedgerBase
{
    [XmlElement(ElementName = ""ISBILLWISEON"")]
    public bool IsBillWise { get; set; }
}
";
        await VerifyTDLReportV2.VerifyGeneratorAsync(src,  
            [
            ("UnitTests.TestBasic.LedgerBase.cs", @"using TallyConnector.Core.Extensions;

#nullable enable
namespace UnitTests.TestBasic;
/*
* Generated based on UnitTests.TestBasic.LedgerBase
*/
partial class LedgerBase : global::TallyConnector.Core.Models.Interfaces.ITallyRequestableObject
{
    internal const string Name_FieldName = ""Name_vO4U"";
    internal const string Parent_FieldName = ""Parent_v6lj"";
    public const int SimpleFieldsCount = 2;
    public static async global::System.Threading.Tasks.Task<global::TallyConnector.Core.Models.Request.RequestEnvelope> GetRequestEnvelope()
    {
        return new();
    }

    public static global::TallyConnector.Core.Models.Request.Field[] GetTDLFields()
    {
        var _fields = new global::TallyConnector.Core.Models.Request.Field[SimpleFieldsCount];
        _fields[0] = new(Name_FieldName);
        _fields[1] = new(Parent_FieldName);
        return _fields;
    }
}"), 
            ("UnitTests.TestBasic.Ledger.cs", @"using TallyConnector.Core.Extensions;

#nullable enable
namespace UnitTests.TestBasic;
/*
* Generated based on UnitTests.TestBasic.Ledger
*/
partial class Ledger : global::TallyConnector.Core.Models.Interfaces.ITallyRequestableObject
{
    internal const string IsBillWise_FieldName = ""IsBillWise_PtsT"";
    public const int SimpleFieldsCount = 3;
    public static async global::System.Threading.Tasks.Task<global::TallyConnector.Core.Models.Request.RequestEnvelope> GetRequestEnvelope()
    {
        return new();
    }

    public static global::TallyConnector.Core.Models.Request.Field[] GetTDLFields()
    {
        var _fields = new global::TallyConnector.Core.Models.Request.Field[SimpleFieldsCount];
        var ledgerBaseFields = global::UnitTests.TestBasic.LedgerBase.GetTDLFields();
        _fields.AddToArray(ledgerBaseFields, 0);
        _fields[2] = new(IsBillWise_FieldName);
        return _fields;
    }
}")]);
    }
}

