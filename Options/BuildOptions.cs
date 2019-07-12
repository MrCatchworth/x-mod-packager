using CommandLine;

namespace XModPackager.Options
{
    [Verb("build", HelpText = "Write mod contents to output directory.")]
    public class BuildOptions
    {
        [Option("loose", Required = false, Default = true, HelpText = "If true, build the mod as loose files rather than a zip file.")]
        public bool Loose {get; set;}
    }
}