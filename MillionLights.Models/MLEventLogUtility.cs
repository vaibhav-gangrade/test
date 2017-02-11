using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Millionlights.Models
{
    public enum MLEventMessageType : byte
    {
        Information = 1,
        Warning = 2,
        Error = 3
    }
    public class MLEventLogUtility
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
            WriteEntry("Millionlights", entry, MLEventMessageType.Information);
        }


        public static void WriteEntry(string entry, MLEventMessageType type)
        {
            WriteEntry("Millionlights", entry, type);
        }

        public static void WriteEntry(string appName, string entry, MLEventMessageType type)
        {
            EventLogEntryType entryType;
            switch (type)
            {
                case MLEventMessageType.Error:
                    entryType = EventLogEntryType.Error;
                    break;
                case MLEventMessageType.Warning:
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



