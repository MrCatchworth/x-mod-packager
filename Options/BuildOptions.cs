using CommandLine;
using XModPackager.Build;

namespace XModPackager.Options
{
    [Verb("build", HelpText = "Write mod contents to output directory.")]
    public class BuildOptions
    {
        [Option("method", HelpText = "Which build method to use.")]
        public BuildMethod? Method {get; set;}


        [Option("workshop", Default = true, HelpText = "Whether or not to build this mod for workshop deployment.")]
        public bool Workshop {get; set;}
    }
}