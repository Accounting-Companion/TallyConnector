using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TallyConnector.Models
{
    class CusCollection
    {
    }

    [XmlRoot(ElementName = "ENVELOPE")]
    public class CusColEnvelope:TallyXmlJson
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
        public ColTDLMessage(string rName, string fName,string topPartName,string rootXML,
            string colName,string lineName,Dictionary<string,string> leftFields,
            Dictionary<string, string> rightFields, string colType,
            List<string> filters = null, List<string> SysFormulae = null)
        {
            Report = new(rName, fName);
            Form = new(fName,topPartName,rootXML);
            Part = new(topPartName,colName, lineName);
            List<string> LF = leftFields.Values.ToList();
            List<string> RF = rightFields.Values.ToList();
            Line = new(lineName,LF,RF);
            Field = new();
            foreach (var Fld in leftFields)
            {
                Field field = new(Fld.Key,Fld.Value);
                Field.Add(field);
            }
            foreach (var Fld in rightFields)
            {
                Field field = new(Fld.Key, Fld.Value);
                Field.Add(field);
            }
            Collection = new(colName,colType, filters, SysFormulae);
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

        [XmlElement(ElementName = "COLLECTION")]
        public Collection Collection { get; set; }



    }

    [XmlRoot(ElementName = "REPORT")]
    public class Report : DCollection
    {
        public Report(string rName, string formname)
        {
            Name = rName;
            FormName = formname;

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
    public class Form: DCollection
    {
        public Form() { }

        public Form(string formName,string partName,string rootXML) 
        {
            Caption = partName;
            ReportTag = rootXML;
            Name = formName;
        }

        [XmlElement(ElementName = "TOPPARTS")]
        public string Caption { get; set; }

        [XmlElement(ElementName = "XMLTAG")]
        public string ReportTag { get; set; }

        [XmlAttribute(AttributeName = "NAME")]
        public string Name { get; set; } //Should match with FormName in Report

    }


    [XmlRoot(ElementName = "PART")]
    public class Part: DCollection
    {
        private string _Repeat;
        public Part(string topPartName,string colName,string lineName)
        {
            Name = topPartName;
            TopLines = lineName;
            _Repeat = $"{TopLines} : {colName}";

        }
        public Part()
        {
        }

        [XmlElement(ElementName = "TOPLINES")]
        public string TopLines { get; set; } //MustMatch with LineName

        [XmlElement(ElementName = "REPEAT")]
        public string Repeat { get {return _Repeat; } set { } }

        [XmlElement(ElementName = "SCROLLED")]
        public string Scrolled { get { return "Vertical"; }set { } }

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
        public Field(string TallyField,String xMLTag )
        {
            XMLTag = xMLTag;
            Set = TallyField;
            Name = xMLTag;
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
        public Collection(string colName, string colType,List<string> filters = null, List<string> SysFormulae = null)
        {
            Name = colName;
            Type = colType;

        }
        public Collection()
        {

        }

        [XmlElement(ElementName = "TYPE")]
        public string Type { get; set; }    //Tally Table Name like - Company,Ledger ..etc;

        [XmlElement(ElementName = "FILTERS")]
        public List<string> Filters { get; set; } 

        [XmlAttribute(AttributeName = "NAME")]
        public string Name { get; set; }

        [XmlElement(ElementName = "SYSTEM")]
        public List<System> SYSTEM { get; set; }
    }

    [XmlRoot(ElementName = "SYSTEM")]
    public class System
    {

        [XmlAttribute(AttributeName = "TYPE")]
        public string Type { get; set; }

        [XmlAttribute(AttributeName = "NAME")]
        public string Name { get; set; }  //Name must match with Collection Filter

        [XmlText]
        public string Text { get; set; }
    }
    public class DCollection
    {

        [XmlAttribute(AttributeName = "ISMODIFY")]
        public string IsModify { get { return "No"; } set { } }

        [XmlAttribute(AttributeName = "ISFIXED")]
        public string IsFixed { get { return "No"; } set { } }

        [XmlAttribute(AttributeName = "ISINITIALIZE")]
        public string IsInitialize { get { return "No"; } set { } }

        [XmlAttribute(AttributeName = "ISOPTION")]
        public string IsOption { get { return "No"; } set { } }

        [XmlAttribute(AttributeName = "ISINTERNAL")]
        public string IsInternal { get { return "No"; } set { } }

    }
}
