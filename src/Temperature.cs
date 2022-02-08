using System;
using System.IO;
using System.Linq;
using System.Text;

namespace sysinfo
{
    static class Temperature
    {
        public static string Value { get; private set; } = string.Empty;

        public static string Name { get; private set; } = nameof(Temperature);

        static Encoding utf8 = new UTF8Encoding(false);
        
        public static void Update()
        {
            decimal min = decimal.MaxValue;
            for (int i = 0; i <= 9; i++)
            {
                string dir = $"/sys/class/hwmon/hwmon{i}";
                if (!Directory.Exists(dir))
                    continue;
                for (int j = 0; j <= 4; j++)
                {
                    string critpath = $"{dir}/temp{j}_crit";
                    if (!File.Exists(critpath))
                        continue;
                    string inputpath = $"{dir}/temp{j}_input";
                    if (!File.Exists(inputpath))
                        continue;
                    
                    string critstr = File.ReadLines(critpath, utf8).First();
                    string tempstr = File.ReadLines(inputpath, utf8).First();
                    decimal crit, temp;
                    if (decimal.TryParse(critstr, out crit)
                        && decimal.TryParse(tempstr, out temp))
                    {
                        if (temp < 0m)
                            temp = 0m;
                        decimal perc = Math.Floor((1m - (temp / crit)) * 100m + 0.5m);
                        if (perc < min)
                            min = perc;
                        continue;
                    }
                    
                    Value = string.Empty;
                    return;
                }
            }
            if (min < decimal.MaxValue)
            {
                Value = $"{Util.SmallSpace}{Util.PercentToBarsWhenEnabled(min)}";
            }
            else
            {
                Value = string.Empty;
            }
        }
    }
}