using UnitTests._Verifiers;

namespace TC.TDLReportSourceGenerator.Tests.Verifiers;
//https://www.youtube.com/watch?v=BfYxZ4mfv0E
// https://github.com/Flash0ver/F0-Talks-SourceGenerators
internal static partial class CSharpSourceGeneratorVerifier<TSourceGenerator>
    where TSourceGenerator : IIncrementalGenerator, new()
{
    public sealed class Test : CSharpSourceGeneratorTest<EmptySourceGeneratorProvider, DefaultVerifier>
    {
        public Test()
        {
        }

        public LanguageVersion LanguageVersion { get; set; } = LanguageVersion.Default;

        protected override IEnumerable<Type> GetSourceGenerators()
        {
            return [typeof(TSourceGenerator)];
        }
        //protected override IEnumerable<ISourceGenerator> GetSourceGenerators()
        //{
        //    return [new TSourceGenerator().AsSourceGenerator()];
        //}
        protected override Task RunImplAsync(CancellationToken cancellationToken)
        {
            TestState.MarkupHandling = MarkupMode.None;
            return base.RunImplAsync(cancellationToken);
        }
        protected override CompilationOptions CreateCompilationOptions()
        {
            CompilationOptions compilationOptions = base.CreateCompilationOptions();
            return compilationOptions.WithSpecificDiagnosticOptions(
                 compilationOptions.SpecificDiagnosticOptions.SetItems(CSharpVerifierHelper.NullableWarnings));
        }

        protected override ParseOptions CreateParseOptions()
        {
            return ((CSharpParseOptions)base.CreateParseOptions()).WithLanguageVersion(LanguageVersion);
        }
    }
}
