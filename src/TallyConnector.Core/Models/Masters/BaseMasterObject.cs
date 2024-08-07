﻿using TallyConnector.Core.Models.Interfaces.Masters;

namespace TallyConnector.Core.Models.Masters;


public class BaseMasterObject : TallyObject, IBaseMasterObject
{


    [XmlElement(ElementName = "NAME")]
    [Required]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string Name { get; set; }
}