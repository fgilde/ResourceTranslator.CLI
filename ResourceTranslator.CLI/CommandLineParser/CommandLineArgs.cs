using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Nextended.Core;
using Nextended.Core.Extensions;

namespace ResourceTranslator.CLI.CommandLineParser
{
    public static class CommandLineArgs
    {
        private const string _optionsFileNameParam = "optionsfile";

        private static readonly string[] _helpParams = { "-h", "--help", "-help", "-?", "/?" };

        public static T Parse<T>(string[] args) where T : new()
        {
            return Parse<T>(string.Join(" ", args));
        }

        public static bool IsHelpRequested(string[] args)
        {
            return args == null
                   || args.Length == 0
                   || args.Any(a => _helpParams.Contains(a, StringComparer.OrdinalIgnoreCase));
        }

        public static string BuildUsage<T>(string toolName) where T : new()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Usage: {toolName} [options]");
            sb.AppendLine();
            sb.AppendLine("Options:");

            foreach (var p in CollectProperties<T>())
            {
                var names = string.Join(", ", NamesForProperty<T>(p));
                var required = p.Attributes?.Required == true ? " (required)" : "";
                sb.AppendLine($"  {names}{required}");
                if (!string.IsNullOrWhiteSpace(p.Attributes?.Help))
                    sb.AppendLine($"      {p.Attributes.Help}");
            }

            sb.AppendLine($"  --{_optionsFileNameParam}");
            sb.AppendLine("      Path to a JSON file containing the options. CLI flags override its values.");
            sb.AppendLine();
            sb.AppendLine("  -h, --help");
            sb.AppendLine("      Show this help and exit.");

            return sb.ToString();
        }

        public static T Parse<T>(string args) where T : new()
        {
            T res = CreateTarget<T>(args);
            
            foreach (var p in CollectProperties<T>())
            {
                foreach (var name in NamesForProperty<T>(p))
                {
                    var value = ValueFromArg(args, name);
                    if (value != null)
                        p.Property.SetValue(res, value.MapTo(p.Property.PropertyType));
                }

                ThrowIfRequiredMissing(p, res);
            }
            return res;
        }

        private static void ThrowIfRequiredMissing<T>((PropertyInfo Property, FromCommandLineAttribute Attributes) p, T res) where T : new()
        {
            if (p.Attributes?.Required == true && p.Property.GetValue(res) == null)
                throw new Exception($"{p.Property.Name} is required, please specify {p.Property.Name} like one of these \r\n{string.Join(Environment.NewLine, NamesForProperty<T>(p).Select(n => $" {n} \"<value>\""))} ");
        }

        private static string[] NamesForProperty<T>((PropertyInfo Property, FromCommandLineAttribute Attributes) prop) where T : new()
        {
            return (prop.Attributes?.ParamNames.Select(s => s.EnsureStartsWith("-").ToLower()) ?? Enumerable.Empty<string>()).Concat(new [] {"-"+prop.Property.Name.ToLower()}).Distinct().ToArray();
        }

        private static IEnumerable<(PropertyInfo Property, FromCommandLineAttribute Attributes)> CollectProperties<T>() where T : new()
        {
            // Only writable properties can be bound from the command line. Read-only computed
            // properties (e.g. Options.Target) would otherwise throw "Property set method not found."
            // when their auto-derived name collides with a real flag.
            return typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite)
                .Select(p => (p, p.GetCustomAttribute<FromCommandLineAttribute>()) );
        }

        private static T CreateTarget<T>(string args) where T : new()
        {
            T res = new T();
            if (!args.Contains($"--{_optionsFileNameParam}")) 
                return res;

            string file = ValueFromArg(args, $"-{_optionsFileNameParam}");
            if (File.Exists(file)) 
                res = Check.TryCatch<T, Exception>(() => JsonSerializer.Deserialize<T>(File.ReadAllText(file)));

            return res ?? new T();
        }

        private static string ValueFromArg(string args, string name)
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