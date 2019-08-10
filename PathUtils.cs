using System.IO;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace XModPackager
{
    public static class PathUtils
    {
        public static readonly string ContentTemplatePath = "content.template.xml";
        public static readonly string ConfigPath = "xmod.json";

        public static string GetHomeDirectory()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var drive = Environment.GetEnvironmentVariable("HOMEDRIVE");
                var path = Environment.GetEnvironmentVariable("HOMEPATH");

                return Path.Combine(drive, path);
            }
            else
            {
                return Environment.GetEnvironmentVariable("HOME");
            }
        }

        private static readonly IList<string> PossibleSteamKeyPaths = new string[] {
            @"HKEY_LOCAL_MACHINE\SOFTWARE\Valve\Steam",
            @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Valve\Steam"
        };

        public static string GetSteamInstallPath()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                foreach (var possiblePath in PossibleSteamKeyPaths)
                {
                    var registryValue = Registry.GetValue(possiblePath, "InstallPath", null);
                    if (registryValue != null)
                    {
                        return registryValue as string;
                    }
                }

                return null;
            }

            return null;
        }

        public static void CheckOutputPath(string rawOutputPath)
        {
            if (File.Exists(rawOutputPath))
            {
                throw new Exception($"Cannot use output path {rawOutputPath}, it already exists as a file");
            }

            var currentDir = Path.GetFullPath(Directory.GetCurrentDirectory() + "/");
            var fullOutputPath = Path.GetFullPath(rawOutputPath + "/");

            if (currentDir.StartsWith(fullOutputPath))
            {
                throw new Exception(
                    string.Join('\n', new string[] {
                        "Your output directory:",
                        fullOutputPath,
                        "Is the same or a parent of the current directory:",
                        currentDir,
                        "This is too unsafe to allow (running the clean command would delete your project).",
                        "Please check your config file and correct the output path."
                    })
                );
            }
        }
    }
}
