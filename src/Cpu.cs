using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace sysinfo
{
    static class Cpu
    {
        public static string Value { get; private set; } = string.Empty;

        public static string Name { get; private set; } = nameof(Cpu);

        static Encoding utf8 = new UTF8Encoding(false);
        static long prevtotal = 0, previdle = 0;
        static Regex numbers = new Regex(@"\d+", RegexOptions.Compiled);

        public static void Update()
        {
            if (!File.Exists("/proc/stat"))
            {
                Value = string.Empty;
                return;
            }   
            
            string stat = File.ReadLines("/proc/stat", utf8).First();
            long total = 0, idle = 0;
            int i = 0;
            foreach (Match match in numbers.Matches(stat).Where(x => x.Success))
            {
                long val;
                if (long.TryParse(match.Value, out val))
                {
                    total += val;
                    i++;
                    if (i == 4)
                    {
                        idle = val;
                    }
                    continue;
                }
                Value = string.Empty;
                return;
            }
            decimal diff = Math.Floor((idle - previdle) * 100m / (total - prevtotal) + 0.5m);
            previdle = idle;
            prevtotal = total;

            Value = $"{Util.SmallSpace}{Util.PercentToBarsWhenEnabled(diff)}";
        }
    }
}