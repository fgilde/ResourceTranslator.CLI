using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ResourceTranslator.CLI.Helper;

internal static class InternalJsonDictionaryConverter
{
    // Public API
    public static IDictionary<string, string> Flatten(JObject obj)
    {
        var dict = new Dictionary<string, string>(StringComparer.Ordinal);
        FlattenInternal(obj, prefix: null, dict);
        return dict;
    }

    public static JObject ConvertToUnflattenDictionary(IDictionary<string, string> flat)
    {
        var root = new JObject();

        foreach (var kv in flat)
        {
            var segments = ParsePath(kv.Key); // dot-getrennte, entschlüsselte Segmente + Arrayteile
            InsertValue(root, segments, kv.Value);
        }

        return root;
    }

    // ---- Flatten ----
    private static void FlattenInternal(JToken token, string? prefix, IDictionary<string, string> dict)
    {
        switch (token)
        {
            case JObject o:
                foreach (var prop in o.Properties())
                {
                    var seg = Escape(prop.Name);
                    var path = string.IsNullOrEmpty(prefix) ? seg : $"{prefix}.{seg}";
                    FlattenInternal(prop.Value, path, dict);
                }
                break;

            case JArray arr:
                for (int i = 0; i < arr.Count; i++)
                {
                    var path = $"{prefix}[{i}]";
                    FlattenInternal(arr[i], path, dict);
                }
                break;

            default: // JValue oder sonstiges
                dict[prefix ?? ""] = token.Type == JTokenType.Null ? "" : token.ToString();
                break;
        }
    }

    // ---- Unflatten helpers ----

    // Escaping: "." -> "\." ; "\" -> "\\"
    private static string Escape(string s)
    {
        return s.Replace("\\", "\\\\").Replace(".", "\\.");
    }

    private static string Unescape(string s)
    {
        // Schrittweise entschärfen: erst Backslashes, dann die escapeten Dots
        // Wir müssen das stabil machen: temporäres Platzhalterzeichen, das im String nicht vorkommt
        const string TMP = "\u0001"; // unlikely char
        return s.Replace("\\\\", TMP).Replace("\\.", ".").Replace(TMP, "\\");
    }

    // Pfad in Segmente zerlegen:
    // - split on '.' die NICHT mit Backslash escaped sind
    // - jedes Segment kann 0..n Arrayteile wie [12] anhängen
    private static List<PathPart> ParsePath(string path)
    {
        var rawSegments = SplitOnUnescapedDots(path);
        var parts = new List<PathPart>();

        var arrayRegex = new Regex(@"\[(\d+)\]", RegexOptions.Compiled);

        foreach (var raw in rawSegments)
        {
            // Segment in Key + evtl. [idx][idx]... zerlegen
            var m = arrayRegex.Matches(raw);
            var keyOnly = raw;
            if (m.Count > 0)
            {
                // Key steht vor dem ersten '['
                var firstIdx = raw.IndexOf('[');
                keyOnly = firstIdx >= 0 ? raw[..firstIdx] : raw;
            }

            var key = Unescape(keyOnly);
            var part = new PathPart { Key = key };

            foreach (Match mm in m)
            {
                part.ArrayIndices.Add(int.Parse(mm.Groups[1].Value));
            }
            parts.Add(part);
        }

        return parts;
    }

    private static List<string> SplitOnUnescapedDots(string s)
    {
        var result = new List<string>();
        var sb = new StringBuilder();
        bool escape = false;

        for (int i = 0; i < s.Length; i++)
        {
            char c = s[i];

            if (escape)
            {
                sb.Append(c);   // egal welches Zeichen, es ist escaped
                escape = false;
            }
            else if (c == '\\')
            {
                escape = true;  // nächstes Zeichen ist escaped
            }
            else if (c == '.')
            {
                // echter Trenner
                result.Add(sb.ToString());
                sb.Clear();
            }
            else
            {
                sb.Append(c);
            }
        }

        if (escape) sb.Append('\\'); // trailing backslash wörtlich übernehmen
        result.Add(sb.ToString());
        return result;
    }

    private static void InsertValue(JObject root, List<PathPart> parts, string value)
    {
        JToken current = root;

        for (int i = 0; i < parts.Count; i++)
        {
            var part = parts[i];
            bool isLast = (i == parts.Count - 1);

            // Sicherstellen: Objekt-Ebene für Key
            if (current.Type != JTokenType.Object)
            {
                // falls durch Arrays o.ä. kein Objekt: ersetzen
                var replacement = new JObject();
                ReplaceCurrent(current, replacement);
                current = replacement;
            }

            var obj = (JObject)current;

            // Property holen/erzeugen
            if (obj[part.Key] == null)
                obj[part.Key] = new JObject();

            current = obj[part.Key]!;

            // Falls Arrays daran hängen: nacheinander durchgehen/erzeugen
            for (int ix = 0; ix < part.ArrayIndices.Count; ix++)
            {
                int arrIndex = part.ArrayIndices[ix];
                if (current.Type != JTokenType.Array)
                {
                    var newArr = new JArray();
                    ReplaceCurrent(obj[part.Key]!, newArr);
                    current = newArr;
                    obj[part.Key] = newArr;
                }

                var ja = (JArray)current;
                EnsureArraySize(ja, arrIndex + 1);

                if (ix == part.ArrayIndices.Count - 1 && isLast)
                {
                    // Letztes Segment + letzter Arrayindex -> setze Wert
                    ja[arrIndex] = JToken.FromObject(value ?? "");
                }
                else
                {
                    if (ja[arrIndex] == null || ja[arrIndex]!.Type == JTokenType.Null)
                        ja[arrIndex] = new JObject();

                    if (ja[arrIndex]!.Type != JTokenType.Object && ja[arrIndex]!.Type != JTokenType.Array)
                        ja[arrIndex] = new JObject();

                    current = ja[arrIndex]!;
                }
            }

            // Wenn keine Arrayindices und letztes Segment -> Wert setzen
            if (part.ArrayIndices.Count == 0 && isLast)
            {
                obj[part.Key] = JToken.FromObject(value ?? "");
            }
        }
    }

    private static void EnsureArraySize(JArray arr, int size)
    {
        while (arr.Count < size) arr.Add(null);
    }

    private static void ReplaceCurrent(JToken current, JToken replacement)
    {
        if (current.Parent is JProperty prop)
        {
            prop.Value = replacement;
        }
        else if (current.Parent is JArray arr)
        {
            int idx = arr.IndexOf(current);
            arr[idx] = replacement;
        }
        // root hat keinen Parent: ignorieren
    }

    private sealed class PathPart
    {
        public string Key { get; set; } = "";
        public List<int> ArrayIndices { get; } = new();
    }
}
