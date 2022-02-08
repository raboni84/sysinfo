using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace sysinfo
{
    static class Memory
    {
        public static string Value { get; private set; } = string.Empty;

        public static string Name { get; private set; } = nameof(Memory);

        static Encoding utf8 = new UTF8Encoding(false);
        static Regex rexmemtotal = new Regex(@"MemTotal:[ ]+(\d+)", RegexOptions.Compiled);
        static Regex rexmemavail = new Regex(@"MemAvailable:[ ]+(\d+)", RegexOptions.Compiled);
        static Regex rexswptotal = new Regex(@"SwapTotal:[ ]+(\d+)", RegexOptions.Compiled);
        static Regex rexswpfree = new Regex(@"SwapFree:[ ]+(\d+)", RegexOptions.Compiled);

        public static void Update()
        {
            if (!File.Exists("/proc/meminfo"))
            {
                Value = string.Empty;
                return;
            }

            string stat = File.ReadAllText("/proc/meminfo", utf8);
            long memtotal, memavail, swptotal, swpfree;
            if (long.TryParse(rexmemtotal.Match(stat).Groups[1].Value, out memtotal)
                && long.TryParse(rexmemavail.Match(stat).Groups[1].Value, out memavail)
                && long.TryParse(rexswptotal.Match(stat).Groups[1].Value, out swptotal)
                && long.TryParse(rexswpfree.Match(stat).Groups[1].Value, out swpfree))
            {
                long total = 0, avail = 0;
                if (memtotal > 0 && swptotal == 0)
                {
                    total = memtotal;
                    avail = memavail;
                }
                else if (memtotal > 0 && swptotal > 0)
                {
                    total = memtotal + swptotal;
                    avail = memavail + swpfree;
                }
                if (total == 0)
                {
                    Value = string.Empty;
                    return;
                }
                
                decimal val = Math.Floor(avail * 100m / total + 0.5m);

                Value = $"{Util.SmallSpace}{Util.PercentToBarsWhenEnabled(val)}";
                return;
            }

            Value = string.Empty;
        }
    }
}