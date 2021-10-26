using System;
using System.IO;

namespace ResourceTranslator.CLI
{
    public static class SupportedFormats
    {
        public static FileFormatType FileFormat(string fileName)
        {
            var lower = Path.GetExtension(fileName).ToLower();
            return lower switch
            {
                ".json" => FileFormatType.Json,
                ".yml" => FileFormatType.Yaml,
                ".yaml" => FileFormatType.Yaml,
                ".xml" => FileFormatType.Xml,
                ".resx" => FileFormatType.Xml,
                _ => throw new NotSupportedException($"Type of {fileName} is not supported")
            };
        }
    }

    public enum FileFormatType
    {
        Json,
        Yaml,
        Xml
    }
}