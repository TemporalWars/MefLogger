using ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Enums;

namespace ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Interfaces
{
    // 3/23/2012: Updated with interface 'ILoggerXmlWriterType'.
    /// <summary>
    /// ILogger interface
    /// </summary>
    public interface ILogger : ILoggerStreamWriterType, ILoggerXmlWriterType
    {
        /// <summary>
        /// Writes a logger message to the current <see cref="LoggerTypeEnum"/>.
        /// </summary>
        /// <param name="loggerType">Set the <see cref="LoggerTypeEnum"/> to use.</param>
        /// <param name="messageType">Set the type of <see cref="MessageTypeEnum"/> to use.</param>
        /// <param name="loggerMessage">Message to save into logger</param>
        /// <param name="loggerOutputPath"></param>
        void WriteItem(LoggerTypeEnum loggerType, MessageTypeEnum messageType, string loggerMessage, string loggerOutputPath);

        // 6/26/2011
        /// <summary>
        /// Writes a logger message to the current <see cref="LoggerTypeEnum"/>, with a result (true/false) included.
        /// </summary>
        /// <param name="loggerType">Set the <see cref="LoggerTypeEnum"/> to use.</param>
        /// <param name="loggerMessage">Message to save into logger</param>
        /// <param name="result">Result of operation (true/false)</param>
        /// <param name="loggerOutputPath"></param>
        void WriteItem(LoggerTypeEnum loggerType, string loggerMessage, bool result, string loggerOutputPath);

        // 3/23/2012
        /// <summary>
        /// Writes a logger message to the current <see cref="LoggerTypeEnum"/>.
        /// </summary>
        /// <param name="loggerType">Set the <see cref="LoggerTypeEnum"/> to use.</param>
        /// <param name="messageType">Set the type of <see cref="MessageTypeEnum"/> to use.</param>
        /// <param name="data">Data to serialize</param>
        /// <param name="loggerOutputPath"></param>
        void WriteItem<T>(LoggerTypeEnum loggerType, MessageTypeEnum messageType, T data, string loggerOutputPath);

        /// <summary>
        /// Reads a message back.
        /// </summary>
        /// <param name="loggerType">Set the <see cref="LoggerTypeEnum"/> to use.</param>
        /// <param name="messageType">Set the type of <see cref="MessageTypeEnum"/> to use.</param>
        /// <returns>Message</returns>
        string ReadItem(LoggerTypeEnum loggerType, MessageTypeEnum messageType);

        // 6/26/2011
        /// <summary>
        /// Sets the header to use in the current logger.
        /// </summary>
        /// <param name="loggerType">Set the <see cref="LoggerTypeEnum"/> to use.</param>
        /// <param name="headerName">Header name to use - defaults to 'Log Activity'</param>
        void SetHeader(LoggerTypeEnum loggerType, string headerName = "Log Activity");

        // 6/26/2011
        /// <summary>
        /// Sets the footer to use in the current logger.
        /// </summary>
        /// <param name="loggerType">Set the <see cref="LoggerTypeEnum"/> to use.</param>
        /// <param name="footerName">Footer name to use - defaults to 'End of Log Activity'</param>
        void SetFooter(LoggerTypeEnum loggerType, string footerName = "End of Log Activity");

        /// <summary>
        /// Sets the total number of result files expected to process.
        /// </summary>
        /// <param name="loggerType">Set the <see cref="LoggerTypeEnum"/> to use.</param>
        /// <param name="filesToProcess">Total number of expected result files to process</param>
        void SetNumberOfFilesToProcess(LoggerTypeEnum loggerType, int filesToProcess);

        /// <summary>
        /// Sets the <paramref name="attributeKeyName"/> given into all MEF parts for the <see cref="LoggerTypeEnum.WpfConsoleLog"/> type.
        /// </summary>
        /// <param name="attributeKeyName">Unique AttributeKeyName</param>
        void SetWpfAttributeKeyName(string attributeKeyName);

        // 7/3/2011
        /// <summary>
        /// Adds the given <paramref name="attributeKeyName"/> and <paramref name="documentId"/> association to 
        /// the internal dictionary.
        /// </summary>
        /// <param name="attributeKeyName">Unique AttributeKeyName</param>
        /// <param name="documentId">Unique DocumentId to associate the proper TraceDocument WPF output console.</param>
        void SetTraceDocumentKey(string attributeKeyName, int documentId);

        /// <summary>
        /// Sets the <paramref name="isRunningInWcfService"/> given into all MEF parts for 
        /// the <see cref="LoggerTypeEnum.WpfConsoleLog"/> type.
        /// </summary>
        /// <param name="isRunningInWcfService">Set when attribute applied within a WCF context.</param>
        /// <param name="attibuteKeyName"></param>
        /// <param name="documentId">Unique DocumentId Key to associate the proper TraceDocument WPF output console.</param>
        void SetWcfServiceFlag(bool isRunningInWcfService, string attibuteKeyName, int documentId);

        // 7/24/2011
        /// <summary>
        /// When the calling program closes, this method should be called to flush and close all open
        /// TraceListeners.  The service will then recreate new listeners to replace the old ones.
        /// </summary>
        void FlushAndCloseAllTraceListeners();

        // 7/24/2011
        /// <summary>
        /// Used to subscribe to the MEFLogger attribute's event handler.
        /// </summary>
        void ResetTraceListenerIdsCallback();

        // 7/25/2011
        /// <summary>
        /// Returns true if the current process is a service, false otherwise
        /// </summary>
        /// <param name="pId">process Id to check</param>
        /// <returns>true if the current process is a service, false otherwise</returns>
        bool IsService(uint pId);

        /// <summary>
        /// Deletes the 'traceLogRunTimeStamp.txt', if it exist.
        /// </summary>
        void DoStartCheckIfNeedsDeletion();
    }
}