using System;
using System.IO;

namespace sysinfo
{
    static class DiskUsageData
    {
        public static string Value { get; private set; } = string.Empty;

        public static string Name { get; private set; } = nameof(DiskUsageData);

        public static void Update()
        {
            DriveInfo dataDI = new DriveInfo("/data");
            long dataTotal = dataDI.TotalSize;
            long dataAvail = dataDI.AvailableFreeSpace;

            decimal data = Math.Floor(dataAvail * 100m / dataTotal + 0.5m);
            Value = $"{Util.SmallSpace}{Util.PercentToBarsWhenEnabled(data, false)}";
        }
    }
}