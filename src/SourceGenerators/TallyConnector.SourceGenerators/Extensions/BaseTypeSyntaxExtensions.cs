namespace TallyConnector.SourceGenerators.Extensions;
public static class BaseTypeSyntaxExtensions
{
    public static string? GetBaseTypeName(this BaseTypeSyntax baseTypeSyntax)
    {
        if (baseTypeSyntax.Type is IdentifierNameSyntax nameSyntax)
        {
            return nameSyntax.Identifier.ValueText;
        }
        return null;
    }
    public static string? GetGenericBaseTypeName(this BaseTypeSyntax baseTypeSyntax)
    {
        if (baseTypeSyntax.Type is GenericNameSyntax nameSyntax)
        {
            return nameSyntax.Identifier.ValueText;
        }
        return null;
    }
}
