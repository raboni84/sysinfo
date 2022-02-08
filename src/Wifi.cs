using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace sysinfo
{
    static class Wifi
    {
        public static string Value { get; private set; } = string.Empty;

        public static string Name { get; private set; } = nameof(Wifi);

        static Encoding utf8 = new UTF8Encoding(false);
        static Regex rexwificard = new Regex(@"([a-z0-9]+:)[ ]+\d+[ ]+(\d+)", RegexOptions.Compiled);

        public static void Update()
        {
            if (!File.Exists("/proc/net/wireless"))
            {
                Value = string.Empty;
                return;
            }

            string stat = File.ReadAllText("/proc/net/wireless", utf8);
            Match match = rexwificard.Match(stat);
            if (!match.Success)
            {
                Value = string.Empty;
                return;
            }

            string card = match.Groups[1].Value;
            decimal wifi;
            if (decimal.TryParse(match.Groups[2].Value, out wifi))
            {
                decimal perc = Math.Floor(wifi * 100m / 70m + 0.5m);
                Value = $"{Util.SmallSpace}{Util.PercentToBarsWhenEnabled(perc)}";
            }
            else
            {
                Value = string.Empty;
            }
        }
    }
}