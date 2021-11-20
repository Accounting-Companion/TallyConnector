using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TallyConnector.Models
{
    [XmlRoot(ElementName = "ATTENDANCETYPE")]
    public class AttendanceType: TallyXmlJson
    {
        [XmlAttribute(AttributeName = "NAME")]
        [JsonIgnore]
        public string OldName { get; set; }

        private string name;
        [XmlElement(ElementName = "NAME")]
        [Required]
        public string Name
        {
            get { return (name == null || name == string.Empty) ? OldName : name; }
            set => name = value;
        }

        [XmlElement(ElementName = "ATTENDANCEPRODUCTIONTYPE")]
        public string ProductionType { get; set; }

        [XmlElement(ElementName = "ATTENDANCEPERIOD")]
        public string Period { get; set; }

        [XmlElement(ElementName = "PARENT")]
        public string Parent { get; set; }

        [XmlElement(ElementName = "BASEUNITS")]
        public string BaseUnit { get; set; }



        [XmlIgnore]
        public string VName { get; set; }

        [XmlIgnore]
        public string Alias
        {
            get
            {
                if (this.LanguageNameList.NameList.NAMES.Count > 0)
                {
                    if (VName == null)
                    {
                        VName = this.LanguageNameList.NameList.NAMES[0];
                    }
                    if (Name == VName)
                    {
                        this.LanguageNameList.NameList.NAMES[0] = this.Name;
                        return string.Join("..\n", this.LanguageNameList.NameList.NAMES.GetRange(1, this.LanguageNameList.NameList.NAMES.Count - 1));

                    }
                    else
                    {
                        //Name = this.LanguageNameList.NameList.NAMES[0];
                        return string.Join("..\n", this.LanguageNameList.NameList.NAMES);

                    }
                }
                else
                {
                    this.LanguageNameList.NameList.NAMES.Add(this.Name);
                    return null;
                }


            }
            set
            {
                this.LanguageNameList = new();
                
                if (value != null)
                {
                    List<string> lis = value.Split("..\n").ToList();

                    LanguageNameList.NameList.NAMES.Add(Name);
                    if (value != "")
                    {
                        LanguageNameList.NameList.NAMES.AddRange(lis);
                    }

                }
                else
                {
                    LanguageNameList.NameList.NAMES.Add(Name);
                }


            }
        }

        [XmlElement(ElementName = "CANDELETE")]
        public string CanDelete { get; set; } //Ignore This While Creating or Altering

        [JsonIgnore]
        [XmlElement(ElementName = "LANGUAGENAME.LIST")]
        public LanguageNameList LanguageNameList { get; set; }

        /// <summary>
        /// Accepted Values //Create, Alter, Delete
        /// </summary>
        [JsonIgnore]
        [XmlAttribute(AttributeName = "Action")]
        public string Action { get; set; }

    }



    [XmlRoot(ElementName = "ENVELOPE")]
    public class AttendanceEnvelope : TallyXmlJson
    {

        [XmlElement(ElementName = "HEADER")]
        public Header Header { get; set; }

        [XmlElement(ElementName = "BODY")]
        public AttendanceBody Body { get; set; } = new();
    }

    [XmlRoot(ElementName = "BODY")]
    public class AttendanceBody
    {
        [XmlElement(ElementName = "DESC")]
        public Description Desc { get; set; } = new();

        [XmlElement(ElementName = "DATA")]
        public AttendanceData Data { get; set; } = new();
    }

    [XmlRoot(ElementName = "DATA")]
    public class AttendanceData
    {
        [XmlElement(ElementName = "TALLYMESSAGE")]
        public AttendanceMessage Message { get; set; } = new();


        [XmlElement(ElementName = "COLLECTION")]
        public AttendanceTypeColl Collection { get; set; } = new AttendanceTypeColl();


    }

    [XmlRoot(ElementName = "COLLECTION")]
    public class AttendanceTypeColl
    {
        [XmlElement(ElementName = "ATTENDANCETYPE")]
        public List<AttendanceType> AttendanceTypes { get; set; }
    }


    [XmlRoot(ElementName = "TALLYMESSAGE")]
    public class AttendanceMessage
    {
        [XmlElement(ElementName = "ATTENDANCETYPE")]
        public AttendanceType AttendanceType { get; set; }
    }
}
