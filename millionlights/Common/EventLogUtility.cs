using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Millionlights.Common
{
    public enum EventMessageType : byte
    {
        Information = 1,
        Warning = 2,
        Error = 3
    }
    public class EventLogUtility
    {

        private static string eventSource;
        private static bool isDeployed;

        public static string EventSource
        {
            get { return eventSource; }
            set { eventSource = value; }
        }

        public static bool IsDeployed
        {
            get { return isDeployed; }
            set { isDeployed = value; }
        }

        public static void WriteEntry(string entry)
        {
            WriteEntry("Millionlights", entry, EventMessageType.Information);
        }


        public static void WriteEntry(string entry, EventMessageType type)
        {
            WriteEntry("Millionlights", entry, type);
        }

        public static void WriteEntry(string appName, string entry, EventMessageType type)
        {
            EventLogEntryType entryType;
            switch (type)
            {
                case EventMessageType.Error:
                    entryType = EventLogEntryType.Error;
                    break;
                case EventMessageType.Warning:
                    entryType = EventLogEntryType.Warning;
                    break;
                default:
                    entryType = EventLogEntryType.Information;
                    break;
            }

            if (!EventLog.SourceExists(appName))
            {
                EventLog.CreateEventSource(appName, "MillionlightsLog");
            }
            EventLogPermission permission = new EventLogPermission(EventLogPermissionAccess.Administer, ".");
            permission.PermitOnly();

            EventLog.WriteEntry(appName, entry, entryType);
        }
    }
}