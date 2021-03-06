﻿using System;
using System.Diagnostics;
using ImageNexus.BenScharbach.Common.MEFLogger.CustomAtts;
using ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Enums;
using ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Interfaces;

namespace ImageNexus.BenScharbach.Common.MEFLogger.TraceLogParts
{
    /// <summary>
    /// The <see cref="TraceLogErrorType"/> MEF Part is used to log warning message types to windows Trace.
    /// </summary>
    [ExportLoggerOperationType(MessageType = MessageTypeEnum.Error, LoggerType = LoggerTypeEnum.TraceLog)]
    class TraceLogErrorType : ILoggerOperationType, ILoggerTraceIdType
    {
        /// <summary>
        /// The ID for the created <see cref="TraceListener"/>.
        /// </summary>
        public int TraceListenerInstanceId { get; set; }
       

        public void WriteMessageType(string message)
        {
            try
            {
                TraceLogCtl.WriteMessage(MessageTypeEnum.Error, TraceListenerInstanceId, string.Concat("ERROR: ", message));
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