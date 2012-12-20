using System;
using ImageNexus.BenScharbach.Common.MEFLogger.CustomAtts;
using ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Enums;
using ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Interfaces;

namespace ImageNexus.BenScharbach.Common.MEFLogger.StreamWriterLogParts
{
    /// <summary>
    /// The <see cref="StreamWriterLogErrorType"/> MEF Part is used to log warning message types to windows Trace.
    /// </summary>
    [ExportLoggerOperationType(MessageType = MessageTypeEnum.Error, LoggerType = LoggerTypeEnum.StreamWriterLog)]
    class StreamWriterLogErrorType : ILoggerOperationType
    {
        public void WriteMessageType(string message)
        {
            try
            {
                StreamWriterLog.AddLogEvent(string.Concat("Error Message: ", message));
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