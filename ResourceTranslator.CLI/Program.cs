using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Nextended.Core.Helper;
using ResourceTranslator.CLI.CommandLineParser;

namespace ResourceTranslator.CLI
{
    class Program
    {
        private static Options options;
        static int Main(string[] args)
        {
            try
            {
                options = CommandLineArgs.Parse<Options>(args);
                return (int)Handle();
            }
            catch (Exception e)
            {
                return (int)Return(ExitCode.UnknownError, e.Message);
            }
        }

        static ExitCode Handle()
        {
            if (!File.Exists(options.FileName))
                return Return(ExitCode.InvalidFilename, $"Error Filename {options.FileName} is invalid or not existing");
            if (options.Target == null || options.Target.Length <= 0)
                return Return(ExitCode.InvalidFilename, $"Error Target cultures {options.TargetCultures} are not set correctly");
            options.OutputDir ??= Path.GetDirectoryName(options.FileName);

            var translator = new Translator(options);
            Console.WriteLine("Begin translation...");
            translator.ExecuteAsync().Wait();

            return Return(ExitCode.Success, "");
        }

        static void PrintAppliedOptions()
        {
            Console.WriteLine("Following options are used");
            Console.WriteLine();
            string jsonString = JsonSerializer.Serialize(options, new JsonSerializerOptions() { WriteIndented = true });
            Console.WriteLine(jsonString);
        }

        static ExitCode Return(ExitCode code, string message)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = code == ExitCode.Success ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine(message);
            if (code != ExitCode.Success)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("Notice: Only " + string.Join(",", Enum<FileFormatType>.GetValues().Select(Enum<FileFormatType>.DescriptionFor)) + " formats in flat dictionary structures are supported currently");
            }

            Console.ForegroundColor = color;

            PrintAppliedOptions();

            return code;
        }

    }

    enum ExitCode : int
    {
        Success = 0,
        InvalidFilename = 1,
        UnknownError = 2
    }
}
