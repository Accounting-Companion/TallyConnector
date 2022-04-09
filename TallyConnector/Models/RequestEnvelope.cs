namespace TallyConnector.Models;

[XmlRoot(ElementName = "ENVELOPE")]
public class RequestEnvelope : TallyXmlJson
{
    public RequestEnvelope()
    {
    }

    public RequestEnvelope(HType Type, string iD)
    {
        Header = new(Request: RequestTye.Export, Type: Type, ID: iD); //Configuring Header To get Export data
    }

    public RequestEnvelope(HType Type, string iD, StaticVariables sv) : this(Type, iD)
    {
        Body.Desc.StaticVariables = sv;
    }

    public RequestEnvelope(HType Type, string iD, StaticVariables sv, ReportField rootreportfield) : this(Type, iD, sv)
    {
        Body.Desc.TDL.TDLMessage = new(rootreportfield);
    }

    [XmlElement(ElementName = "HEADER")]
    public Header Header { get; set; }

    [XmlElement(ElementName = "BODY")]
    public RequestBody Body { get; set; } = new();
}

[XmlRoot(ElementName = "BODY")]
public class RequestBody
{
    [XmlElement(ElementName = "DESC")]
    public ReqDescription Desc { get; set; } = new();
}

[XmlRoot(ElementName = "DESC")]
public class ReqDescription
{

    [XmlElement(ElementName = "STATICVARIABLES")]
    public StaticVariables StaticVariables { get; set; } = new();

    [XmlElement(ElementName = "TDL")]
    public ReqTDL TDL { get; set; } = new();

    [XmlElement(ElementName = "FUNCPARAMLIST")]
    public FunctionParam FunctionParams { get; set; }


}

[XmlRoot(ElementName = "FUNCPARAMLIST")]
public class FunctionParam
{
    public FunctionParam()
    {
    }
    public FunctionParam(List<string> param)
    {
        Param = param;
    }
    [XmlElement(ElementName = "PARAM")]
    public List<string> Param { get; set; }


}
[XmlRoot(ElementName = "TDL")]
public class ReqTDL
{

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
                      string childof,
                      List<string> nativeFields,
                      List<Filter> filters,
                      YesNo isInitialize)
    {
        Collection.Add(new(colName: colName,
                           colType: colType,
                           childOf: childof,
                           nativeFields: nativeFields,
                           filters: filters?.Select(c => c.FilterName).ToList(),
                           Isintialize: isInitialize));

        filters?.ForEach(filter => System.Add(new(name: filter.FilterName,
                                                 text: filter.FilterFormulae)));
    }


    public TDLMessage(List<TallyCustomObject> tallyCustomObjects,
                         string objCollectionName,
                         string ObjNames)
    {
        Object = tallyCustomObjects;
        Collection.Add(new(objcollectionName: objCollectionName,
                         objects: ObjNames));
    }

    public TDLMessage(ReportField rootreportField, List<Filter> filters = null)
    {
        string rName = $"LISTOF{rootreportField.FieldName}";
        string CollectionName = rootreportField.CollectionName;
        Report = new() { new(rName) };
        Form = new() { new(rName) };
        Part = new() { new(rName, CollectionName) };
        List<string> rootlinefields = new();
        Line RootLine = new(rName, new(), rootreportField.FieldName);
        Line = new() { RootLine };
        Dictionary<string, string> Repeatfields = new();
        List<string> Fetchlist = new();

        GenerateFields(rootreportField, rootlinefields, Repeatfields, Fetchlist);
        RootLine.Fields.Add(string.Join(",", rootlinefields));
        foreach (var key in Repeatfields.Keys)
        {
            var value = Repeatfields[key];
            RootLine.Option.Add($"{value}:$$NumItems:{key} > 0");
            Line.Add(new Line(lineName: value, CollectionName: key));
        }
        Collection = new() { new(CollectionName, rootreportField.CollectionType, null, new() { string.Join(",", Fetchlist) }, filters?.Select(c => c.FilterName).ToList()) };

    }

    private void GenerateFields(ReportField rootreportField, List<string> Tfields, Dictionary<string, string> Repeatfields, List<string> fetchlist = null)
    {

        rootreportField.SubFields?.ForEach(field =>
        {
            List<string> TSfields = new();
            Dictionary<string, string> TSRepeatfields = new();
            if (field.SubFields.Count > 0)
            {
                if (field.CollectionName != null)
                {
                    Repeatfields[field.CollectionName] = field.FieldName;
                    //Tfields.Add(field.FieldName);
                    fetchlist?.Add(field.XMLTag);
                    GenerateFields(field, TSfields, TSRepeatfields);

                    //Field Newf = new(new List<string>() { field.FieldName }, Repeatfields, $"C{field.FieldName}");
                    //Newf.XMLTag = string.Empty;
                    Field.Add(new(TSfields, TSRepeatfields, field.FieldName, field.XMLTag));
                    //Fields.Add(Newf);
                }
                else
                {
                    Tfields.Add(field.FieldName);
                    fetchlist?.Add(field.XMLTag);
                    GenerateFields(field, TSfields, TSRepeatfields);

                    Field.Add(new(TSfields, TSRepeatfields, field.FieldName, field.XMLTag));

                }

            }
            else
            {

                if (field.CollectionName != null)
                {
                    Repeatfields[field.CollectionName] = field.FieldName;
                    //Tfields.Add(field.FieldName);
                    fetchlist?.Add(field.XMLTag);
                    //Fields.Add(new(TSfields, Repeatfields, field.XMLTag));
                    Field.Add(new(field.FieldName, field.XMLTag));
                }
                else
                {
                    Tfields.Add(field.FieldName);
                    fetchlist?.Add(field.XMLTag);
                    Field Newf = new(field.FieldName, field.XMLTag);
                    Field.Add(Newf);
                }

            }

        });
    }

    [XmlElement(ElementName = "REPORT")]
    public List<Report> Report { get; set; }

    [XmlElement(ElementName = "FORM")]
    public List<Form> Form { get; set; }

    [XmlElement(ElementName = "PART")]
    public List<Part> Part { get; set; }

    [XmlElement(ElementName = "LINE")]
    public List<Line> Line { get; set; }

    [XmlElement(ElementName = "FIELD")]
    public List<Field> Field { get; set; } = new();

    [XmlElement(ElementName = "OBJECT")]
    public List<TallyCustomObject> Object { get; set; }

    [XmlElement(ElementName = "COLLECTION")]
    public List<Collection> Collection { get; set; } = new();

    [XmlElement(ElementName = "SYSTEM")]
    public List<System> System { get; set; } = new();

}

[XmlRoot(ElementName = "REPORT")]
public class Report : DCollection
{
    public Report() { }
    public Report(string rName)
    {
        AttrName = rName;
        FormName = rName;
        SetAttributes();
    }

    [XmlAttribute(AttributeName = "NAME")]
    public string AttrName { get; set; }

    [XmlElement(ElementName = "FORMS")]
    public string FormName { get; set; }

    [XmlElement(ElementName = "USE")]
    public List<string> Use { get; set; }

    [XmlElement(ElementName = "VARIABLE")]
    public List<string> Variable { get; set; }

    [XmlElement(ElementName = "REPEAT")]
    public List<string> Repeat { get; set; }

    [XmlElement(ElementName = "SET")]
    public List<string> Set { get; set; }
}

[XmlRoot(ElementName = "FORM")]
public class Form : DCollection
{
    public Form() { }

    public Form(string formName)
    {
        Name = formName;
        PartName = formName;
        SetAttributes();
    }

    [XmlElement(ElementName = "TOPPARTS")]
    public string PartName { get; set; }

    [XmlElement(ElementName = "XMLTAG")]
    public string ReportTag { get; set; }

    [XmlAttribute(AttributeName = "NAME")]
    public string Name { get; set; } //Should match with FormName in Report

    [XmlElement(ElementName = "USE")]
    public List<string> Use { get; set; }

    [XmlElement(ElementName = "PARTS")]
    public List<string> Parts { get; set; }

    [XmlElement(ElementName = "SET")]
    public List<string> Set { get; set; }

}


[XmlRoot(ElementName = "PART")]
public class Part : DCollection
{
    private string _Repeat;
    public Part(string topPartName, string colName, string lineName)
    {
        Name = topPartName;
        Lines = new() { lineName };
        _Repeat = $"{lineName} : {colName}";
        SetAttributes();

    }
    public Part()
    {
    }

    public Part(string rootTag, string collectionName)
    {
        Name = rootTag;
        Lines = new() { rootTag };
        _Repeat = $"{rootTag} : {collectionName}";
        SetAttributes();
    }

    [XmlElement(ElementName = "TOPLINES")]
    public List<string> Lines { get; set; } //MustMatch with LineName

    [XmlElement(ElementName = "REPEAT")]
    public string Repeat { get { return _Repeat; } set { } }

    [XmlElement(ElementName = "SCROLLED")]
    public string Scrolled { get { return "Vertical"; } set { } }

    [XmlAttribute(AttributeName = "NAME")]
    public string Name { get; set; }

}



[XmlRoot(ElementName = "LINE")]
public class Line : DCollection
{
    private bool _IsVerticleVisible => Repeat is null;
    public Line(string lineName, List<string> fields, string xmlTag)
    {
        Name = lineName;
        Fields = fields;
        XMLTag = xmlTag;
        SetAttributes();
    }

    public Line(string lineName, string CollectionName)
    {
        Name = lineName;
        Fields = new() { lineName };
        Repeat = $"{lineName} : {CollectionName}";
        SetAttributes(isOption: YesNo.Yes);
    }

    public Line()
    {
    }

    [XmlElement(ElementName = "FIELDS")]
    public List<string> Fields { get; set; }

    [XmlAttribute(AttributeName = "NAME")]
    public string Name { get; set; }

    [XmlElement(ElementName = "XMLTAG")]
    public string XMLTag { get; set; }

    [XmlElement(ElementName = "OPTION")]
    public List<string> Option { get; set; } = new();


    [XmlElement(ElementName = "REPEAT")]
    public string Repeat { get; set; }

    [XmlElement(ElementName = "SCROLLED")]
    public string Scrolled { get { return _IsVerticleVisible ? "Vertical" : null; } set { } }

}




[XmlRoot(ElementName = "FIELD")]
public class Field : DCollection
{

    public Field(string name, string xMLTag)
    {
        XMLTag = xMLTag;
        Set = $"${xMLTag}";
        Name = name;
        SetAttributes();
    }

    public Field(List<string> fields, Dictionary<string, string> repeatFields, string fieldName, string xmlTag)
    {
        Fields = string.Join(",", fields);
        Name = fieldName;
        Option = repeatFields.Select(kv => $"{kv.Value} : {kv.Key}").ToList();

        XMLTag = xmlTag;
        SetAttributes();
    }
    public Field(List<string> fields, string fieldName, string xmltag)
    {
        Fields = string.Join(",", fields);
        Name = fieldName;
        XMLTag = xmltag;
        SetAttributes();
    }
    public Field()
    {

    }
    private bool _IsVerticleVisible => Repeat is null;

    [XmlElement(ElementName = "SET")]
    public string Set { get; set; } //TallyFields Like $Name

    [XmlElement(ElementName = "XMLTAG")]
    public string XMLTag { get; set; }  //Desired XML Tag

    [XmlAttribute(AttributeName = "NAME")]
    public string Name { get; set; }

    [XmlElement(ElementName = "FIELDS")]
    public string Fields { get; set; }

    [XmlElement(ElementName = "XMLATTR")]
    public string XMLAttr { get; set; }


    [XmlElement(ElementName = "REPEAT")]
    public string Repeat { get; set; }

    [XmlElement(ElementName = "SCROLLED")]
    public string Scrolled { get { return _IsVerticleVisible ? "Vertical" : null; } set { } }

    [XmlElement(ElementName = "OPTION")]
    public List<string> Option { get; set; }
}




[XmlRoot(ElementName = "COLLECTION")]
public class Collection : DCollection
{
    public Collection(string colName,
                      string colType,
                      string childOf,
                      List<string> nativeFields = null,
                      List<string> filters = null,
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
        SetAttributes(isInitialize: Isintialize);


    }
    public Collection()
    {

    }

    public Collection(string objcollectionName, string objects)
    {
        Name = objcollectionName;
        this.Objects = objects;
    }

    [XmlElement(ElementName = "TYPE")]
    public string Type { get; set; }    //Tally Table Name like - Company,Ledger ..etc;

    [XmlElement(ElementName = "CHILDOF")]
    public string Childof { get; set; }

    [XmlElement(ElementName = "NATIVEMETHOD")]
    public List<string> NativeFields { get; set; }

    [XmlElement(ElementName = "FILTERS")]
    public List<string> Filters { get; set; }
    /// <summary>
    /// Name of Single or Multiple Custom objects Seperated by (,)
    /// </summary>
    [XmlElement(ElementName = "OBJECTS")]
    public string Objects { get; set; }

    [XmlAttribute(AttributeName = "NAME")]
    public string Name { get; set; }


}


[XmlRoot(ElementName = "SYSTEM")]
public class System
{
    public System() { }
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

public class DCollection
{

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
public class TallyCustomObject : DCollection
{
    public TallyCustomObject()
    {
        LocalFormulas = new();
    }
    public TallyCustomObject(string name, List<string> formulas)
    {
        Name = name;
        LocalFormulas = formulas;
    }
    [XmlAttribute(AttributeName = "NAME")]
    public string Name { get; set; }

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

    public String Result { get; set; }

    public string VoucherMasterId { get; set; }
}

public enum RespStatus
{
    Sucess,
    Failure
}