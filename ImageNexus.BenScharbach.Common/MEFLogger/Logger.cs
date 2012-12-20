using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using ImageNexus.BenScharbach.Common.MEFLogger.LogWindowParts;
using ImageNexus.BenScharbach.Common.MEFLogger.StreamWriterLogParts;
using ImageNexus.BenScharbach.Common.MEFLogger.TraceLogParts;
using ImageNexus.BenScharbach.Common.MEFLogger.XmlWriterLogParts;
using ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Enums;
using ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Interfaces;

namespace ImageNexus.BenScharbach.Common.MEFLogger
{
    [Export(typeof(ILogger))]
    class Logger : ILogger
    {
        [ImportMany]
        IEnumerable<Lazy<ILoggerOperationType, ILoggerOperationTypeCapabilities>> _loggerOperations;

        // 6/18/2011
        private Dictionary<string, Lazy<ILoggerOperationType, ILoggerOperationTypeCapabilities>> _loggerOperationsDictionary;

        // 6/29/2011
        private int _traceListenerInstanceId = -1;

        #region Properties

        // 7/10/2011
        /// <summary>
        /// Set when attribute is applied to some assembly which is running
        /// in the context of a WCF service.
        /// </summary>
        public static bool IsRunningInWcfService { get; private set; }

        #endregion

        // 7/24/2011
        /// <summary>
        /// Used to subscribe to the <see cref="MEFLogger"/> attribute's event handler.
        /// </summary>
        public void ResetTraceListenerIdsCallback()
        {
            // Reset to -1, which forces the 'CreateTextTraceListener' to be called.
            _traceListenerInstanceId = -1;
        }

        // 7/24/2011
        /// <summary>
        /// When the calling program closes, this method should be called to flush and close all open
        /// TraceListeners.  The service will then recreate new listeners to replace the old ones.
        /// </summary>
        public void FlushAndCloseAllTraceListeners()
        {
            TraceLogCtl.FlushAndCloseAllTraceListeners();
        }

        // 7/2/2011
        /// <summary>
        /// Sets the <paramref name="attributeKeyName"/> given into all MEF parts for 
        /// the <see cref="MEFLoggerInterfaces.Enums.LoggerTypeEnum.WpfConsoleLog"/> type.
        /// </summary>
        /// <param name="attributeKeyName">Unique AttributeKeyName</param>
        public void SetWpfAttributeKeyName(string attributeKeyName)
        {
            // Iterate all MEF Parts for the 'LoggerTypeEnum.WpfConsoleLog;' and set the AttributeKeyName
            foreach (var loggerOperation in _loggerOperations)
            {
                if (loggerOperation.Metadata.LoggerType != LoggerTypeEnum.WpfConsoleLog) continue;

                ((ILoggerLogWindowType) loggerOperation.Value).AttributeKeyName = attributeKeyName;
            }
        }

        // 7/10/2011
        /// <summary>
        /// Sets the <paramref name="isRunningInWcfService"/> given into all MEF parts for 
        /// the <see cref="MEFLoggerInterfaces.Enums.LoggerTypeEnum.WpfConsoleLog"/> type.
        /// </summary>
        /// <param name="isRunningInWcfService">Set when attribute applied within a WCF context.</param>
        /// <param name="attributeKeyName">Unique AttributeKeyName</param>
        /// <param name="documentId">Unique DocumentId Key to associate the proper TraceDocument WPF output console.</param>
        public void SetWcfServiceFlag(bool isRunningInWcfService, string attributeKeyName, int documentId)
        {
            // Iterate all MEF Parts for the 'LoggerTypeEnum.WpfConsoleLog;' and set the isRunningInWcfService
            foreach (var loggerOperation in _loggerOperations)
            {
                if (loggerOperation.Metadata.LoggerType != LoggerTypeEnum.WpfConsoleLog) continue;

                ((ILoggerLogWindowType)loggerOperation.Value).IsRunningInWcfService = isRunningInWcfService;
            }

            // Set in LogWindowCtl
            IsRunningInWcfService = isRunningInWcfService;

            // Create WcfTraceListener instance
            // Note: Only on service.
            if (isRunningInWcfService)
                LogWindowCtl.CreateWcfTraceDocumentListener(attributeKeyName, documentId);
        }

        // 7/3/2011
        /// <summary>
        /// Adds the given <paramref name="attributeKeyName"/> and <paramref name="documentId"/> association to 
        /// the internal dictionary.
        /// </summary>
        /// <param name="attributeKeyName">Unique AttributeKeyName</param>
        /// <param name="documentId">Unique DocumentId to associate the proper TraceDocument WPF output console.</param>
        public void SetTraceDocumentKey(string attributeKeyName, int documentId)
        {
            // 7/3/2011 - Get ILogWindowCtl ref
            var logWindowCtl = (ILogWindowCtl)LogWindowCtl.GetInstance;

            logWindowCtl.SetTraceDocumentKey(attributeKeyName, documentId);
        }

        /// <summary>
        /// Writes a logger message to the current <see cref="MEFLoggerInterfaces.Enums.LoggerTypeEnum"/>.
        /// </summary>
        /// <param name="loggerType">Set the <see cref="MEFLoggerInterfaces.Enums.LoggerTypeEnum"/> to use.</param>
        /// <param name="messageType">Set the type of <see cref="MEFLoggerInterfaces.Enums.MessageTypeEnum"/> to use.</param>
        /// <param name="loggerMessage">Message to save into logger</param>
        /// <param name="loggerOutputPath">Sub-folder location to place logs within the 'ProgramData' folder.</param>
        public void WriteItem(LoggerTypeEnum loggerType, MessageTypeEnum messageType, string loggerMessage, string loggerOutputPath)
        {
            VerifyDictionaryIsCreated();

            if (loggerType == LoggerTypeEnum.TraceLog)
            {
                if (_traceListenerInstanceId == -1)
                {
                    if (String.IsNullOrEmpty(loggerOutputPath))
                        loggerOutputPath = "ICL_Trace_Logs\\TraceLogCallsOutput.txt";

                    _traceListenerInstanceId = CreateTextTraceListener(loggerOutputPath);
                    if (_traceListenerInstanceId == -1) return;
                }

                WriteItem(messageType, loggerMessage, _traceListenerInstanceId);
                return;
            }

            LoggerOperationsDictionaryAddMessage(ref loggerType, ref messageType, ref loggerMessage, LogMessageCallback);
        }

        // 6/26/2011
        /// <summary>
        /// Writes a logger message to the current <see cref="MEFLoggerInterfaces.Enums.LoggerTypeEnum"/>, with a result (true/false) included.
        /// </summary>
        /// <param name="loggerType">Set the <see cref="MEFLoggerInterfaces.Enums.LoggerTypeEnum"/> to use.</param>
        /// <param name="loggerMessage">Message to save into logger</param>
        /// <param name="result">Result of operation (true/false)</param>
        /// <param name="loggerOutputPath">Sub-folder location to place logs within the 'ProgramData' folder.</param>
        public void WriteItem(LoggerTypeEnum loggerType, string loggerMessage, bool result, string loggerOutputPath)
        {
            VerifyDictionaryIsCreated();

            if (loggerType == LoggerTypeEnum.TraceLog)
            {
                if (_traceListenerInstanceId == -1)
                {
                    if (String.IsNullOrEmpty(loggerOutputPath))
                        loggerOutputPath = "ICL_Trace_Logs\\TraceLogCallsOutput.txt";

                    _traceListenerInstanceId = CreateTextTraceListener(loggerOutputPath);
                    if (_traceListenerInstanceId == -1) return;
                }

                WriteItem(loggerMessage, _traceListenerInstanceId, result);
                return;
            }

            var messageType = MessageTypeEnum.Result;
            LoggerOperationsDictionaryAddMessage(ref loggerType, ref messageType, ref loggerMessage, 
                LogResultMessageCallback, new LoggerOpDictParam{Result = result});
        }

        // 3/23/2012
        /// <summary>
        /// Writes a logger message to the current <see cref="LoggerTypeEnum"/>.
        /// </summary>
        /// <param name="loggerType">Set the <see cref="LoggerTypeEnum"/> to use.</param>
        /// <param name="messageType">Set the type of <see cref="MessageTypeEnum"/> to use.</param>
        /// <param name="data">Data to serialize</param>
        /// <param name="loggerOutputPath"></param>
        public void WriteItem<T>(LoggerTypeEnum loggerType, MessageTypeEnum messageType, T data, string loggerOutputPath)
        {
            VerifyDictionaryIsCreated();

            if (loggerType != LoggerTypeEnum.XmlWriterLog)
            {
                throw new InvalidOperationException("This logger 'WriteItem' signature is only for the 'Xml' logger type.");
            }

            LoggerOperationsDictionaryAddMessage(ref loggerType, ref messageType, data);
        }

        // 6/29/2011
        /// <summary>
        /// Creates an instance of a Text TraceListener.
        /// </summary>
        private static int CreateTextTraceListener(string loggerOutputPath = "ICL_Trace_Logs\\TraceLogCallsOutput.txt")
        {
            return TraceLogCtl.CreateTextTraceListener(loggerOutputPath);
        }

        // 6/27/2011
        /// <summary>
        /// Writes a Trace logger message to the current <see cref="MEFLoggerInterfaces.Enums.LoggerTypeEnum"/>, with a result (true/false) included.
        /// </summary>
        /// <param name="loggerMessage">Message to save into logger</param>
        /// <param name="traceListenerInstanceId">The ID for the created <see cref="TraceListener"/></param>
        /// <param name="result">Result of operation (true/false)</param>
        private void WriteItem(string loggerMessage, int traceListenerInstanceId, bool result)
        {
            VerifyDictionaryIsCreated();

            var loggerType = LoggerTypeEnum.TraceLog;
            var messageType = MessageTypeEnum.Result;
            LoggerOperationsDictionaryAddMessage(ref loggerType, ref messageType, ref loggerMessage,
                LogTraceResultMessageCallback, new LoggerOpDictParam { Result = result, TraceListenerInstanceId = traceListenerInstanceId});
        }


        // 6/27/2011
        /// <summary>
        /// Writes a Trace logger message to the current <see cref="MEFLoggerInterfaces.Enums.LoggerTypeEnum"/>, using the Listener ID to write
        /// to a specific instance.
        /// </summary>
        /// <param name="messageType">Set the type of <see cref="MEFLoggerInterfaces.Enums.MessageTypeEnum"/> to use.</param>
        ///  /// <param name="loggerMessage">Message to save into logger</param>
        /// <param name="traceListenerInstanceId">The ID for the created <see cref="TraceListener"/></param>
        private void WriteItem(MessageTypeEnum messageType, string loggerMessage, int traceListenerInstanceId)
        {
            VerifyDictionaryIsCreated();

            var loggerType = LoggerTypeEnum.TraceLog;
            LoggerOperationsDictionaryAddMessage(ref loggerType, ref messageType, ref loggerMessage,
                LogTraceMessageCallback, new LoggerOpDictParam { TraceListenerInstanceId = traceListenerInstanceId });
        }

        /// <summary>
        /// Reads a message back.
        /// </summary>
        /// <param name="loggerType">Set the <see cref="MEFLoggerInterfaces.Enums.LoggerTypeEnum"/> to use.</param>
        /// <param name="messageType">Set the type of <see cref="MEFLoggerInterfaces.Enums.MessageTypeEnum"/> to use.</param>
        /// <returns>Message</returns>
        public string ReadItem(LoggerTypeEnum loggerType, MessageTypeEnum messageType)
        {
            VerifyDictionaryIsCreated();

            var loggerKey = String.Concat(loggerType, messageType);
            Lazy<ILoggerOperationType, ILoggerOperationTypeCapabilities> loggerOperation;

            return _loggerOperationsDictionary.TryGetValue(loggerKey, out loggerOperation) ? loggerOperation.Value.ReadMessageType() : String.Empty;
        }

        // 6/26/2011
        /// <summary>
        /// Sets the header to use in the current logger.
        /// </summary>
        /// <param name="loggerType">Set the <see cref="MEFLoggerInterfaces.Enums.LoggerTypeEnum"/> to use.</param>
        /// <param name="headerName">Header name to use - defaults to 'Log Activity'</param>
        public void SetHeader(LoggerTypeEnum loggerType, string headerName)
        {
            VerifyDictionaryIsCreated();

            var header = String.Format("-------------------- {0} ---------------------", headerName);

            var messageType = MessageTypeEnum.Information;
            LoggerOperationsDictionaryAddMessage(ref loggerType, ref messageType, ref header, LogMessageCallback);
        }

        // 6/26/2011
        /// <summary>
        /// Sets the footer to use in the current logger.
        /// </summary>
        /// <param name="loggerType">Set the <see cref="MEFLoggerInterfaces.Enums.LoggerTypeEnum"/> to use.</param>
        /// <param name="footerName">Footer name to use - defaults to 'End of Log Activity'</param>
        public void SetFooter(LoggerTypeEnum loggerType, string footerName)
        {
            VerifyDictionaryIsCreated();

            var footer = String.Format("-------------------- {0} ---------------------", footerName);

            var messageType = MessageTypeEnum.Information;
            LoggerOperationsDictionaryAddMessage(ref loggerType, ref messageType, ref footer, LogMessageCallback);
        }

        // 6/26/2011
        /// <summary>
        /// Sets the total number of result files expected to process.
        /// </summary>
        /// <param name="loggerType">Set the <see cref="MEFLoggerInterfaces.Enums.LoggerTypeEnum"/> to use.</param>
        /// <param name="filesToProcess">Total number of expected result files to process</param>
        public void SetNumberOfFilesToProcess(LoggerTypeEnum loggerType, int filesToProcess)
        {
            VerifyDictionaryIsCreated(); // 7/23/2011

            var loggerKey = String.Concat(loggerType, MessageTypeEnum.Result);
            Lazy<ILoggerOperationType, ILoggerOperationTypeCapabilities> loggerOperation;
            if (_loggerOperationsDictionary.TryGetValue(loggerKey, out loggerOperation))
            {
                ((ILoggerResultType) loggerOperation.Value).NumberOfFilesToProcess = filesToProcess;
            }
        }

        /// <summary>
        /// Starts the <see cref="StreamWriter"/> log file.
        /// </summary>
        /// <param name="logName">LogName to use for output file.</param>
        /// <param name="headerName">(Optional) HeaderName to use - defaults to 'Log Activity'</param>
        public void StartStreamWriterLogfile(string logName, string headerName = "Log Activity")
        {
            StreamWriterLog.StartLogfile(logName, headerName);
        }

        /// <summary>
        /// Closes an open <see cref="StreamWriter"/> log file.
        /// </summary>
        public void EndStreamWriterLogFile()
        {
            StreamWriterLog.EndLogFile();
        }

        // 3/23/2012
        public void StartXmlWriterLogfile<TData>(string logName)
        {
            XmlWriterLog<TData>.StartXmlLogfile(logName);
        }

        // 3/23/2012
        public void EndXmlWriterLogFile<TData>()
        {
            XmlWriterLog<TData>.EndXmlLogFile();
        }

        // 6/26/2011
        /// <summary>
        /// Helper method which insures the logger-operations dictionary is created.
        /// </summary>
        private void VerifyDictionaryIsCreated()
        {
            if (_loggerOperationsDictionary == null)
                _loggerOperationsDictionary = _loggerOperations.ToDictionary(k => String.Concat(k.Metadata.LoggerType, k.Metadata.MessageType));
        }

        // 6/27/2011
        private struct LoggerOpDictParam
        {
            public bool Result;
            public int TraceListenerInstanceId;
        }

        // 6/26/2011
        /// <summary>
        /// Helper method which logs a message into the proper MEF Part, contained in the operations dictionary.
        /// </summary>
        /// <param name="loggerType">The <see cref="MEFLoggerInterfaces.Enums.LoggerTypeEnum"/></param>
        /// <param name="messageType">The <see cref="MEFLoggerInterfaces.Enums.MessageTypeEnum"/></param>
        /// <param name="loggerMessage">Message to store</param>
        /// <param name="logMessage"></param>
        /// <param name="loggerOpDictParams"></param>
        private void LoggerOperationsDictionaryAddMessage(ref LoggerTypeEnum loggerType, ref MessageTypeEnum messageType,
            ref string loggerMessage, Action<Lazy<ILoggerOperationType, ILoggerOperationTypeCapabilities>, string, LoggerOpDictParam> logMessage, 
            LoggerOpDictParam loggerOpDictParams = new LoggerOpDictParam())
        {
            var loggerKey = String.Concat(loggerType, messageType);
            Lazy<ILoggerOperationType, ILoggerOperationTypeCapabilities> loggerOperation;
            if (_loggerOperationsDictionary.TryGetValue(loggerKey, out loggerOperation))
            {
                logMessage(loggerOperation, loggerMessage, loggerOpDictParams);
            }
        }

        // 3/23/2012
        /// <summary>
        /// Helper method which logs a message into the proper MEF Part, contained in the operations dictionary.
        /// </summary>
        /// <param name="loggerType">The <see cref="MEFLoggerInterfaces.Enums.LoggerTypeEnum"/></param>
        /// <param name="messageType">The <see cref="MEFLoggerInterfaces.Enums.MessageTypeEnum"/></param>
        /// <param name="data">Data to serialize</param>
        /// <param name="loggerOpDictParams"></param>
        private void LoggerOperationsDictionaryAddMessage<T>(ref LoggerTypeEnum loggerType, ref MessageTypeEnum messageType,
                                                             T data, LoggerOpDictParam loggerOpDictParams = new LoggerOpDictParam())
        {
            var loggerKey = String.Concat(loggerType, messageType);
            Lazy<ILoggerOperationType, ILoggerOperationTypeCapabilities> loggerOperation;
            if (_loggerOperationsDictionary.TryGetValue(loggerKey, out loggerOperation))
            {
                ((ILoggerOperationTypeXml)loggerOperation.Value).WriteMessageType(data);
            }
        }

        /// <summary>
        /// Log Message Action callback for the <see cref="LoggerOperationsDictionaryAddMessage"/>.
        /// </summary>
        private static void LogMessageCallback(Lazy<ILoggerOperationType, ILoggerOperationTypeCapabilities> loggerOperation, string loggerMessage, LoggerOpDictParam result)
        {
            loggerOperation.Value.WriteMessageType(loggerMessage);
        }

        /// <summary>
        /// Log Result-Message Action callback for the <see cref="LoggerOperationsDictionaryAddMessage"/>.
        /// </summary>
        private static void LogResultMessageCallback(Lazy<ILoggerOperationType, ILoggerOperationTypeCapabilities> loggerOperation, string loggerMessage, LoggerOpDictParam result)
        {
            ((ILoggerResultType)loggerOperation.Value).WriteMessageType(loggerMessage, result.Result);
        }

        /// <summary>
        /// Log Trace-Message Action callback for the <see cref="LoggerOperationsDictionaryAddMessage"/>.
        /// </summary>
        private static void LogTraceMessageCallback(Lazy<ILoggerOperationType, ILoggerOperationTypeCapabilities> loggerOperation, string loggerMessage, LoggerOpDictParam result)
        {
            ((ILoggerTraceIdType) loggerOperation.Value).TraceListenerInstanceId = result.TraceListenerInstanceId;
            loggerOperation.Value.WriteMessageType(loggerMessage);
        }

        /// <summary>
        /// Log Trace-Result-Message Action callback for the <see cref="LoggerOperationsDictionaryAddMessage"/>.
        /// </summary>
        private static void LogTraceResultMessageCallback(Lazy<ILoggerOperationType, ILoggerOperationTypeCapabilities> loggerOperation, string loggerMessage, LoggerOpDictParam result)
        {
            ((ILoggerTraceIdType)loggerOperation.Value).TraceListenerInstanceId = result.TraceListenerInstanceId;
            ((ILoggerResultType)loggerOperation.Value).WriteMessageType(loggerMessage, result.Result);
        }

        // 7/25/2011
        /// <summary>
        /// Returns true if the current process is a service, false otherwise
        /// </summary>
        /// <param name="pId">process Id to check</param>
        /// <returns>true if the current process is a service, false otherwise</returns>
        bool ILogger.IsService(uint pId)
        {
            return IsService(pId);
        }

        // 7/25/2011
        /// <summary>
        /// Returns true if the current process is a service, false otherwise
        /// </summary>
        /// <param name="pId">process Id to check</param>
        /// <returns>true if the current process is a service, false otherwise</returns>
        public static bool IsService(uint pId)
        {
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Service WHERE ProcessId =" + "\"" + pId + "\""))
            {
                foreach (ManagementObject service in searcher.Get()) return true;
            }
            return false;
        }

        // 7/26/2011
        /// <summary>
        /// Deletes the 'traceLogRunTimeStamp.txt', if it exist.
        /// </summary>
        public void DoStartCheckIfNeedsDeletion()
        {
            TraceLogCtl.DoStartCheckIfNeedsDeletion();
        }
    }
}