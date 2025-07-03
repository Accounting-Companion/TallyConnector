namespace TallyConnector.Core.Models.Request;

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
/// <summary>
/// A fluent builder for creating filter formula strings.
/// </summary>
public class FilterBuilder
{
    private readonly StringBuilder _formula = new();

    /// <summary>
    /// Starts a new condition for a specific field.
    /// This is the entry point for creating a condition like "Age > 18".
    /// </summary>
    /// <param name="fieldName">The name of the field to filter on.</param>
    /// <returns>A condition builder to specify the operator and value.</returns>
    public ConditionBuilder Field(string fieldName)
    {
        //// We add a space if the formula isn't empty and doesn't end with an opening parenthesis
        //if (_formula.Length > 0 && _formula[^1] != '(')
        //{
        //    _formula.Append(' ');
        //}
        AppendFieldName(fieldName);
        return new ConditionBuilder(this, _formula);
    }
    // Helper to avoid repeating the "add space and field name" logic
    private void AppendFieldName(string fieldName)
    {
        if (_formula.Length > 0 && !char.IsWhiteSpace(_formula[_formula.Length - 1]) && _formula[_formula.Length - 1] != '(')
        {
            _formula.Append(' ');
        }
        _formula.Append($"${fieldName}");
    }
    public static ConditionBuilder WithField(string fieldName) => new FilterBuilder().Field(fieldName);

    /// <summary>
    /// Starts and completes a new condition for a specific field in a single step.
    /// This prevents errors where a condition is started but not completed.
    /// Example: builder.Field("Age", c => c.IsGreaterThan(30))
    /// </summary>
    /// <param name="fieldName">The name of the field to filter on.</param>
    /// <param name="conditionAction">An action that uses the ConditionBuilder to define the condition.</param>
    /// <returns>The parent FilterBuilder to continue chaining.</returns>
    public FilterBuilder Field(string fieldName, Action<ConditionBuilder> conditionAction)
    {
        AppendFieldName(fieldName);

        // Create a temporary ConditionBuilder just for this operation
        var conditionBuilder = new ConditionBuilder(this, _formula);

        // Execute the user's action (e.g., c => c.IsGreaterThan(30))
        conditionAction(conditionBuilder);

        // The action has already modified the formula via the ConditionBuilder.
        // We can now return the main FilterBuilder.
        return this;
    }

    public static FilterBuilder WithMasterId(Action<ConditionBuilder> conditionAction) => new FilterBuilder().Field("MASTERID", conditionAction);

    public static FilterBuilder WithAlterId(Action<ConditionBuilder> conditionAction) => new FilterBuilder().Field("ALTERID", conditionAction);

    public static FilterBuilder WithGUID(Action<ConditionBuilder> conditionAction) => new FilterBuilder().Field("GUID", conditionAction);


    /// <summary>
    /// Appends an "AND" logical operator to the formula.
    /// </summary>
    public FilterBuilder And(FilterBuilder builder)
    {
        _formula.Append(" AND ");
        _formula.Append(builder);
        return this;
    }

    /// <summary>
    /// Appends an "OR" logical operator to the formula.
    /// </summary>
    public FilterBuilder Or(FilterBuilder builder)
    {
        _formula.Append(" OR ");
        _formula.Append(builder);
        return this;
    }

    /// <summary>
    /// Creates a logical group using parentheses.
    /// The conditions inside the group are built using the provided action.
    /// Example: builder.Group(g => g.Field("A").IsEqualTo(1).Or().Field("B").IsEqualTo(2))
    /// Result: "(A = 1 OR B = 2)"
    /// </summary>
    /// <param name="groupBuilderAction">An action that defines the sub-filter within the group.</param>
    public FilterBuilder Group(Action<FilterBuilder> groupBuilderAction)
    {
        // We add a space if the formula isn't empty and doesn't end with an opening parenthesis
        if (_formula.Length > 0 && _formula[^1] != '(')
        {
            _formula.Append(' ');
        }

        var groupBuilder = new FilterBuilder();
        groupBuilder._formula.Append('(');
        groupBuilderAction(groupBuilder);
        groupBuilder._formula.Append(')');

        _formula.Append(groupBuilder.Build());
        return this;
    }


    /// <summary>
    /// Builds the final formula string.
    /// </summary>
    /// <returns>The complete filter formula string.</returns>
    public string Build()
    {
        // Trim any trailing spaces or operators
        return _formula.ToString().Trim();
    }

    public override string ToString()
    {
        return Build();
    }

    /// <summary>
    /// Implicitly converts the FilterBuilder to its string representation.
    /// This allows you to use the builder directly where a string is expected.
    /// </summary>
    /// <param name="builder">The FilterBuilder instance to convert.</param>
    public static implicit operator string(FilterBuilder builder)
    {
        // Handle null case gracefully
        if (builder == null)
        {
            return string.Empty;
        }
        return builder.Build();
    }
}
/// <summary>
/// A helper class to build a specific condition (Operator + Value) for a field.
/// This class is returned by the FilterBuilder.Field() method.
/// </summary>
public class ConditionBuilder
{
    private readonly FilterBuilder _parentBuilder;
    private readonly StringBuilder _formula;

    public ConditionBuilder(FilterBuilder parentBuilder, StringBuilder formula)
    {
        _parentBuilder = parentBuilder;
        _formula = formula;
    }

    public void IsEqualTo(object value) => AppendCondition("=", value);
    public void IsNotEqualTo(object value) => AppendCondition("<>", value);
    public void IsGreaterThan(object value) => AppendCondition(">", value);
    public void IsGreaterThanOrEqualTo(object value) => AppendCondition(">=", value);
    public void IsLessThan(object value) => AppendCondition("<", value);
    public void IsLessThanOrEqualTo(object value) => AppendCondition("<=", value);

    private FilterBuilder AppendCondition(string op, object value)
    {
        _formula.Append($" {op} {FormatValue(value)}");
        return _parentBuilder;
    }

    /// <summary>
    /// Formats the value for inclusion in the formula string.
    /// Strings are wrapped in single quotes, other types are converted to string.
    /// </summary>
    private string FormatValue(object value)
    {
        if (value == null)
        {
            return "NULL";
        }

        if (value is string || value is char || value is Guid)
        {
            // Escape single quotes within the string
            return $"'{value.ToString()!
                .Replace("'", "''")}'";
        }

        if (value is bool boolValue)
        {
            // C# bools are "True"/"False"
            return boolValue ? "1" : "0";
        }

        // For numbers and other types, just use ToString()
        return value.ToString();
    }
}

