using System.Linq.Expressions;
using TallyConnector.Abstractions.Models;
using TallyConnector.Core.Models;
using static TallyConnector.TDLReportSourceGenerator.Constants;

namespace TallyConnector.Core;
public class QueryBuilder<TEntity, TMeta> where TEntity : BaseObject, IMetaGenerated where TMeta : MetaObject
{
    private readonly TMeta _meta;
    private readonly List<FilterCondition> _metaFilterConditions = new();
    public QueryBuilder(TMeta meta)
    {
        _meta = meta;
    }
    public QueryBuilder<TEntity, TMeta> FilterBy(Func<TMeta, FilterCondition> builder, string? name = null)
    {
        _metaFilterConditions.Add(builder(_meta));
        return this;
    }
}

