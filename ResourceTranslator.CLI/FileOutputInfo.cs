using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ResourceTranslator.CLI
{
    public class FileOutputInfo
    {
        public string Extension { get; set; }
        public string Culture { get; set; }
        public string FileName { get; set; }
        public string OriginalFileName { get; set; }

        public string ToString(string format)
        {
            var result = format;
            var props = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            result = props.Aggregate(result, (current, prop) => current.Replace("{" + prop.Name + "}", prop.GetValue(this)?.ToString() ?? string.Empty));
            if (result.StartsWith("."))
                result = result.Substring(1);
            return result.Replace("..", ".");
        }

        internal static FileOutputInfo CreateFileOutputInfos(string fileName)
        {
            var ext = Path.GetExtension(fileName);
            var fileNameWithoutExtensions = Path.GetFileNameWithoutExtension(fileName);
            var fileNameWithoutExtensionAndCulture = FileNameWithoutCulture(fileNameWithoutExtensions);
            return new FileOutputInfo()
            {
                Extension = ext,
                FileName = fileNameWithoutExtensionAndCulture,
                OriginalFileName = fileNameWithoutExtensions
            };
        }

        private static string FileNameWithoutCulture(string fileName)
        {
            fileName = CultureInfo.GetCultures(CultureTypes.SpecificCultures).Where(i => !string.IsNullOrEmpty(i.Name)).Aggregate(fileName, (current, cultureInfo) => current.Replace(cultureInfo.Name, string.Empty));
            return CultureInfo.GetCultures(CultureTypes.NeutralCultures).Where(i => !string.IsNullOrEmpty(i.Name)).Aggregate(fileName, (current, cultureInfo) => current.Replace(cultureInfo.Name, string.Empty));
        }
    }
}