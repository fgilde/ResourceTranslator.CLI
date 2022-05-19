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
                //".ts" => FileFormatType.TypeScriptObject,
                //".js" => FileFormatType.JavaScriptObject,
                ".yml" => FileFormatType.Yaml,
                ".yaml" => FileFormatType.Yaml,
                ".xml" => FileFormatType.Xml,
                ".resx" => FileFormatType.Xml,
                _ => FileFormatType.Text
            };
        }
    }

    public enum FileFormatType
    {
        Json,
        Yaml,
        Xml,
        TypeScriptObject,
        JavaScriptObject,
        Text
    }
}