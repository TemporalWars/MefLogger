using System;
using System.Diagnostics;
using ImageNexus.BenScharbach.Common.MEFLogger.CustomAtts;
using ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Enums;
using ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Interfaces;

namespace ImageNexus.BenScharbach.Common.MEFLogger.TraceLogParts
{
    /// <summary>
    /// The <see cref="TraceLogInformationType"/> MEF Part is used to log information message types to windows Trace.
    /// </summary>
    [ExportLoggerOperationType(MessageType = MessageTypeEnum.Information, LoggerType =  LoggerTypeEnum.TraceLog)]
    class TraceLogInformationType : ILoggerOperationType, ILoggerTraceIdType
    {
        /// <summary>
        /// The ID for the created <see cref="TraceListener"/>.
        /// </summary>
        public int TraceListenerInstanceId { get; set; }

        public void WriteMessageType(string message)
        {
            try
            {
                TraceLogCtl.WriteMessage(MessageTypeEnum.Information, TraceListenerInstanceId, string.Concat("Information: ", message));
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