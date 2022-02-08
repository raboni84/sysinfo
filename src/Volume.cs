using System.Text;
using toolbelt;

namespace sysinfo
{
    static class Volume
    {
        public static string Value { get; private set; } = string.Empty;

        public static string Name { get; private set; } = nameof(Volume);

        static Encoding utf8 = new UTF8Encoding(false);

        static decimal lastState = int.MaxValue;

        public static void Update()
        {
            string[] vol = ShellUtils.RunShellTextAsync("pamixer", "--get-volume --get-mute").Result.Split(' ');
            if (vol.Length != 2)
            {
                Value = string.Empty;
                return;
            }
            string valset = string.Empty;
            if (vol[0] == "true")
            {
                if (lastState != -1)
                {
                    Notify.Message($"Volume: muted", string.Empty, 2000, NotifyCategory.Volume);
                    lastState = -1;
                }
                valset += $"{Util.SmallSpace}";
            }
            else if (vol[0] == "false")
            {
                valset = $"{Util.SmallSpace}";
            }
            else
            {
                Value = string.Empty;
                return;
            }
            decimal perc;
            if (decimal.TryParse(vol[1], out perc))
            {
                valset += $"{Util.PercentToBarsWhenEnabled(perc, false)}";
                if (perc != lastState && vol[0] == "false")
                {
                    Notify.Message($"Volume: {perc}%", string.Empty, 2000, NotifyCategory.Volume);
                    lastState = perc;
                }
                Value = valset;
            }
            else
            {
                Value = string.Empty;
            }
        }
    }
}