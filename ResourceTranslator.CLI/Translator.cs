﻿using System;
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
                var result = await TranslateAsync(inputDictionary.Values, targets);
                for (var cultureIndex = 0; cultureIndex < targets.Length; cultureIndex++)
                {
                    var targetCulture = targets[cultureIndex];
                    var file = OutputFileNameForTargetCulture(targetCulture);
                    var resultDictionary = OutputDictionary(file) ?? new Dictionary<string, string>();
                    int keyIndex = 0;
                    foreach (var pair in inputDictionary)
                    {
                        if (!resultDictionary.ContainsKey(pair.Key) || _options.OverwriteExistingValuesWithNewTranslations)
                        {
                            var translated = result[keyIndex]?.Translations?.ToList()[cultureIndex]?.Text;
                            if (!string.IsNullOrWhiteSpace(translated))
                            {
                                resultDictionary[pair.Key] = translated;
                            }
                        }

                        keyIndex++;
                    }

                    await DictionaryFileHelper.SaveDictionaryToFile(resultDictionary, file, GetResultFormat(), encoding);
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

            return (from target in _options.Target let file = OutputFileNameForTargetCulture(target) 
                let targetDict = OutputDictionary(file) 
                where targetDict == null || targetDict.Count != inputDictionary.Count || !inputDictionary.Keys.All(targetDict.ContainsKey) select target)
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