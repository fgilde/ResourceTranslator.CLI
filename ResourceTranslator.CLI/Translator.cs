using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Nextended.Core.Extensions;
using TranslatorService;
using TranslatorService.Models.Translation;

namespace ResourceTranslator.CLI
{
    public class Translator
    {
        private readonly Options _options;
        private FileFormatType usedFormat;
        private readonly IDictionary<string, string> inputDictionary;
        private TranslatorClient client;
        private FileOutputInfo outputInfo;
        private Encoding encoding;
        private string content; 

        public Translator(Options options)
        {
            _options = options;
            usedFormat = SupportedFormats.FileFormat(options.FileName);
            if (usedFormat != FileFormatType.Text)
            {
                inputDictionary = DictionaryFileHelper.LoadDictionaryFromFile(options.FileName, usedFormat);
            }
            else
            {
                content = File.ReadAllText(options.FileName);
            }            
            if(options.Encoding == "auto")
                encoding = EncodingHelper.GetEncoding(options.FileName) ?? Encoding.UTF8;
            else
                encoding = FindEncoding(options.Encoding);
            outputInfo = FileOutputInfo.CreateFileOutputInfos(_options.FileName);
        }

        private Encoding FindEncoding(string encoding = "utf-8")
        {
            try
            {
                return Encoding.GetEncoding(encoding);
            }
            catch (ArgumentException)
            {
                return Encoding.UTF8;
            }
        }

        public async Task ExecuteAsync()
        {
            client = CreateClient();
            if (usedFormat == FileFormatType.Text)
            {
                await ExecuteTextAsync();
                return;
            }
            var targets = GetNeededTargets();
            if (targets.Any())
            {
                var outputDictionaries = targets.ToDictionary(
                    target => target,
                    target => OutputDictionary(OutputFileNameForTargetCulture(target)) ?? new Dictionary<string, string>());

                // Translate only what is actually missing, not the whole file. One target lacking a
                // single key used to re-translate every entry for every needed target, which is the
                // difference between a handful of billed strings per build and thousands.
                // Ordering matters: the API result is correlated back to this list by index.
                var pending = inputDictionary
                    .Where(pair => !IsIgnoredKey(pair.Key))
                    .Where(pair => _options.OverwriteExistingValuesWithNewTranslations
                                   || outputDictionaries.Values.Any(output => !output.ContainsKey(pair.Key)))
                    .ToList();

                if (pending.Any())
                {
                    var result = await TranslateAsync(pending.Select(pair => pair.Value), targets);
                    for (var cultureIndex = 0; cultureIndex < targets.Length; cultureIndex++)
                    {
                        var resultDictionary = outputDictionaries[targets[cultureIndex]];
                        for (var keyIndex = 0; keyIndex < pending.Count; keyIndex++)
                        {
                            var pair = pending[keyIndex];
                            if (resultDictionary.ContainsKey(pair.Key) && !_options.OverwriteExistingValuesWithNewTranslations)
                                continue;

                            var translated = result[keyIndex]?.Translations?.ToList()[cultureIndex]?.Text;
                            if (!string.IsNullOrWhiteSpace(translated))
                            {
                                resultDictionary[pair.Key] = translated;
                            }
                        }
                    }
                }

                foreach (var target in targets)
                {
                    var resultDictionary = outputDictionaries[target];
                    ApplyIgnoredValues(resultDictionary);
                    await DictionaryFileHelper.SaveDictionaryToFile(resultDictionary, OutputFileNameForTargetCulture(target), GetResultFormat(), encoding);
                }
            }
            else
            {
                Console.WriteLine("No translation needed.Skipping translate");
            }
            await SortAllDictionaries();
        }

        private async Task ExecuteTextAsync()
        {
            if (_options.Target.Any())
            {
                foreach (var targets in _options.Target.Where(t => !_options.SkipExistingOutputs || !File.Exists(OutputFileNameForTargetCulture(t))).ChunkBy(6)) // Max request by culture limit (API Requirement)
                {
                    var outputFiles = targets.Select(OutputFileNameForTargetCulture).ToArray();
                    outputFiles.Where(File.Exists).Apply(File.Delete); // Because we are chunking and appending we need to delete files first
                    foreach (var chars in content.ChunkBy(5000)) // Max 5000 chars (API requirement)
                    {
                        var text = new string(chars.ToArray());
                        TranslationResponse res = await client.TranslateAsync(text, targets);
                        await Task.WhenAll(res.Translations.Select((translation, i) => File.AppendAllTextAsync(outputFiles[i], translation.Text)));
                    }
                }
            }
        }

        /// <summary>
        /// True for keys listed in -ignorekeys, whose value is an identifier rather than prose.
        /// Sending such a value to the translator corrupts it whenever it happens to read as a word in
        /// the target language: a resource file's own "culture": "en" comes back as "في" (ar), "v" (cs,
        /// sl), "i" (sv), "içinde" (tr) or "trong" (vi), while staying "en" for every other target —
        /// which is why it only ever looked intermittently broken.
        /// </summary>
        private bool IsIgnoredKey(string key)
        {
            return _options.IgnoredKeys.Any(ignored => MatchesKey(key, ignored));
        }

        /// <summary>
        /// Exact match on the whole dotted path, so "culture" ignores a root-level culture without also
        /// ignoring "parent.child.culture". A trailing * switches to prefix matching for whole subtrees.
        /// Comparison is case-insensitive, so a file spelling the key "Culture" is covered too.
        /// </summary>
        private static bool MatchesKey(string key, string configured)
        {
            if (configured.EndsWith("*", StringComparison.Ordinal))
                return key.StartsWith(configured[..^1], StringComparison.OrdinalIgnoreCase);

            return string.Equals(key, configured, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Copies the entries held back from translation into the output, without clobbering a value the
        /// output already has — an existing culture identifier there is the target's own and more correct
        /// than the source's.
        /// </summary>
        private void ApplyIgnoredValues(IDictionary<string, string> resultDictionary)
        {
            foreach (var pair in inputDictionary.Where(pair => IsIgnoredKey(pair.Key) && !resultDictionary.ContainsKey(pair.Key)))
            {
                resultDictionary[pair.Key] = pair.Value;
            }
        }

        private async Task SortAllDictionaries()
        {
            if (!_options.AutoSort) return;

            await DictionaryFileHelper.SaveDictionaryToFile(new SortedDictionary<string, string>(inputDictionary), _options.FileName, usedFormat, encoding);
            await Task.WhenAll(_options.Target.Select(target =>
            {
                return Task.Run(() =>
                {
                    var file = OutputFileNameForTargetCulture(target);
                    var targetDictionary = OutputDictionary(file);
                    if (targetDictionary != null && targetDictionary.Any())
                    {
                        targetDictionary = new SortedDictionary<string, string>(targetDictionary);
                        DictionaryFileHelper.SaveDictionaryToFile(targetDictionary, file, GetResultFormat(), encoding);
                    }
                });
            }));

        }

        private string[] GetNeededTargets()
        {
            if (_options.OverwriteExistingValuesWithNewTranslations)
                return _options.Target;

            // Deliberately no count comparison: a target carrying an extra key the source no longer has
            // (a key that was renamed or removed after being translated once — nothing prunes those)
            // can never match the source count, which pinned that culture to "needs translation" on
            // every single run, forever. Only genuinely missing source keys make a target needed.
            return (from target in _options.Target let file = OutputFileNameForTargetCulture(target)
                let targetDict = OutputDictionary(file)
                where targetDict == null || !inputDictionary.Keys.All(targetDict.ContainsKey) select target)
                .ToArray();
        }

        private FileFormatType GetResultFormat()
        {
            return string.IsNullOrEmpty(_options.OutputFormat) ? usedFormat : _options.OutputFormat.MapTo<FileFormatType>();
        }

        private IDictionary<string, string> OutputDictionary(string file)
        {
            return File.Exists(file)
                ? DictionaryFileHelper.LoadDictionaryFromFile(file, SupportedFormats.FileFormat(file))
                : new Dictionary<string, string>();
        }

        private async Task<IList<TranslationResponse>> TranslateAsync(IEnumerable<string> inputs, string[] target)
        {
            var result = new List<TranslationResponse>();
            foreach (var list in inputs.ChunkBy(25))
            {
                var toAdd = await client.TranslateAsync(list, target);
                result.AddRange(toAdd);
            }

            return result;
        }


        private string OutputFileNameForTargetCulture(string targetCulture)
        {
            var fileName = outputInfo.Clone().SetProperties(i => i.Culture = targetCulture).ToString(_options.FileOutputFormat);
            if (!Directory.Exists(_options.OutputDir))
                Directory.CreateDirectory(_options.OutputDir);
            return Path.Combine(_options.OutputDir, fileName);
        }


        private TranslatorClient CreateClient()
        {
            return new TranslatorClient(_options.ApiKey, _options.Region);
        }

    }
}