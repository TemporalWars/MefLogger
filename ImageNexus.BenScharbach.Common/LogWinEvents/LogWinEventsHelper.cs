using System;
using System.Diagnostics;

namespace ImageNexus.BenScharbach.Common.LogWinEvents
{
    // 11/9/2010
    /// <summary>
    /// The <see cref="LogWinEventsHelper"/> class is used to log event messages and errors
    /// to windows event log.
    /// </summary>
    public static class LogWinEventsHelper
    {
        // Location to store the 'Source' group to within windows LogEvents.
        private const string LogName = "Application";
        
        /// <summary>
        ///  Group 'Source' name to log events into.
        /// </summary>
        public static string LogSourceName { get; set; }

        /// <summary>
        /// Logs an error message into windows <see cref="EventLog"/>.
        /// </summary>
        public static void LogErrorMessage(string errorMessage)
        {
            try
            {
                //const string sSource = "ICLWebService";

                if (string.IsNullOrEmpty(LogSourceName))
                    throw new ArgumentException("You MUST set the 'LogSourceName' property first.");

                if (!EventLog.SourceExists(LogSourceName))
                    EventLog.CreateEventSource(LogSourceName, LogName);

                var evt = new EventLog { Source = "ICLWebService" };
                evt.WriteEntry(errorMessage, EventLogEntryType.Error);
            }
            catch (Exception)
            {
               // Emtpy
            }
           
        }

        /// <summary>
        /// Logs a general information message into windows <see cref="EventLog"/>.
        /// </summary>
        public static void LogInformationMessage(string infoMessage)
        {
            try
            {
                //const string sSource = "ICLWebService";

                if (string.IsNullOrEmpty(LogSourceName))
                    throw new ArgumentException("You MUST set the 'LogSourceName' property first.");

                if (!EventLog.SourceExists(LogSourceName))
                    EventLog.CreateEventSource(LogSourceName, LogName);

                var evt = new EventLog { Source = "ICLWebService" };
                evt.WriteEntry(infoMessage, EventLogEntryType.Information);
            }
            catch (Exception)
            {
                // Empty
            }
            
        }

        /// <summary>
        /// Logs a general information message into windows <see cref="EventLog"/>.
        /// </summary>
        public static void LogWarningMessage(string infoMessage)
        {
            try
            {
                //const string sSource = "ICLWebService";

                if (string.IsNullOrEmpty(LogSourceName))
                    throw new ArgumentException("You MUST set the 'LogSourceName' property first.");

                if (!EventLog.SourceExists(LogSourceName))
                    EventLog.CreateEventSource(LogSourceName, LogName);

                var evt = new EventLog { Source = "ICLWebService" };
                evt.WriteEntry(infoMessage, EventLogEntryType.Warning);
            }
            catch (Exception)
            {
                // Empty
            }
            
        }
    }
}
