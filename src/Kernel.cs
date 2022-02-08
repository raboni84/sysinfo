using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace sysinfo
{
    static class Kernel
    {
        public static string Value { get; private set; } = string.Empty;

        public static string Name { get; private set; } = nameof(Kernel);

        static Encoding utf8 = new UTF8Encoding(false);
        static Regex rexkernel = new Regex(@"(\d+\.\d+\.\d+)-[a-zA-Z0-9]+-\d+", RegexOptions.Compiled);

        public static void Update()
        {
            if (!File.Exists("/proc/version"))
            {
                Value = string.Empty;
                return;
            }

            string stat = File.ReadAllText("/proc/version", utf8);
            Match match = rexkernel.Match(stat);
            if (!match.Success)
            {
                Value = string.Empty;
                return;
            }

            Value = $"{Util.SmallSpace}{match.Groups[1].Value}";
        }
    }
}