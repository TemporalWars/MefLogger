using System;
using ImageNexus.BenScharbach.Common.MEFLogger.CustomAtts;
using ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Enums;
using ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Interfaces;

namespace ImageNexus.BenScharbach.Common.MEFLogger.StreamWriterLogParts
{
    /// <summary>
    /// The <see cref="StreamWriterLogInformationType"/> MEF Part is used to log information message types to windows Trace.
    /// </summary>
    [ExportLoggerOperationType(MessageType = MessageTypeEnum.Information, LoggerType = LoggerTypeEnum.StreamWriterLog)]
    class StreamWriterLogInformationType : ILoggerOperationType
    {

        public void WriteMessageType(string message)
        {
            try
            {
                StreamWriterLog.AddLogEvent(string.Concat("Info Message: ", message));
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