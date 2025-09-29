namespace TallyConnector.Abstractions.Models;


public class Filter
{
    public Filter()
    {
    }

    public Filter(string filterName, string filterFormulae, bool excludeinCollection = false)
    {
        FilterName = filterName;
        FilterFormulae = filterFormulae;
        ExcludeinCollection = excludeinCollection;
    }

    public string FilterName { get; set; }

    public string FilterFormulae { get; set; }
    public bool ExcludeinCollection { get; set; }
}
public abstract class FilterCondition
{
    public string Name { get; set; }
    public static FilterCondition And(FilterCondition left, FilterCondition right) => new CompositeFilterCondition(left, right, true);
    public static FilterCondition Or(FilterCondition left, FilterCondition right) => new CompositeFilterCondition(left, right, false);
}
public enum FilterOperator
{
    Equals, NotEquals, GreaterThan, GreaterThanOrEqual, LessThan, LessThanOrEqual,
    StartsWith, Contains, EndsWith
}
public class SimpleFilterCondition : FilterCondition
{
    public PropertyMetaData Property { get; }
    public FilterOperator Operator { get; }
    public object Value { get; }
    public SimpleFilterCondition(PropertyMetaData property, FilterOperator op, object value) { Property = property; Operator = op; Value = value; }

    public override string ToString()
    {
        var value = Value is string stringVal ? $"\"{stringVal}\"" : Value.ToString();
        switch (Operator)
        {
            case FilterOperator.Equals:
                return $"{Property.Set} = {value}";
            case FilterOperator.NotEquals:
                return $"{Property.Set} <> {value}";
            case FilterOperator.GreaterThan:
                return $"{Property.Set} > {value}";
            case FilterOperator.GreaterThanOrEqual:
                return $"{Property.Set} >= {value}";
            case FilterOperator.LessThan:
                return $"{Property.Set} < {value}";
            case FilterOperator.LessThanOrEqual:
                return $"{Property.Set} <= {value}";
            case FilterOperator.StartsWith:
                return $"{Property.Set} Starts with {value}";
            case FilterOperator.Contains:
                return $"{Property.Set} Contains {value}";
            case FilterOperator.EndsWith:
                return $"{Property.Set} Endwith {value}";
        }
        return string.Empty;
    }
}
public class CompositeFilterCondition : FilterCondition
{
    public FilterCondition Left { get; }
    public FilterCondition Right { get; }
    public bool IsAndOperator { get; }
    public CompositeFilterCondition(FilterCondition left, FilterCondition right, bool isAnd) { Left = left; Right = right; IsAndOperator = isAnd; }

    public override string ToString()
    {
        return $"{Left} {(IsAndOperator ? "AND" : "OR")} {Right}";
    }
}
