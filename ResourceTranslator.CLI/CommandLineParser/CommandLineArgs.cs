using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Nextended.Core.Extensions;

namespace ResourceTranslator.CLI.CommandLineParser
{
    public static class CommandLineArgs
    {
        private const string _optionsFileNameParam = "optionsfile";

        public static T Parse<T>(string[] args) where T : new()
        {
            return Parse<T>(string.Join(" ", args));
        }

        public static T Parse<T>(string args) where T : new()
        {
            T res = CreateTarget<T>(args);
            
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props)
            {
                var attribute = prop.GetCustomAttribute<FromCommandLineAttribute>();
                var namesforProp = (attribute?.ParamNames.Select(s => s.EnsureStartsWith("-").ToLower()) ?? Enumerable.Empty<string>()).Concat(new [] {"-"+prop.Name.ToLower()}).Distinct().ToArray();
                foreach (var name in namesforProp)
                {
                    var value = GetValue(args, name);
                    if (value != null)
                        prop.SetValue(res, value.MapTo(prop.PropertyType));
                }

                if (attribute?.Required == true && prop.GetValue(res) == null)
                {
                    throw new Exception($"{prop.Name} is required, please specify {prop.Name} like one of these \r\n{string.Join(Environment.NewLine, namesforProp.Select(n => $" {n} \"<value>\""))} ");
                }
            }
            return res;
        }

        private static T CreateTarget<T>(string args) where T : new()
        {
            T res = new T();
            if (args.Contains($"--{_optionsFileNameParam}"))
            {
                string file = GetValue(args, $"-{_optionsFileNameParam}");
                if (File.Exists(file))
                {
                    try
                    {
                        res = JsonSerializer.Deserialize<T>(File.ReadAllText(file));
                    }
                    catch {}
                }
            }

            return res ?? new T();
        }

        private static string GetValue(string args, string name)
        {
            var indexOf = args.IndexOf(name, StringComparison.Ordinal);
            if (indexOf >= 0)
            {
                var rest = args.Substring(indexOf + name.Length);
                var end = rest.IndexOf(" -");
                end = end > 0 ? end : rest.Length;
                var value = rest.Substring(0, end).Trim().Replace("\"", "");
                return value;
            }

            return null;
        }

        private static string EnsureStartsWith(this string str, string toStartWith)
        {
            if (!str.StartsWith(toStartWith))
                str = toStartWith + str;
            return str;
        }
    }
}