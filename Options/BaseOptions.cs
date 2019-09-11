using CommandLine;

namespace XModPackager.Options
{
    public class BaseOptions
    {
        [Option("verbose", HelpText = "Show debug output.")]
        public bool Verbose { get; set; }
        [Option("quiet", HelpText = "Don't show info output.")]
        public bool Quiet { get; set; }
    }
}