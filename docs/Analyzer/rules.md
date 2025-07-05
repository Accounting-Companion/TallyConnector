# Analyzer Rules

| Rule ID    | Title                          | Description                                                      | Severity |
|------------|--------------------------------|------------------------------------------------------------------|----------|
| TC0001     | class needs to be partial       | if class has custom classes they need to be marked as partial | Error  | 
| TC0002     | class from external dll should  have generate Meta attribute  | If class from external dll has no generate meta attribute we cannot generate full meta for current class | Error  | 
| TC0003     | class should have TDL collection attribute with Type     | If Collection Type is not defined we assume assume class name as type | Warning  | 
| TCXML0001 | Property should have xml element attribute | we use XML Serialization and data comes from tally has all upper case XML Tag, so if we dont mention correct tag value, property value will be null during deserialization | Error | 
| TCXML0002 | Xml Element name should be upper case | we use XML Serialization and data comes from tally has all upper case XML Tag, so if we dont mention element value in uppercase, property value will be null during deserialization| Error | 
| TCXML0003 | Xml Root name should be upper case | we use XML Serialization and data comes from tally has all upper case XML Tag, so if we dont mention element value in uppercase, list will empty during deserialization | Error |
| TCXML0004 | if XML root is same as base class add XmlType(AnonymousType = true)| we use XML Serialization and if XML root is same for base class and inherited class XMLSerializer throws error | Error | 
