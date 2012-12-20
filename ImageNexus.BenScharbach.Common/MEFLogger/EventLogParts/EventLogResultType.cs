using System;
using System.Diagnostics;
using ImageNexus.BenScharbach.Common.MEFLogger.CustomAtts;
using ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Enums;
using ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Interfaces;

namespace ImageNexus.BenScharbach.Common.MEFLogger.EventLogParts
{
    /// <summary>
    /// The <see cref="EventLogResultType"/> MEF Part is used to log result message types to windows EventLog, where
    /// the result is either True or False.  Furthermore, an internal counter is kept to store successful results vs failures.
    /// </summary>
    [ExportLoggerOperationType(MessageType = MessageTypeEnum.Result, LoggerType =  LoggerTypeEnum.EventLog)]
    class EventLogResultType : ILoggerOperationType, ILoggerResultType
    {
        // Location to store the 'Source' group to within windows LogEvents.
        private const string LogName = "Application";

        // Group 'Source' name to log events into.
        private const string LogSourceName = "ICLWebService";

        // 6/26/2011
        private int _filesProcessSuccessfullyCounter;

        #region Properties

        /// <summary>
        /// Set to the number of files to process.
        /// </summary>
        public int NumberOfFilesToProcess { get; set; }

        #endregion

        public void WriteMessageType(string message)
        {
            throw new NotImplementedException();
        }

        // 6/26/2011
        /// <summary>
        /// Message to write to the internal logger, with the result (true/false).
        /// </summary>
        /// <param name="message">Messagge to store.</param>
        /// <param name="result">Result to store (true/false)</param>
        public void WriteMessageType(string message, bool result)
        {
            try
            {
                if (string.IsNullOrEmpty(LogSourceName))
                    throw new ArgumentException("You MUST set the 'LogSourceName' property first.");

                if (!EventLog.SourceExists(LogSourceName))
                    EventLog.CreateEventSource(LogSourceName, LogName);

                _filesProcessSuccessfullyCounter += (result) ? 1 : 0;

                var messageWithResults = string.Concat(message,
                              String.Format("Processed {0} of {1} files successfully.", _filesProcessSuccessfullyCounter,
                                            NumberOfFilesToProcess));

                var evt = new EventLog { Source = LogSourceName };
                evt.WriteEntry(messageWithResults, EventLogEntryType.Information);
            }
            catch (Exception)
            {
                // Empty
            }
        }

        public string ReadMessageType()
        {
            throw new NotImplementedException();
        }
    }
}