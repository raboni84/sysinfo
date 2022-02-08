using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace sysinfo
{
    static class NetUsage
    {
        public static string Value { get; private set; } = string.Empty;

        public static string Name { get; private set; } = nameof(NetUsage);

        static Encoding utf8 = new UTF8Encoding(false);
        static long prev_rxtx = 0, max_rxtx = 0;
        static Regex numbers = new Regex(@"\d+", RegexOptions.Compiled);

        public static void Update()
        {
            long rxtx = 0;
            foreach (var dev in Directory.EnumerateDirectories("/sys/class/net/").Where(x => !x.EndsWith("/lo")))
            {
                string rx_str = File.ReadLines($"{dev}/statistics/rx_bytes", utf8).First();
                string tx_str = File.ReadLines($"{dev}/statistics/tx_bytes", utf8).First();
                long rx_val, tx_val;
                if (!long.TryParse(rx_str, out rx_val) ||
                    !long.TryParse(tx_str, out tx_val))
                {
                    Value = string.Empty;
                    return;
                }
                rxtx += rx_val;
                rxtx += tx_val;
            }

            if (prev_rxtx == 0)
            {
                prev_rxtx = rxtx;
                Value = string.Empty;
                return;
            }

            long diff_rxtx = rxtx - prev_rxtx;
            if (max_rxtx < diff_rxtx)
                max_rxtx = diff_rxtx;
            
            decimal percent = 100m - Math.Floor(diff_rxtx * 100m / max_rxtx + 0.5m);

            Value = $"{Util.SmallSpace}{Util.PercentToBarsWhenEnabled(percent)}";
            prev_rxtx = rxtx;
        }
    }
}