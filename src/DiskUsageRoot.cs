using System;
using System.IO;

namespace sysinfo
{
    static class DiskUsageRoot
    {
        public static string Value { get; private set; } = string.Empty;

        public static string Name { get; private set; } = nameof(DiskUsageRoot);

        public static void Update()
        {
            DriveInfo slashDI = new DriveInfo("/");
            long slashTotal = slashDI.TotalSize;
            long slashAvail = slashDI.AvailableFreeSpace;

            decimal slash = Math.Floor(slashAvail * 100m / slashTotal + 0.5m);
            Value = $"{Util.SmallSpace}{Util.PercentToBarsWhenEnabled(slash, false)}";
        }
    }
}