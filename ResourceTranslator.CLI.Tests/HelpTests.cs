using ResourceTranslator.CLI;
using ResourceTranslator.CLI.CommandLineParser;
using Xunit;

namespace ResourceTranslator.CLI.Tests
{
    public class HelpTests
    {
        [Theory]
        [InlineData("-h")]
        [InlineData("--help")]
        [InlineData("-help")]
        [InlineData("/?")]
        [InlineData("-?")]
        public void IsHelpRequested_DetectsHelpTokens(string token)
        {
            Assert.True(CommandLineArgs.IsHelpRequested(new[] { token }));
        }

        [Fact]
        public void IsHelpRequested_TrueWhenNoArgs()
        {
            Assert.True(CommandLineArgs.IsHelpRequested(new string[0]));
        }

        [Fact]
        public void IsHelpRequested_FalseForNormalArgs()
        {
            Assert.False(CommandLineArgs.IsHelpRequested(new[] { "-f", "en.json" }));
        }

        [Fact]
        public void BuildUsage_ListsFlagsWithAliasesAndRequiredMarker()
        {
            var usage = CommandLineArgs.BuildUsage<Options>("resourceTranslator");

            Assert.Contains("resourceTranslator", usage);
            // documented aliases appear
            Assert.Contains("-f", usage);
            Assert.Contains("-target", usage);
            Assert.Contains("--optionsfile", usage);
            // a required flag is marked as required
            Assert.Contains("required", usage, System.StringComparison.OrdinalIgnoreCase);
        }
    }
}
