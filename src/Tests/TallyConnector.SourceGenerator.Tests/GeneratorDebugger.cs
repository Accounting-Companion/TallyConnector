using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace GeneratorDebugging
{
    internal static class GeneratorDebugger
    {
        internal static GeneratorDriverRunResult RunDebugging(
            IEnumerable<SyntaxTree> sourceCode,
            IIncrementalGenerator[] generators,
            IEnumerable<AdditionalText>? additionalTexts = null,
            ParseOptions? parseOptions = null,
            IEnumerable<MetadataReference>? references = null,
            AnalyzerConfigOptionsProvider? analyzerCfgProvider = null
            )
        {
            var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

            var inputCompilation = CSharpCompilation.Create("compilationAssemblyName", sourceCode, references, compilationOptions);

            var driver = (GeneratorDriver)CSharpGeneratorDriver.Create(generators);

            if (additionalTexts != null)
                driver = driver.AddAdditionalTexts(ImmutableArray.CreateRange(additionalTexts));

            if (parseOptions != null)
                driver = driver.WithUpdatedParseOptions(parseOptions);

            if (analyzerCfgProvider != null)
                driver = driver.WithUpdatedAnalyzerConfigOptions(analyzerCfgProvider);

            driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out _, out _);
            return driver.GetRunResult();
        }
    }
}