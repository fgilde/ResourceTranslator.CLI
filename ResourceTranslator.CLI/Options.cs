using System.Linq;
using ResourceTranslator.CLI.CommandLineParser;

namespace ResourceTranslator.CLI
{
    public class Options
    {
        [FromCommandLine("f", nameof(FileName), Required = true)]
        public string FileName { get; set; }

        [FromCommandLine("endpoint", nameof(TextTranslationEndpoint), Required = true)]
        public string TextTranslationEndpoint { get; set; }

        [FromCommandLine("key", nameof(ApiKey), Required = true)]
        public string ApiKey { get; set; }

        [FromCommandLine("of", nameof(FileOutputFormat), Required = true)]
        public string FileOutputFormat { get; set; } = "{FileName}.{Culture}.{Extension}";

        [FromCommandLine("outdir", nameof(OutputDir))]
        public string OutputDir { get; set; }

        [FromCommandLine("target", nameof(TargetCultures))]
        public string TargetCultures { get; set; } = "de-DE,es-ES;it-IT";

        [FromCommandLine("source", nameof(SourceCulture))]
        public string SourceCulture { get; set; }       
        
        [FromCommandLine("region", nameof(Region), Required = true)]
        public string Region { get; set; }

        public string[] Target => TargetCultures.Split(',', ';').Select(v => v.Trim()).ToArray();
        
        [FromCommandLine("format", nameof(OutputFormat) , Help = "Specify format to save result as. Leave empty to use input format. possible options are ("+ nameof(FileFormatType.Xml) + ", " + nameof(FileFormatType.Json) + ", " + nameof(FileFormatType.Yaml) + ") ")]
        public string OutputFormat { get; set; }

        [FromCommandLine("overwritevalues", nameof(OverwriteExistingValuesWithNewTranslations))]
        public bool OverwriteExistingValuesWithNewTranslations { get; set; }

        [FromCommandLine("sort", nameof(AutoSort))]
        public bool AutoSort { get; set; }

        [FromCommandLine("skip", nameof(SkipExistingOutputs))]
        public bool SkipExistingOutputs { get; set; }
    }
}