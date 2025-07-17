using System.Linq.Expressions;
using TallyConnector.Abstractions.Models;

namespace TallyConnector.Core.Models.Request;
public class BaseRequestOptions
{
    public string? Company { get; set; }

    public BaseRequestOptions SetCompany(string Company)
    {
        this.Company = Company;
        return this;
    }
}
public class PostRequestOptions : BaseRequestOptions
{

}

public class DateFilterRequestOptions : BaseRequestOptions
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }

    public new DateFilterRequestOptions SetCompany(string Company)
    {
        this.Company = Company;
        return this;
    }

    public DateFilterRequestOptions From(DateTime fromDate)
    {
        this.FromDate = fromDate;
        return this;
    }

    public DateFilterRequestOptions To(DateTime toDate)
    {
        this.ToDate = toDate;
        return this;
    }


}
public class AutoColumnReportPeriodRequestOptions : DateFilterRequestOptions
{
    public PeriodicityType Periodicity { get; set; }
}


public class RequestOptions : DateFilterRequestOptions
{
    public List<Filter>? Filters { get; set; }
    public List<string>? Compute { get; set; } = [];
    public List<string>? ComputeVar { get; set; } = [];
    public string? Childof { get; set; }
    public string? CollectionType { get; set; }
    public YesNo? BelongsTo { get; set; }

    public new RequestOptions SetCompany(string Company)
    {
        this.Company = Company;
        return this;
    }
}
public class RequestOptionsBuilder<T, TMeta> where T : ITallyRequestableObject where TMeta : MetaObject
{
    FilterExpressionParser _parser = new();
    public RequestOptionsBuilder<T, TMeta> Where(Expression<Func<TMeta, bool>> expression)
    {

        return this;
    }
    public RequestOptionsBuilder<T, TMeta> Where(Expression<Func<TMeta, PropertyMetaData<bool>>> expression)
    {

        return this;
    }
}

public class FilterExpressionParser : ExpressionVisitor
{
    private readonly Stack<string> _path = new();

    public FilterNode Parse<TMeta>(Expression<Func<TMeta, bool>> expr)
    {
        // Start visiting the body (should be BinaryExpression or MethodCallExpression)
        var visited = Visit(expr.Body) as ConstantExpression;

        if (visited == null || visited.Value == null)
            throw new InvalidOperationException("Expected a ConstantExpression wrapping FilterNode.");

        return (FilterNode)visited.Value;
    }

    protected override Expression VisitBinary(BinaryExpression node)
    {
        if (node.NodeType == ExpressionType.AndAlso || node.NodeType == ExpressionType.OrElse)
        {
            var op = node.NodeType == ExpressionType.AndAlso ? "&&" : "||";

            var leftExpr = Visit(node.Left) as ConstantExpression;
            var rightExpr = Visit(node.Right) as ConstantExpression;

            if (leftExpr?.Value is not FilterNode leftNode)
                throw new InvalidOperationException("Invalid left node in logical expression.");

            if (rightExpr?.Value is not FilterNode rightNode)
                throw new InvalidOperationException("Invalid right node in logical expression.");

            return Expression.Constant(new LogicalFilterNode(op, leftNode, rightNode));
        }

        if (node.NodeType == ExpressionType.Equal)
        {
            Visit(node.Left); // Push property path
            var propPath = _path.Pop();

            var rightExpr = Visit(node.Right) as ConstantExpression;

            return Expression.Constant(new BinaryFilterNode("==", propPath, rightExpr?.Value));
        }

        throw new NotSupportedException($"Unsupported binary operation: {node.NodeType}");
    }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        if (node.Method.Name == "Contains")
        {
            Visit(node.Object);
            var propPath = _path.Pop();

            var argExpr = Visit(node.Arguments[0]) as ConstantExpression;

            return Expression.Constant(new MethodFilterNode("Contains", propPath, argExpr?.Value));
        }

        throw new NotSupportedException($"Unsupported method call: {node.Method.Name}");
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        if (node.Expression is MemberExpression parent)
        {
            Visit(parent);
            var parentPath = _path.Pop();
            _path.Push($"{parentPath}.{node.Member.Name}");
        }
        else
        {
            _path.Push(node.Member.Name);
        }

        return Expression.Constant(null); // Dummy return, path is pushed to stack
    }

    protected override Expression VisitConstant(ConstantExpression node) => node;
}
public class PaginatedRequestOptions : RequestOptions
{
    public int PageNum { get; set; } = 1;
    public int? RecordsPerPage { get; set; }

    /// <summary>
    /// If set to true, Count request is not send  receiving data from tally
    /// </summary>
    public bool DisableCountTag { get; set; }
}
public class MasterRequestOptions : RequestOptions
{
    public MasterLookupField LookupField { get; set; } = MasterLookupField.Name;
}


public class CollectionRequestOptions : PaginatedRequestOptions
{
    public CollectionRequestOptions()
    {
    }

    public CollectionRequestOptions(string collectionType)
    {
        CollectionType = collectionType;
    }

    public bool Pagination { get; set; }
    public string? ChildOf { get; set; }

}

