namespace TallyConnector.XmlTests;

/// <summary>
/// Base class for model-specific XML tests providing common resource loading functionality.
/// </summary>
public abstract class XmlTestBase
{
    /// <summary>
    /// Override this to specify the resource path relative to Resources folder.
    /// Example: "TallyPrime/V6/Voucher"
    /// </summary>
    protected abstract string ResourceSubPath { get; }

    protected string GetResourcePath(string filename)
    {
        var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", ResourceSubPath, filename);
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Resource file not found at {path}");
        }
        return path;
    }

    protected string ReadResourceXml(string filename)
    {
        return File.ReadAllText(GetResourcePath(filename));
    }
}
