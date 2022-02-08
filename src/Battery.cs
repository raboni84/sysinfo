using System.IO;
using System.Linq;
using System.Text;

namespace sysinfo
{
    static class Battery
    {
        public static string Value { get; private set; } = string.Empty;

        public static string Name { get; private set; } = nameof(Battery);

        static Encoding utf8 = new UTF8Encoding(false);

        static bool warned = false, alerted = true;

        public static void Update()
        {
            string cablePath = "/sys/class/power_supply/AC/online";
            string capacityPath = "/sys/class/power_supply/BAT0/capacity";
            string statusPath = "/sys/class/power_supply/BAT0/status";
            if (!File.Exists(cablePath) || !File.Exists(capacityPath) || !File.Exists(statusPath))
            {
                Value = string.Empty;
                return;
            }

            string cablestr = File.ReadLines(cablePath, utf8).First();
            string capacitystr = File.ReadLines(capacityPath, utf8).First();
            string status = File.ReadLines(statusPath, utf8).First();
            string icon = ""; // battery
            string iconadd = string.Empty;
            decimal capacity;
            if (decimal.TryParse(capacitystr, out capacity))
            {
                if (cablestr == "1")
                {
                    icon = ""; // plug
                }

                if (status == "Charging")
                {
                    iconadd = ""; // level-up-alt
                }
                if (status == "Discharging")
                {
                    iconadd = ""; // level-down-alt
                }

                if (status == "Charging" || status == "Full")
                {
                    warned = false;
                    alerted = false;
                }
                else
                {
                    if (capacity > 10 && capacity <= 15 && !warned)
                    {
                        Notify.Message("Battery Warning", "Battery charge less than 15%", 15000, NotifyCategory.Battery);
                        warned = true;
                    }
                    else if (capacity <= 10 && !alerted)
                    {
                        Notify.Message("Battery Alert", "Battery charge less than 10%", 0, NotifyCategory.Battery);
                        alerted = true;
                    }
                }
                Value = $"{icon}{iconadd}{Util.SmallSpace}{Util.PercentToBarsWhenEnabled(capacity)}";
                return;
            }

            Value = string.Empty;
        }
    }
}