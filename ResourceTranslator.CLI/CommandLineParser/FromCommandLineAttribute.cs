using System;

namespace ResourceTranslator.CLI.CommandLineParser
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class FromCommandLineAttribute: Attribute
    {
        public FromCommandLineAttribute(params string[] paramNames)
        {
            ParamNames = paramNames;
        }

        public string Help { get; set; }
        public bool Required { get; set; }

        public string[] ParamNames { get; set; }
    }
}