using System;
using System.Collections.Generic;

namespace sysinfo
{
    public static class Util
    {
        static string[] bars = { "⠀", "▁", "▂", "▃", "▄", "▅", "▆", "▇", "█" };
        static string[] signs = { "", "", "", "", "", "", "", "", "" };

        public static string[] pangoError = { "<span foreground=\"red\">", "</span>" };
        public static string[] pangoWarning = { "<span foreground=\"orange\">", "</span>" };
        public static bool EnablePangoMarkup { get; set; } = false;
        public static bool EnableJsonProtocol { get; set; } = false;
        public static bool EnableBars { get; set; } = false;

        public static char SmallSpace = ' ';
        public static char Space = ' ';
        public static char Percent = '٪';

        public static string PercentToBarsWhenEnabled(decimal value, bool sign = true)
        {
            decimal factor = 100m / (bars.Length - 1);
            int pick = (int)Math.Ceiling(value / factor);
            if (pick < 0)
                pick = 0;
            else if (pick > bars.Length - 1)
                pick = bars.Length - 1;
            string val = string.Empty;
            
            if (EnableBars)
            {
                val = bars[pick];
                if (sign && signs[pick] != string.Empty)
                    val = $"{signs[pick]}{val}";
            }
            else
            {
                val = $"{value}{Percent}";
            }
            
            if (EnablePangoMarkup && pick < bars.Length / 4.2m)
                val = $"{pangoError[0]}{val}{pangoError[1]}";
            else if (EnablePangoMarkup && pick < bars.Length / 2.4m)
                val = $"{pangoWarning[0]}{val}{pangoWarning[1]}";
            return val;
        }
    }
}