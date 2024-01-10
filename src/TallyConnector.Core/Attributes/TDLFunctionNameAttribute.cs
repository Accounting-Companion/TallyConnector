namespace TallyConnector.Core.Attributes;
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false,Inherited =true)]
public class BaseMethodNameAttribute : Attribute
{
    public BaseMethodNameAttribute(string functionName)
    {
        FunctionName = functionName;
    }

    public string FunctionName { get; set; }

}

public class TDLFunctionsMethodNameAttribute : BaseMethodNameAttribute
{
    public TDLFunctionsMethodNameAttribute(string functionName) : base(functionName)
    {
    }
}


public class TDLCollectionsMethodNameAttribute : BaseMethodNameAttribute
{
    public TDLCollectionsMethodNameAttribute(string functionName) : base(functionName)
    {
    }
}
public class TDLFiltersMethodNameAttribute : BaseMethodNameAttribute
{
    public TDLFiltersMethodNameAttribute(string functionName) : base(functionName)
    {
    }
}
public class TDLComputeMethodNameAttribute : BaseMethodNameAttribute
{
    public TDLComputeMethodNameAttribute(string functionName) : base(functionName)
    {
    }
}
