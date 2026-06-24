using System.Linq;
using ResourceTranslator.CLI;
using ResourceTranslator.CLI.CommandLineParser;
using Xunit;

namespace ResourceTranslator.CLI.Tests
{
    public class CommandLineArgsTests
    {
        private static readonly string[] BaseArgs =
        {
            "-f", "en.json",
            "-endpoint", "https://api.cognitive.microsofttranslator.com/",
            "-key", "the-key",
            "-region", "westeurope",
        };

        [Fact]
        public void Parse_TargetFlag_SetsTargetCulturesWithoutThrowing()
        {
            var args = BaseArgs.Concat(new[] { "-target", "de,fr" }).ToArray();

            var options = CommandLineArgs.Parse<Options>(args);

            Assert.Equal("de,fr", options.TargetCultures);
            Assert.Equal(new[] { "de", "fr" }, options.Target);
        }

        [Fact]
        public void Parse_TargetCulturesFlag_SetsTargetCultures()
        {
            var args = BaseArgs.Concat(new[] { "-targetcultures", "de;fr" }).ToArray();

            var options = CommandLineArgs.Parse<Options>(args);

            Assert.Equal("de;fr", options.TargetCultures);
            Assert.Equal(new[] { "de", "fr" }, options.Target);
        }

        [Fact]
        public void Parse_ScalarFlags_StillBind()
        {
            var options = CommandLineArgs.Parse<Options>(BaseArgs);

            Assert.Equal("en.json", options.FileName);
            Assert.Equal("https://api.cognitive.microsofttranslator.com/", options.TextTranslationEndpoint);
            Assert.Equal("the-key", options.ApiKey);
            Assert.Equal("westeurope", options.Region);
        }
    }
}
