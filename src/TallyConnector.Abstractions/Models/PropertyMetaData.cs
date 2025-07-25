﻿using System.Reflection;

namespace TallyConnector.Abstractions.Models;
public class PropertyMetaData
{

    public static Type Type => typeof(PropertyMetaData);


    public PropertyMetaData(string name, string xmlTag) : this(name, xmlTag, $"${xmlTag}")
    {
    }
    public PropertyMetaData(string name, string xmlTag, string set) : this(name, xmlTag, set, set?.Replace("$", ""))
    {
    }

    public PropertyMetaData(string name,
                            string xMLTag,
                            string set,
                            string? fetchText,
                            string? invisible = null,
                            string? tDLType = null,
                            string? format = null)
    {
        Name = name;
        XMLTag = xMLTag;
        Set = set;
        FetchText = fetchText;
        Invisible = invisible;
        TDLType = tDLType;
        Format = format;
    }

    public string Name { get; set; }
    public string XMLTag { get; set; }
    public string Set { get; set; }
    public string? FetchText { get; set; }

    public string? Invisible { get; set; }

    public string? TDLType { get; set; }

    public string? Format { get; set; }
}

public class PropertyMetaData<T> : PropertyMetaData
{
    public PropertyMetaData(string name, string xmlTag) : base(name, xmlTag)
    {
    }

    public PropertyMetaData(string name, string xmlTag, string set) : base(name, xmlTag, set)
    {
    }

    public PropertyMetaData(string name, string xMLTag, string set, string? fetchText, string? invisible = null, string? tDLType = null, string? format = null) : base(name, xMLTag, set, fetchText, invisible, tDLType, format)
    {
    }

    // Dummy operators to satisfy Expression<Func<T, bool>>
    public static bool operator ==(PropertyMetaData<T> prop, T value) => false;
    public static bool operator !=(PropertyMetaData<T> prop, T _value) => false;

    public static bool operator >(PropertyMetaData<T> prop, T value) => false;
    public static bool operator <(PropertyMetaData<T> prop, T value) => false;
    public static bool operator >=(PropertyMetaData<T> prop, T value) => false;
    public static bool operator <=(PropertyMetaData<T> prop, T value) => false;

    public bool Contains(string _) => false;
    public bool StartsWith(string _) => false;
    public bool EndsWith(string _) => false;
    public bool Like(string pattern) => false;

    public override bool Equals(object? obj) => base.Equals(obj);
    public override int GetHashCode() => base.GetHashCode();
}