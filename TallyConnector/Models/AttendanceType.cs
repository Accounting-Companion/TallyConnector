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
    public class AttendanceType : TallyXmlJson
    {
        public AttendanceType()
        {
            LanguageNameList = new();
        }

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
        public string Alias { get; set; }

        [XmlElement(ElementName = "CANDELETE")]
        public string CanDelete { get; set; } //Ignore This While Creating or Altering

        [JsonIgnore]
        [XmlElement(ElementName = "LANGUAGENAME.LIST")]
        public List<LanguageNameList> LanguageNameList { get; set; }

        /// <summary>
        /// Accepted Values //Create, Alter, Delete
        /// </summary>
        [JsonIgnore]
        [XmlAttribute(AttributeName = "Action")]
        public string Action { get; set; }

        public void CreateNamesList()
        {
            if (this.LanguageNameList.Count == 0)
            {
                this.LanguageNameList.Add(new LanguageNameList());
                this.LanguageNameList[0].NameList.NAMES.Add(this.Name);

            }
            if (this.Alias != null && this.Alias != string.Empty)
            {
                this.LanguageNameList[0].LanguageAlias = this.Alias;
            }
        }

    }



    [XmlRoot(ElementName = "ENVELOPE")]
    public class AttendanceTypeEnvelope : TallyXmlJson
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
