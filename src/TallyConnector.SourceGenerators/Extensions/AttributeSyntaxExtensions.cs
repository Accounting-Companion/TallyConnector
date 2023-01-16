namespace TallyConnector.SourceGenerators.Extensions;
public static class AttributeSyntaxExtensions
{
    public static string? GetAttributeName(this AttributeSyntax attributeSyntax)
    {
        if (attributeSyntax.Name is IdentifierNameSyntax nameSyntax)
        {
            return nameSyntax.Identifier.ValueText;
        }
        return null;
    }
    public static string? GetGenericAttributeName(this AttributeSyntax attributeSyntax)
    {
        if (attributeSyntax.Name is IdentifierNameSyntax nameSyntax)
        {
            return nameSyntax.Identifier.ValueText;
        }
        return null;
    }
}
