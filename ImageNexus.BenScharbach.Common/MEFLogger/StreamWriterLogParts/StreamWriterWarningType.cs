using System;
using ImageNexus.BenScharbach.Common.MEFLogger.CustomAtts;
using ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Enums;
using ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Interfaces;

namespace ImageNexus.BenScharbach.Common.MEFLogger.StreamWriterLogParts
{
    /// <summary>
    /// The <see cref="StreamWriterLogWarningType"/> MEF Part is used to log warning message types to windows Trace.
    /// </summary>
    [ExportLoggerOperationType(MessageType = MessageTypeEnum.Warning, LoggerType = LoggerTypeEnum.StreamWriterLog)]
    class StreamWriterLogWarningType : ILoggerOperationType
    {

        public void WriteMessageType(string message)
        {
            try
            {
                StreamWriterLog.AddLogEvent(string.Concat("Warning Message: ", message));
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