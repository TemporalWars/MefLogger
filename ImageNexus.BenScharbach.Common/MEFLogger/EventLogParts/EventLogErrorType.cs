using System;
using System.Diagnostics;
using ImageNexus.BenScharbach.Common.MEFLogger.CustomAtts;
using ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Enums;
using ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Interfaces;

namespace ImageNexus.BenScharbach.Common.MEFLogger.EventLogParts
{
    /// <summary>
    /// The <see cref="EventLogErrorType"/> MEF Part is used to log warning message types to windows EventLog.
    /// </summary>
    [ExportLoggerOperationType(MessageType = MessageTypeEnum.Error, LoggerType = LoggerTypeEnum.EventLog)]
    class EventLogErrorType : ILoggerOperationType
    {
        // Location to store the 'Source' group to within windows LogEvents.
        private const string LogName = "Application";

        // Group 'Source' name to log events into.
        private const string LogSourceName = "ICLWebService";

        public void WriteMessageType(string message)
        {
            try
            {
                //const string sSource = "ICLWebService";

                if (string.IsNullOrEmpty(LogSourceName))
                    throw new ArgumentException("You MUST set the 'LogSourceName' property first.");

                if (!EventLog.SourceExists(LogSourceName))
                    EventLog.CreateEventSource(LogSourceName, LogName);

                var evt = new EventLog { Source = LogSourceName };
                evt.WriteEntry(message, EventLogEntryType.Error);
            }
            catch (Exception)
            {
                // Emtpy
            }
        }

        public string ReadMessageType()
        {
            throw new NotImplementedException();
        }
    }
}