namespace TallyConnector.Models;
class CusCollection
{
}

[XmlRoot(ElementName = "ENVELOPE")]
public class CusColEnvelope : TallyXmlJson
{
    public CusColEnvelope()
    {
    }

    public CusColEnvelope(string reportName, StaticVariables staticVariables = null)
    {
        Body = new();
        Header = new(Request: RequestTye.Export, Type: HType.Collection, ID: reportName); //Configuring Header To get Export data
        Body.Desc.StaticVariables = staticVariables;
    }
    public CusColEnvelope(RequestTye RequestTye, HType Type, string reportName, StaticVariables staticVariables = null)
    {
        Body = new();
        Header = new(Request: RequestTye, Type: Type, ID: reportName); //Configuring Header To get Export data
        Body.Desc.StaticVariables = staticVariables;
    }
    [XmlElement(ElementName = "HEADER")]
    public Header Header { get; set; }

    [XmlElement(ElementName = "BODY")]
    public ColBody Body { get; set; } = new();
}

[XmlRoot(ElementName = "BODY")]
public class ColBody
{
    [XmlElement(ElementName = "DESC")]
    public ColDescription Desc { get; set; } = new();

    //[XmlElement(ElementName = "DATA")]
    //public LData Data { get; set; } = new LData();
}

[XmlRoot(ElementName = "DESC")]
public class ColDescription
{

    [XmlElement(ElementName = "STATICVARIABLES")]
    public StaticVariables StaticVariables { get; set; } = new();

    [XmlElement(ElementName = "TDL")]
    public ColTDL TDL { get; set; } = new();
}

[XmlRoot(ElementName = "TDL")]
public class ColTDL
{

    [XmlElement(ElementName = "TDLMESSAGE")]
    public ColTDLMessage TDLMessage { get; set; } = new();
}

[XmlRoot(ElementName = "TDLMESSAGE")]
public class ColTDLMessage
{
    public ColTDLMessage(string rName,
                         string fName,
                         string topPartName,
                         string rootXML,
                         string colName,
                         string lineName,
                         Dictionary<string, string> leftFields,
                         Dictionary<string, string> rightFields,
                         string colType,
                         List<string> filters = null,
                         List<string> SysFormulae = null)
    {
        Report = new(rName, fName);
        Form = new(fName, topPartName, rootXML);
        Part = new(topPartName, colName, lineName);
        List<string> LF = leftFields.Values.ToList();
        List<string> RF = rightFields.Values.ToList();
        Line = new(lineName, LF, RF);
        Fields = new();
        foreach (var Fld in leftFields)
        {
            Field field = new(Fld.Key, Fld.Value);
            Fields.Add(field);
        }
        foreach (var Fld in rightFields)
        {
            Field field = new(Fld.Key, Fld.Value);
            Fields.Add(field);
        }
        Collection = new(colName: colName, colType: colType, filters: filters);
        if (filters != null && SysFormulae != null)
        {
            for (int i = 0; i < SysFormulae.Count; i++)
            {
                System NSystem = new(filters[i], SysFormulae[i]);
                System.Add(NSystem);
            }
        }

    }

    public ColTDLMessage(string reportName,
                         string ColType)
    {

    }
    public ColTDLMessage(string colName,
                         string colType,
                         List<string> nativeFields,
                         List<string> filters = null,
                         List<string> SysFormulae = null)
    {
        Collection = new(colName: colName, colType: colType, nativeFields: nativeFields, filters: filters);
        if (filters != null && SysFormulae != null)
        {
            for (int i = 0; i < SysFormulae.Count; i++)
            {
                System NSystem = new(filters[i], SysFormulae[i]);
                System.Add(NSystem);
            }
        }
    }
    public ColTDLMessage(List<TallyCustomObject> tallyCustomObjects,
                         string objCollectionName,
                         string ObjNames)
    {
        Objects = tallyCustomObjects;
        Collection = new(objcollectionName: objCollectionName,
                         objects: ObjNames);
    }
    public ColTDLMessage()
    {

    }


    public ColTDLMessage(string colName,
                         string colType,
                         List<string> nativeFields,
                         List<Filter> filters = null)
    {
        List<string> TdlFilter = new();
        filters?.ForEach(filter =>
        {
            TdlFilter.Add(filter.FilterName);
            System.Add(new(filter.FilterName, filter.FilterFormulae));
        });
        Collection = new(colName: colName, colType: colType, nativeFields: nativeFields, filters: TdlFilter);

    }

    public ColTDLMessage(ReportField rootreportField)
    {
        string rootTag = rootreportField.FieldName;
        string name = $"LISTOF{rootTag}";
        string CollectionName = $"Custom{rootTag}Coll".ToUpper();
        Report = new(name);
        Form = new(name);
        Part = new(name, CollectionName);
        Line = new(name, rootTag);
        Fields = new();
        Field rootField = new(rootTag);
        Fields.Add(rootField);
        Dictionary<string, string> Repeatfields = new();
        List<string> RootFields = new();
        List<string> Fetchlist = new();

        GenerateFields(rootreportField, RootFields, Repeatfields, Fetchlist);
        rootField.Fields = string.Join(",", RootFields);
        rootField.Repeat = Repeatfields.Select(kv => $"{kv.Value} : {kv.Key}").ToList();
        Collection = new(colName: CollectionName, colType: rootTag);
        Collection.NativeFields = new() { string.Join(",", Fetchlist) };
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
                    Tfields.Add(field.FieldName);
                    fetchlist?.Add(field.XMLTag);
                    GenerateFields(field, TSfields, TSRepeatfields);

                    //Field Newf = new(new List<string>() { field.FieldName }, Repeatfields, $"C{field.FieldName}");
                    //Newf.XMLTag = string.Empty;
                    Fields.Add(new(TSfields, TSRepeatfields, field.FieldName, field.XMLTag));
                    //Fields.Add(Newf);
                }
                else
                {
                    Tfields.Add(field.FieldName);
                    fetchlist?.Add(field.XMLTag);
                    GenerateFields(field, TSfields, TSRepeatfields);

                    Fields.Add(new(TSfields, TSRepeatfields, field.FieldName, field.XMLTag));

                }

            }
            else
            {

                if (field.CollectionName != null)
                {
                    Repeatfields[field.CollectionName] = field.FieldName;
                    Tfields.Add(field.FieldName);
                    fetchlist?.Add(field.XMLTag);
                    //Fields.Add(new(TSfields, Repeatfields, field.XMLTag));
                    Fields.Add(new(field.FieldName, field.XMLTag));
                }
                else
                {
                    Tfields.Add(field.FieldName);
                    fetchlist?.Add(field.XMLTag);
                    Field Newf = new(field.FieldName, field.XMLTag);
                    Fields.Add(Newf);
                }

            }

        });
    }

    [XmlElement(ElementName = "REPORT")]
    public Report Report { get; set; }

    [XmlElement(ElementName = "FORM")]
    public Form Form { get; set; }

    [XmlElement(ElementName = "PART")]
    public Part Part { get; set; }

    [XmlElement(ElementName = "LINE")]
    public Line Line { get; set; }

    [XmlElement(ElementName = "FIELD")]
    public List<Field> Fields { get; set; }

    [XmlElement(ElementName = "OBJECT")]
    public List<TallyCustomObject> Objects { get; set; }

    [XmlElement(ElementName = "COLLECTION")]
    public Collection Collection { get; set; }

    [XmlElement(ElementName = "SYSTEM")]
    public List<System> System { get; set; } = new();

}

[XmlRoot(ElementName = "REPORT")]
public class Report : DCollection
{
    public Report(string rName, string formname)
    {
        AttrName = rName;
        FormName = formname;
        SetAttributes();

    }
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
        PartName = formName;
        ReportTag = formName;
        Name = formName;
        SetAttributes();
    }
    public Form(string formName, string partName, string rootXML)
    {
        PartName = partName;
        ReportTag = rootXML;
        Name = formName;
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
    public Part(string name, string colName)
    {
        Name = name;
        Lines = new() { name };
        _Repeat = $"{name} : {colName}";
        SetAttributes();

    }
    public Part()
    {
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
    public Line(string lineName, List<string> leftFields, List<string> rightFields)
    {
        Name = lineName;
        LeftFields = leftFields;
        RightFields = rightFields;
        SetAttributes();
    }
    public Line(string name, string rootfiledname)
    {
        Name = name;
        LeftFields = new() { rootfiledname };
    }
    public Line()
    {
    }

    [XmlElement(ElementName = "LEFTFIELDS")]
    public List<string> LeftFields { get; set; }

    [XmlElement(ElementName = "RIGHTFIELDS")]
    public List<string> RightFields { get; set; }

    [XmlAttribute(AttributeName = "NAME")]
    public string Name { get; set; }

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
    public Field(string xMLTag)
    {
        XMLTag = xMLTag;
        Set = $"${xMLTag}";
        Name = xMLTag;
        SetAttributes();
    }

    public Field(List<string> fields, Dictionary<string, string> repeatFields, string fieldName, string xmlTag)
    {
        Fields = string.Join(",", fields);
        Name = fieldName;
        Repeat = repeatFields.Select(kv => $"{kv.Value} : {kv.Key}").ToList();

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
    private bool _IsVerticleVisible => Repeat?.Count > 0;

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
    public List<string> Repeat { get; set; }

    [XmlElement(ElementName = "SCROLLED")]
    public string Scrolled { get { return _IsVerticleVisible ? "Vertical" : null; } set { } }


}


[XmlRoot(ElementName = "COLLECTION")]
public class Collection : DCollection
{
    public Collection(string colName, string colType, List<string> nativeFields = null,
        List<string> filters = null)
    {
        Name = colName;
        Type = colType;
        if (nativeFields != null)
        {
            NativeFields = nativeFields;
        }
        if (filters != null)
        {
            Filters = filters;
        }
        SetAttributes();


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
    public string IsModify { get; set; }

    [XmlAttribute(AttributeName = "ISFIXED")]
    public string IsFixed { get; set; }

    [XmlAttribute(AttributeName = "ISINITIALIZE")]
    public string IsInitialize { get; set; }

    [XmlAttribute(AttributeName = "ISOPTION")]
    public string IsOption { get; set; }

    [XmlAttribute(AttributeName = "ISINTERNAL")]
    public string IsInternal { get; set; }

    public void SetAttributes(string ismodify = "No", string isFixed = "No", string isInitialize = "No",
        string isOption = "No", string isInternal = "No")
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

[XmlRoot(ElementName = "ENVELOPE")]
public class ResponseEnvelope
{

    [XmlElement(ElementName = "HEADER")]
    public RHeader Header { get; set; }

    [XmlElement(ElementName = "BODY")]
    public RBody Body { get; set; }
}

[XmlRoot(ElementName = "HEADER")]
public class RHeader
{

    [XmlElement(ElementName = "VERSION")]
    public int Version { get; set; }

    [XmlElement(ElementName = "STATUS")]
    public int Status { get; set; }
}

[XmlRoot(ElementName = "BODY")]
public class RBody
{

    [XmlElement(ElementName = "DATA")]
    public Rdata Data { get; set; }

    //[XmlElement(ElementName = "DESC")]
    //public Desc Desc { get; set; }
}

[XmlRoot(ElementName = "DATA")]
public class Rdata
{
    [XmlElement(ElementName = "LINEERROR")]
    public string LineError { get; set; }

    [XmlElement(ElementName = "IMPORTRESULT")]
    public ImportResult ImportResult { get; set; }
}

[XmlRoot(ElementName = "IMPORTRESULT")]
public class ImportResult
{


    [XmlElement(ElementName = "CREATED")]
    public int? Created { get; set; }

    [XmlElement(ElementName = "ALTERED")]
    public int? Altered { get; set; }

    [XmlElement(ElementName = "DELETED")]
    public int? Deleted { get; set; }

    [XmlElement(ElementName = "LASTVCHID")]
    public int? LastVchId { get; set; }

    [XmlElement(ElementName = "LASTMID")]
    public int? LastMID { get; set; }

    [XmlElement(ElementName = "COMBINED")]
    public int? Combined { get; set; }

    [XmlElement(ElementName = "IGNORED")]
    public int? Ignored { get; set; }

    [XmlElement(ElementName = "ERRORS")]
    public int? Errors { get; set; }

    [XmlElement(ElementName = "CANCELLED")]
    public int? Cacelled { get; set; }
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
