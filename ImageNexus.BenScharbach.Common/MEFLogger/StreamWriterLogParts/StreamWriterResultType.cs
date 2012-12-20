using System;
using ImageNexus.BenScharbach.Common.MEFLogger.CustomAtts;
using ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Enums;
using ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Interfaces;

namespace ImageNexus.BenScharbach.Common.MEFLogger.StreamWriterLogParts
{
    /// <summary>
    /// The <see cref="StreamWriterLogResultType"/> MEF Part is used to log result message types to windows TraceLog, where
    /// the result is either True or False.  Furthermore, an internal counter is kept to store successful results vs failures.
    /// </summary>
    [ExportLoggerOperationType(MessageType = MessageTypeEnum.Result, LoggerType = LoggerTypeEnum.StreamWriterLog)]
    class StreamWriterLogResultType : ILoggerOperationType, ILoggerResultType
    {
        #region Properties

        /// <summary>
        /// Set to the number of files to process.
        /// </summary>
        public int NumberOfFilesToProcess
        {
            get { return StreamWriterLog.NumberOfFilesToProcess; } 
            set { StreamWriterLog.NumberOfFilesToProcess = value; }
        }

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
                StreamWriterLog.UpdateResultCounter(result);

                var messageWithResults = string.Concat(message,
                              String.Format("Processed {0} of {1} files successfully.", StreamWriterLog.FilesProcessSuccessfullyCounter,
                                            NumberOfFilesToProcess));


                StreamWriterLog.AddLogEvent(string.Concat("Result Message: ", messageWithResults));
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