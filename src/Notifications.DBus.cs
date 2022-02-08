using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Tmds.DBus;

[assembly: InternalsVisibleTo(Tmds.DBus.Connection.DynamicAssemblyName)]
namespace Notifications.DBus
{
    [DBusInterface("org.dunstproject.cmd0")]
    interface ICmd0 : IDBusObject
    {
        Task ContextMenuCallAsync();
        Task NotificationActionAsync(int Number);
        Task NotificationCloseLastAsync();
        Task NotificationCloseAllAsync();
        Task NotificationShowAsync();
        Task PingAsync();
        Task<T> GetAsync<T>(string prop);
        Task<Cmd0Properties> GetAllAsync();
        Task SetAsync(string prop, object val);
        Task<IDisposable> WatchPropertiesAsync(Action<PropertyChanges> handler);
    }

    [Dictionary]
    class Cmd0Properties
    {
        private bool _paused = default(bool);
        public bool Paused
        {
            get
            {
                return _paused;
            }

            set
            {
                _paused = (value);
            }
        }

        private uint _displayedLength = default(uint);
        public uint DisplayedLength
        {
            get
            {
                return _displayedLength;
            }

            set
            {
                _displayedLength = (value);
            }
        }

        private uint _historyLength = default(uint);
        public uint HistoryLength
        {
            get
            {
                return _historyLength;
            }

            set
            {
                _historyLength = (value);
            }
        }

        private uint _waitingLength = default(uint);
        public uint WaitingLength
        {
            get
            {
                return _waitingLength;
            }

            set
            {
                _waitingLength = (value);
            }
        }
    }

    static class Cmd0Extensions
    {
        public static Task<bool> GetPausedAsync(this ICmd0 o) => o.GetAsync<bool>("paused");
        public static Task<uint> GetDisplayedLengthAsync(this ICmd0 o) => o.GetAsync<uint>("displayedLength");
        public static Task<uint> GetHistoryLengthAsync(this ICmd0 o) => o.GetAsync<uint>("historyLength");
        public static Task<uint> GetWaitingLengthAsync(this ICmd0 o) => o.GetAsync<uint>("waitingLength");
        public static Task SetPausedAsync(this ICmd0 o, bool val) => o.SetAsync("paused", val);
    }

    [DBusInterface("org.freedesktop.Notifications")]
    interface INotifications : IDBusObject
    {
        Task<string[]> GetCapabilitiesAsync();
        Task<uint> NotifyAsync(string AppName, uint ReplacesId, string AppIcon, string Summary, string Body, string[] Actions, IDictionary<string, object> Hints, int ExpireTimeout);
        Task CloseNotificationAsync(uint Id);
        Task<(string name, string vendor, string version, string specVersion)> GetServerInformationAsync();
        Task<IDisposable> WatchNotificationClosedAsync(Action<(uint id, uint reason)> handler, Action<Exception> onError = null);
        Task<IDisposable> WatchActionInvokedAsync(Action<(uint id, string actionKey)> handler, Action<Exception> onError = null);
    }
}