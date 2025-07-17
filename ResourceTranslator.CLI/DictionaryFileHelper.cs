using Nextended.Core.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json.Linq;
using YamlDotNet.Serialization;

namespace ResourceTranslator.CLI
{
    public class DictionaryFileHelper
    {
        public static IDictionary<string, string> LoadDictionaryFromFile(string filename)
        {
            return LoadDictionaryFromFile(filename, SupportedFormats.FileFormat(filename));
        }

        public static IDictionary<string, string> LoadDictionaryFromFile(string filename, FileFormatType fileFormat)
        {
            return fileFormat switch
            {
                FileFormatType.TypeScriptObject => CreateFromTypeScriptObject(filename),
                FileFormatType.Json => CreateFromJson(filename),
                FileFormatType.Yaml => CreateFromYaml(filename),
                FileFormatType.Xml => CreateFromXml(filename),
                _ => throw new ArgumentOutOfRangeException(nameof(fileFormat), fileFormat, null)
            };
        }

        private static IDictionary<string, string> CreateFromTypeScriptObject(string filename)
        {
            var content = File.ReadAllText(filename);
            var startIndex = content.IndexOf('{'); // Object begins (maybe)
            var result = File.ReadAllLines(filename).Select(s => s.Split(":")).Where(parts => parts.Length == 2).ToDictionary(parts => parts[0], parts => parts[1]);
            return result;
        }

        public static Task SaveDictionaryToFile(IDictionary<string, string> dictionary, string fileName,
            FileFormatType fileFormat, Encoding encoding)
        {
            return fileFormat switch
            {
                FileFormatType.Json => SaveAsJson(dictionary, fileName, encoding),
                FileFormatType.Yaml => SaveAsYaml(dictionary, fileName, encoding),
                FileFormatType.Xml => SaveAsXml(dictionary, fileName, encoding),
                _ => throw new ArgumentOutOfRangeException(nameof(fileFormat), fileFormat, null)
            };
        }

        private static Task SaveAsXml(IDictionary<string, string> dictionary, string fileName, Encoding encoding)
        {
            //XElement el = new XElement("root",
            //    dictionary.Select(kv => new XElement(kv.Key, kv.Value)));
            //el.Save(fileName);
            //return Task.CompletedTask;
            throw new NotSupportedException("Sorry writing resx or xml is currently not supported and will be added later. Use Json or Yaml format instead");
        }

        private static Task SaveAsYaml(IDictionary<string, string> dictionary, string fileName, Encoding encoding)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);
            var serializer = new SerializerBuilder().Build();
            var yaml = serializer.Serialize(dictionary);
            File.WriteAllText(fileName, yaml, encoding);
            return Task.CompletedTask;
        }

        private static Task SaveAsJson(IDictionary<string, string> dictionary, string fileName, Encoding encoding)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);
            var dict = JsonDictionaryConverter.ConvertToUnflattenDictionary(dictionary);
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };
            
            string json = JsonSerializer.Serialize(dict, jsonSerializerOptions);
            File.WriteAllText(fileName, json, encoding);
            return Task.CompletedTask;
        }


        private static IDictionary<string, string> CreateFromXml(string filename)
        {
            var result = new Dictionary<string, string>();
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);

            foreach (XmlNode node in doc.SelectNodes("//root/data"))
            {
                var value = node.SelectSingleNode("value").InnerText.Replace('"', '\'');
                var comment = node.SelectSingleNode("comment")?.InnerText?.Replace('"', '\'') ?? "";
                var key = node.Attributes["name"].Value;
                result.Add(key, value);
            }

            return result;
        }

        private static IDictionary<string, string> CreateFromYaml(string filename)
        {
            var serializer = new YamlDotNet.Serialization.Deserializer();
            return serializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(filename));
        }

        private static IDictionary<string, string> CreateFromJson(string filename)
        {
            JObject jsonObject = JObject.Parse(File.ReadAllText(filename));
            var dict = JsonDictionaryConverter.Flatten(jsonObject);
            return dict;
            //return JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(filename));
        }


    }
}