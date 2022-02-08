using System.Collections.Generic;
using Notifications.DBus;
using Tmds.DBus;

namespace sysinfo
{
    public enum NotifyCategory : uint
    {
        Info = 0,
        Temperature = 1,
        Battery = 2,
        Volume = 3
    }

    public static class Notify
    {
        static Dictionary<string, object> emptyHints = new Dictionary<string, object>();
        static INotifications notify = Connection.Session
            .CreateProxy<INotifications>("org.freedesktop.Notifications", "/org/freedesktop/Notifications");

        public static uint Message(string title, string summary, int timeout = 15000, NotifyCategory category = NotifyCategory.Info)
        {
            uint res = notify.NotifyAsync("sysinfo", (uint)category, string.Empty,
                title, summary, new string[] { },
                emptyHints, timeout).Result;
            return res;
        }
    }
}