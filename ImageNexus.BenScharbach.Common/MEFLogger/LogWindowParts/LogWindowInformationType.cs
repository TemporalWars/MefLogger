using System;
using ImageNexus.BenScharbach.Common.MEFLogger.CustomAtts;
using ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Enums;
using ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Interfaces;

namespace ImageNexus.BenScharbach.Common.MEFLogger.LogWindowParts
{
    /// <summary>
    /// The <see cref="LogWindowInformationType"/> MEF Part is used to log information message types to a WPF Console.
    /// </summary>
    [ExportLoggerOperationType(MessageType = MessageTypeEnum.Information, LoggerType = LoggerTypeEnum.WpfConsoleLog)]
    class LogWindowInformationType : ILoggerOperationType, ILoggerLogWindowType
    {
        #region Properties

        // 7/2/2011
        /// <summary>
        /// Unique AttributeKeyName.
        /// </summary>
        public string AttributeKeyName { get; set; }

        // 7/10/2011
        /// <summary>
        /// Set when attribute is applied to some assembly which is running
        /// in the context of a WCF service.
        /// </summary>
        public bool IsRunningInWcfService { get; set; }

        #endregion

        public void WriteMessageType(string message)
        {
            try
            {
                // 7/3/2011 - Get ILogWindowCtl ref
                var logWindowCtl = (ILogWindowCtl)LogWindowCtl.GetInstance;

                // 7/10/2011
                logWindowCtl.WriteMessage(MessageTypeEnum.Information, string.Concat("Information: ", message), AttributeKeyName);
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