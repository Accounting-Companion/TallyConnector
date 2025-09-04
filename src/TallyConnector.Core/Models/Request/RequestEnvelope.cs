using TallyConnector.Abstractions.Models;

namespace TallyConnector.Core.Models.Request;


[XmlRoot(ElementName = "ENVELOPE")]
public class RequestEnvelope : TallyXml 
{
    public RequestEnvelope()
    {
    }

    public RequestEnvelope(HType Type, string iD)
    {
        Header = new(Request: RequestType.Export, Type: Type, ID: iD); //Configuring Header To get Export data
    }
    public RequestEnvelope(RequestType requestType, HType Type, string iD)
    {
        Header = new(Request: requestType, Type: Type, ID: iD);
    }

    public RequestEnvelope(HType Type, string iD, StaticVariables? sv) : this(Type, iD)
    {
        Body.Desc.StaticVariables = sv;
    }




    [XmlElement(ElementName = "HEADER")]
    public Header? Header { get; set; }

    [XmlElement(ElementName = "BODY")]
    public RequestBody Body { get; set; } = new();
}
[XmlRoot(ElementName = "HEADER")]
public class Header
{
    public Header(RequestType Request, HType Type, string ID)
    {
        _request = Request;
        _type = Type;
        _Id = ID;
    }
    public Header() { }
    private int _version = 1;
    private RequestType _request;
    private HType _type;
    private string? _Id;
    [XmlElement(ElementName = "VERSION")]
    public int Version { get { return _version; } set { _version = value; } }

    [XmlElement(ElementName = "TALLYREQUEST")]
    public RequestType Request { get { return _request; } set { _request = value; } }

    [XmlElement(ElementName = "TYPE")]
    public HType Type { get { return _type; } set { _type = value; } }

    [XmlElement(ElementName = "ID")]
    public string? ID { get { return _Id; } set { _Id = value; } }
}
[XmlRoot(ElementName = "STATICVARIABLES")]
public class StaticVariables
{
    private string? _ExportFormat;

    public StaticVariables()
    {
        SVExportFormat = "XML";
    }

    [XmlElement(ElementName = "SVEXPORTFORMAT")]
    public string SVExportFormat { get { return _ExportFormat!; } set { _ExportFormat = $"$$SysName:{value}"; } }

    [XmlElement(ElementName = "SVCURRENTCOMPANY")]
    public string? SVCompany { get; set; }


    [XmlElement(ElementName = "SVFROMDATE")]
    public TallyDate? SVFromDate { get; set; }


    [XmlElement(ElementName = "SVTODATE")]
    public TallyDate? SVToDate { get; set; }

    //[XmlElement(ElementName = "SVViewName")]
    //public VoucherViewType? ViewName { get; set; }

    [XmlElement(ElementName = "EXPLODEFLAG")]
    public string? ExplodeFlag { get; set; }


}
[XmlRoot(ElementName = "BODY")]
public class RequestBody
{
    [XmlElement(ElementName = "DESC")]
    public ReqDescription Desc { get; set; } = new();

    [XmlElement(ElementName = "DATA")]
    public RequestData RequestData { get; set; } = new();
}

public class RequestData 
{

    public List<BaseObject>? Data { get; set; }
    [XmlElement(ElementName = "RESULT")]

    public string? FuncResult { get; set; } = null;

}

[XmlRoot(ElementName = "DESC")]
public class ReqDescription
{

    [XmlElement(ElementName = "STATICVARIABLES")]
    public StaticVariables? StaticVariables { get; set; } = new();

    [XmlElement(ElementName = "TDL")]
    public ReqTDL TDL { get; set; } = new();

    [XmlArray(ElementName = "FUNCPARAMLIST")]
    [XmlArrayItem(ElementName = "PARAM")]
    public List<string>? FunctionParams { get; set; }


}


[XmlRoot(ElementName = "TDL")]
public class ReqTDL
{
    public ReqTDL()
    {
    }

    [XmlElement(ElementName = "TDLMESSAGE")]
    public TDLMessage TDLMessage { get; set; } = new();
}

[XmlRoot(ElementName = "TDLMESSAGE")]
public class TDLMessage
{
    public TDLMessage()
    {
    }
    public TDLMessage(string colName,
                      string colType,
                      string? childof = null,
                      List<string>? nativeFields = null,
                      List<Filter>? filters = null,
                      List<string>? computevar = null,
                      List<string>? compute = null,
                      List<TallyCustomObject>? objects = null,
                      YesNo isInitialize = YesNo.No)
    {

        Object = objects;
        Collection.Add(new(colName: colName,
                           colType: colType,
                           childOf: childof,
                           nativeFields: nativeFields,
                           filters: filters?.Where(f => !f.ExcludeinCollection).Select(c => c.FilterName).ToList()!,
                           computevar: computevar,
                           compute: compute,
                           Isintialize: isInitialize));

        filters?.ForEach(filter => System.Add(new(name: filter.FilterName!,
                                                 text: filter.FilterFormulae!)));


    }


    [XmlElement(ElementName = "IMPORTFILE")]
    public List<ImportFile>? ImportFile { get; set; }

    [XmlElement(ElementName = "REPORT")]
    public List<Report>? Report { get; set; }

    [XmlElement(ElementName = "FORM")]
    public List<Form>? Form { get; set; }

    [XmlElement(ElementName = "PART")]
    public List<Part>? Part { get; set; }

    [XmlElement(ElementName = "LINE")]
    public List<Line>? Line { get; set; }

    [XmlElement(ElementName = "FIELD")]
    public List<Field>? Field { get; set; } = [];

    [XmlElement(ElementName = "OBJECT")]
    public List<TallyCustomObject>? Object { get; set; }

    [XmlElement(ElementName = "COLLECTION")]
    public List<Collection> Collection { get; set; } = [];

    [XmlElement(ElementName = "NAMESET")]
    public List<NameSet> NameSet { get; set; } = [];

    [XmlElement(ElementName = "FUNCTION")]
    public List<TDLFunction> Functions { get; set; } = [];

    [XmlElement(ElementName = "SYSTEM")]
    public List<System>? System { get; set; } = [];

}

public class ImportFile : BaseTDLClass
{
    public ImportFile(string name, List<string>? option = null)
    {
        Name = name;
        Option = option;
    }

    public ImportFile()
    {
    }
    [XmlElement(ElementName = "OPTION")]
    public List<string>? Option = [];


    [XmlElement(ElementName = "RESPONSEREPORT")]
    public string ResponseReport { get; set; }

    [XmlElement(ElementName = "DELETE")]
    public List<string>? Delete = [];

    [XmlElement(ElementName = "ADD")]
    public List<string>? Add = [];
}

public class NameSet : BaseTDLClass
{
    public NameSet()
    {
        SetAttributes();
    }
    public NameSet(string name)
    {
        Name = name;
        SetAttributes();
    }

    [XmlElement(ElementName = "LIST")]
    public List<string> List { get; set; }
}
public class TDLFunction : BaseTDLClass
{
    public TDLFunction()
    {
        SetAttributes();
    }

    public TDLFunction(string name)
    {
        Name = name;
        SetAttributes();
    }

    [XmlElement(ElementName = "Parameter")]
    public List<string> Parameters { get; set; } = [];

    [XmlElement(ElementName = "VARIABLES")]
    public List<string> Variables { get; set; } = [];

    [XmlElement(ElementName = "LocalFormula")]
    public List<string> LocalFormulas { get; set; } = [];

    [XmlElement(ElementName = "Returns")]
    public string? Returns { get; set; }

    [XmlElement(ElementName = "Action")]
    public List<string> Actions { get; set; } = [];
}

[XmlRoot(ElementName = "REPORT")]
public class Report : BaseTDLClass
{
    public Report() { }
    public Report(string rName)
    {
        Name = rName;
        FormName = rName;
        SetAttributes();
    }



    [XmlElement(ElementName = "FORMS")]
    public string? FormName { get; set; }

    [XmlElement(ElementName = "USE")]
    public List<string>? Use { get; set; }

    [XmlElement(ElementName = "VARIABLE")]
    public List<string>? Variable { get; set; }

    [XmlElement(ElementName = "REPEAT")]
    public List<string>? Repeat { get; set; }

    [XmlElement(ElementName = "SET")]
    public List<string>? Set { get; set; }
}

[XmlRoot(ElementName = "FORM")]
public class Form : BaseTDLClass
{
    public Form() { }

    public Form(string formName)
    {
        Name = formName;
        PartName = formName;
        SetAttributes();
    }

    [XmlElement(ElementName = "TOPPARTS")]
    public string? PartName { get; set; }

    [XmlElement(ElementName = "XMLTAG")]
    public string? ReportTag { get; set; }



    [XmlElement(ElementName = "USE")]
    public List<string>? Use { get; set; }

    [XmlElement(ElementName = "PARTS")]
    public List<string>? Parts { get; set; }

    [XmlElement(ElementName = "SET")]
    public List<string>? Set { get; set; }

    [XmlElement(ElementName = "OPTION")]
    public List<string>? Option { get; set; }

}


[XmlRoot(ElementName = "PART")]
public class Part : BaseTDLClass
{
    public Part(string topPartName, string? colName, string lineName)
    {
        Name = topPartName;
        Lines = [lineName];
        if (colName != null)
        {
            Repeat = $"{lineName} : {colName}";
        }
        SetAttributes();

    }
    public Part()
    {
    }

    public Part(string rootTag, string? collectionName)
    {
        Name = rootTag;
        Lines = [rootTag];
        if (collectionName != null)
        {
            Repeat = $"{rootTag} : {collectionName}";
        }
        SetAttributes();
    }

    [XmlElement(ElementName = "TOPLINES")]
    public List<string>? Lines { get; set; } //MustMatch with LineName

    [XmlElement(ElementName = "REPEAT")]
    public string? Repeat { get; set; }

    [XmlElement(ElementName = "SCROLLED")]
    public string? Scrolled { get; set; } = "Vertical";


    [XmlElement(ElementName = "XMLTAG")]
    public string? XMLTag { get; set; }

}



[XmlRoot(ElementName = "LINE")]
public class Line : BaseTDLClass
{
    public Line(string lineName, List<string> fields, string? xmlTag = null)
    {
        Name = lineName;
        Fields = fields;
        XMLTag = xmlTag;
        SetAttributes();
    }

    public Line(string lineName, string CollectionName)
    {
        Name = lineName;
        Fields = [lineName];
        Repeat = [$"{lineName} : {CollectionName}"];
        SetAttributes(isOption: YesNo.Yes);
    }

    public Line()
    {
    }

    [XmlElement(ElementName = "USE")]
    public string? Use { get; set; }

    [XmlElement(ElementName = "FIELDS")]
    public List<string>? Fields { get; set; }



    [XmlElement(ElementName = "XMLTAG")]
    public string? XMLTag { get; set; }

    [XmlElement(ElementName = "XMLATTR")]
    public List<string>? XMLAttributes { get; set; } = [];

    [XmlElement(ElementName = "OPTION")]
    public List<string> Option { get; set; } = [];

    [XmlElement(ElementName = "EXPLODE")]
    public List<string> Explode { get; set; } = [];

    [XmlElement(ElementName = "LOCAL")]
    public List<string> Local { get; set; } = [];

    [XmlElement(ElementName = "DELETE")]
    public List<string> Delete { get; set; } = [];


    [XmlElement(ElementName = "REPEAT")]
    public List<string>? Repeat { get; set; }

    [XmlElement(ElementName = "SCROLLED")]
    public string? Scrolled => Repeat is null ? null : "Vertical";

}




[XmlRoot(ElementName = "FIELD")]
public class Field : BaseTDLClass
{

    public Field(string name)
    {
        XMLTag = null;
        Set = null;
        Name = name;
        SetAttributes();
    }
    public Field(string name, string xMLTag)
    {
        XMLTag = xMLTag;
        Set = $"{xMLTag}";
        Name = name;
        SetAttributes();
    }
    public Field(string name, string xMLTag, string? set = null)
    {
        XMLTag = xMLTag;
        Set = set;
        Name = name;
        SetAttributes();
    }

    public Field(List<string> fields, Dictionary<string, string> repeatFields, string fieldName, string xmlTag)
    {
        Fields = fields;
        Name = fieldName;
        Option = [.. repeatFields.Select(kv => $"{kv.Value} : {kv.Key}")];

        XMLTag = xmlTag;
        SetAttributes();
    }
    public Field(List<string> fields, string fieldName, string xmltag)
    {
        Fields = fields;
        Name = fieldName;
        XMLTag = xmltag;
        SetAttributes();
    }
    public Field()
    {

    }

    [XmlElement(ElementName = "SET")]
    public string? Set { get; set; } //TallyFields Like $Name

    [XmlElement(ElementName = "XMLTAG")]
    public string? XMLTag { get; set; }  //Desired XML Tag


    [XmlElement(ElementName = "FIELDS")]
    public List<string>? Fields { get; set; }

    [XmlElement(ElementName = "XMLATTR")]
    public string? XMLAttr { get; set; }


    [XmlElement(ElementName = "REPEAT")]
    public string? Repeat { get; set; }

    [XmlElement(ElementName = "SCROLLED")]
    public string? Scrolled => Repeat is null ? null : "Vertical";

    [XmlElement(ElementName = "OPTION")]
    public List<string>? Option { get; set; }

    [XmlElement(ElementName = "TYPE")]
    public string? Type { get; set; }

    [XmlElement(ElementName = "USE")]
    public string? Use { get; set; }

    [XmlElement(ElementName = "FORMAT")]
    public string? Format { get; set; }

    [XmlElement(ElementName = "INVISIBLE")]
    public string? Invisible { get; set; }

    public static implicit operator Field(PropertyMetaData meta)
    {
        Field field = new(meta.Name, meta.XMLTag, meta.Set);
        if (meta.TDLType != null)
        {
            field.Type = meta.TDLType;
        }
        if (meta.Format != null)
        {
            field.Format = meta.Format;
        }
        if (meta.Invisible != null)
        {
            field.Invisible = meta.Invisible;
        }
        return field;
    }
}




[XmlRoot(ElementName = "COLLECTION")]
public class Collection : BaseTDLClass
{
    public Collection(string colName,
                      string colType,
                      string? childOf = null,
                      List<string>? nativeFields = null,
                      List<string>? filters = null,
                      List<string>? computevar = null,
                      List<string>? compute = null,
                      YesNo Isintialize = YesNo.No)
    {
        Name = colName;
        Type = colType;
        Childof = childOf;
        if (nativeFields != null)
        {
            NativeFields = nativeFields;
        }
        if (filters != null)
        {
            Filters = filters;
        }
        ComputeVar = [];
        if (computevar != null)
        {
            ComputeVar.AddRange(computevar);
        }
        Compute = [];
        if (compute != null)
        {
            Compute.AddRange(compute);
        }

        SetAttributes(isInitialize: Isintialize);


    }
    public Collection()
    {
        SetAttributes();
    }

    public Collection(string objcollectionName, string objects)
    {
        Name = objcollectionName;
        Objects = objects;
        SetAttributes();
    }


    [XmlElement(ElementName = "TYPE")]
    public string? Type { get; set; }    //Tally Table Name like - Company,Ledger ..etc;

    [XmlElement(ElementName = "CHILDOF")]
    public string? Childof { get; set; }

    [XmlElement(ElementName = "BELONGSTO")]
    public YesNo BelongsTo { get; set; }

    [XmlElement(ElementName = "SOURCECOLLECTION")]
    public string? Collections { get; set; }

    [XmlElement(ElementName = "NATIVEMETHOD")]
    public List<string>? NativeFields { get; set; }

    [XmlElement(ElementName = "COMPUTE")]
    public List<string>? Compute { get; set; }

    [XmlElement(ElementName = "COMPUTEVAR")]
    public List<string>? ComputeVar { get; set; }

    [XmlElement(ElementName = "WALK")]
    public string? Walk { get; set; }

    [XmlElement(ElementName = "BY")]
    public List<string>? By { get; set; }

    [XmlElement(ElementName = "AGGRCOMPUTE")]
    public List<string>? AggrCompute { get; set; }

    [XmlElement(ElementName = "SORT")]
    public List<string>? Sort { get; set; }

    [XmlElement(ElementName = "FILTERS")]
    public List<string>? Filters { get; set; }
    /// <summary>
    /// Name of Single or Multiple Custom objects Seperated by (,)
    /// </summary>
    [XmlElement(ElementName = "OBJECTS")]
    public string? Objects { get; set; }
    public string DataSource { get; set; }
}


[XmlRoot(ElementName = "SYSTEM")]
public class System
{
    public System()
    {
        Name = string.Empty;
        Text = string.Empty;
    }
    public System(string name, string text)
    {
        Name = name;
        Text = text;
    }
    [XmlAttribute(AttributeName = "TYPE")]
    public string Type { get { return "Formulae"; } set { } }

    [XmlAttribute(AttributeName = "NAME")]
    public string Name { get; set; }  //Name must match with Collection Filter

    [XmlText]
    public string Text { get; set; }
}

public class BaseTDLClass
{
    [XmlAttribute(AttributeName = "NAME")]
    public string Name { get; set; }

    [XmlAttribute(AttributeName = "ISMODIFY")]
    public YesNo IsModify { get; set; }

    [XmlAttribute(AttributeName = "ISFIXED")]
    public YesNo IsFixed { get; set; }

    [XmlAttribute(AttributeName = "ISINITIALIZE")]
    public YesNo IsInitialize { get; set; }

    [XmlAttribute(AttributeName = "ISOPTION")]
    public YesNo IsOption { get; set; }

    [XmlAttribute(AttributeName = "ISINTERNAL")]
    public YesNo IsInternal { get; set; }

    public void SetAttributes(YesNo ismodify = YesNo.No,
                              YesNo isFixed = YesNo.No,
                              YesNo isInitialize = YesNo.No,
                              YesNo isOption = YesNo.No,
                              YesNo isInternal = YesNo.No)
    {
        IsModify = ismodify;
        IsFixed = isFixed;
        IsInitialize = isInitialize;
        IsOption = isOption;
        IsInternal = isInternal;
    }

}


[XmlRoot(ElementName = "OBJECT")]
public class TallyCustomObject : BaseTDLClass
{
    public TallyCustomObject()
    {
        LocalFormulas = [];
        SetAttributes();
    }
    public TallyCustomObject(string name, List<string> formulas)
    {
        Name = name;
        LocalFormulas = formulas;
        SetAttributes();
    }

    [XmlElement(ElementName = "LOCALFORMULA")]
    public List<string> LocalFormulas { get; set; }
}

[XmlRoot(ElementName = "RESPONSE")]
public class FailureResponse
{

}

public class PResult
{
    public RespStatus Status { get; set; }

    public string? Result { get; set; }

}
public class TallyResult
{
    public RespStatus Status { get; set; }

    public string? Response { get; set; }
}
public enum RespStatus
{
    Sucess,
    Failure
}
public enum RequestType
{
    [XmlEnum(Name = "EXPORT")]
    Export,
    [XmlEnum(Name = "IMPORT")]
    Import,
    [XmlEnum(Name = "Import Data")]
    ImportData
}

public enum HType
{
    [XmlEnum(Name = "OBJECT")]
    Object,
    [XmlEnum(Name = "COLLECTION")]
    Collection,
    [XmlEnum(Name = "DATA")]
    Data,
    [XmlEnum(Name = "FUNCTION")]
    Function,
}