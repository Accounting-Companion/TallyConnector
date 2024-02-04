﻿namespace TallyConnector.Core.Attributes;
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class MaptoDTOAttribute<T> : Attribute where T : class
{
}
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class MapToPropertyAttribute() : Attribute
{
    public string PropertyPath { get; set; }
}
