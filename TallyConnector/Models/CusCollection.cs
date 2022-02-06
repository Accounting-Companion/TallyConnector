namespace TallyConnector.Models;
class CusCollection
{
}

[XmlRoot(ElementName = "ENVELOPE")]
public class CusColEnvelope : TallyXmlJson
{
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
        Field = new();
        foreach (var Fld in leftFields)
        {
            Field field = new(Fld.Key, Fld.Value);
            Field.Add(field);
        }
        foreach (var Fld in rightFields)
        {
            Field field = new(Fld.Key, Fld.Value);
            Field.Add(field);
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

    [XmlElement(ElementName = "REPORT")]
    public Report Report { get; set; }

    [XmlElement(ElementName = "FORM")]
    public Form Form { get; set; }

    [XmlElement(ElementName = "PART")]
    public Part Part { get; set; }

    [XmlElement(ElementName = "LINE")]
    public Line Line { get; set; }

    [XmlElement(ElementName = "FIELD")]
    public List<Field> Field { get; set; }

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
        Name = rName;
        FormName = formname;
        SetAttributes();

    }
    public Report() { }

    [XmlElement(ElementName = "FORMS")]
    public string FormName { get; set; }

    [XmlAttribute(AttributeName = "NAME")]
    public string Name { get; set; }

    [XmlText]
    public string Text { get; set; } //Form Name must be same as FormID
}

[XmlRoot(ElementName = "FORM")]
public class Form : DCollection
{
    public Form() { }

    public Form(string formName, string partName, string rootXML)
    {
        Caption = partName;
        ReportTag = rootXML;
        Name = formName;
        SetAttributes();
    }

    [XmlElement(ElementName = "TOPPARTS")]
    public string Caption { get; set; }

    [XmlElement(ElementName = "XMLTAG")]
    public string ReportTag { get; set; }

    [XmlAttribute(AttributeName = "NAME")]
    public string Name { get; set; } //Should match with FormName in Report

}


[XmlRoot(ElementName = "PART")]
public class Part : DCollection
{
    private string _Repeat;
    public Part(string topPartName, string colName, string lineName)
    {
        Name = topPartName;
        TopLines = lineName;
        _Repeat = $"{TopLines} : {colName}";
        SetAttributes();

    }
    public Part()
    {
    }

    [XmlElement(ElementName = "TOPLINES")]
    public string TopLines { get; set; } //MustMatch with LineName

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
    public Field(string TallyField, String xMLTag)
    {
        XMLTag = xMLTag;
        Set = TallyField;
        Name = xMLTag;
        SetAttributes();
    }

    public Field()
    {

    }

    [XmlElement(ElementName = "SET")]
    public string Set { get; set; } //TallyFields Like $Name

    [XmlElement(ElementName = "XMLTAG")]
    public string XMLTag { get; set; }  //Desired XML Tag

    [XmlAttribute(AttributeName = "NAME")]
    public string Name { get; set; }


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

    public string VCHID { get; set; }
}

public enum RespStatus
{
    Sucess,
    Failure
}
