namespace TallyConnector.Core.Attributes;
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class BaseMethodNameAttribute : Attribute
{
    public BaseMethodNameAttribute()
    {
    }

    public BaseMethodNameAttribute(string functionName)
    {
        FunctionName = functionName;
    }

    public string FunctionName { get; set; }

}

public class TDLFunctionsMethodNameAttribute : BaseMethodNameAttribute
{
    public TDLFunctionsMethodNameAttribute()
    {
    }

    public TDLFunctionsMethodNameAttribute(string functionName) : base(functionName)
    {
    }
}


public class TDLCollectionsMethodNameAttribute : BaseMethodNameAttribute
{
    public TDLCollectionsMethodNameAttribute()
    {
    }

    public TDLCollectionsMethodNameAttribute(string functionName) : base(functionName)
    {
    }
}
public class TDLReportsMethodNameAttribute : BaseMethodNameAttribute
{
    public TDLReportsMethodNameAttribute()
    {
    }

    public TDLReportsMethodNameAttribute(string functionName) : base(functionName)
    {
    }
}
public class TDLFiltersMethodNameAttribute : BaseMethodNameAttribute
{
    public TDLFiltersMethodNameAttribute()
    {
    }

    public TDLFiltersMethodNameAttribute(string functionName) : base(functionName)
    {
    }
}
public class TDLComputeMethodNameAttribute : BaseMethodNameAttribute
{
    public TDLComputeMethodNameAttribute()
    {
    }

    public TDLComputeMethodNameAttribute(string functionName) : base(functionName)
    {
    }
}
public class TDLNamesetMethodNameAttribute : BaseMethodNameAttribute
{
    public TDLNamesetMethodNameAttribute()
    {
    }

    public TDLNamesetMethodNameAttribute(string functionName) : base(functionName)
    {
    }
}
