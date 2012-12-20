using System;
using System.Diagnostics;
using ImageNexus.BenScharbach.Common.MEFLogger.CustomAtts;
using ImageNexus.BenScharbach.Common.MEFLogger.EventLogParts;
using ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Enums;
using ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Interfaces;

namespace ImageNexus.BenScharbach.Common.MEFLogger.TraceLogParts
{
    /// <summary>
    /// The <see cref="EventLogWarningType"/> MEF Part is used to log warning message types to windows Trace.
    /// </summary>
    [ExportLoggerOperationType(MessageType = MessageTypeEnum.Warning, LoggerType = LoggerTypeEnum.TraceLog)]
    class TraceLogWarningType : ILoggerOperationType, ILoggerTraceIdType
    {
        /// <summary>
        /// The ID for the created <see cref="TraceListener"/>.
        /// </summary>
        public int TraceListenerInstanceId { get; set; }

        public void WriteMessageType(string message)
        {
            try
            {
                TraceLogCtl.WriteMessage(MessageTypeEnum.Warning, TraceListenerInstanceId, string.Concat("Warning: ", message));
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