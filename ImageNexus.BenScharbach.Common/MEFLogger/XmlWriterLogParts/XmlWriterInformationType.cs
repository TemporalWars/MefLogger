using System;
using ImageNexus.BenScharbach.Common.MEFLogger.CustomAtts;
using ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Enums;
using ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Interfaces;

namespace ImageNexus.BenScharbach.Common.MEFLogger.XmlWriterLogParts
{
    /// <summary>
    /// The <see cref="XmlWriterLogInformationType"/> MEF Part is used to log information message types to windows Trace.
    /// </summary>
    [ExportLoggerOperationType(MessageType = MessageTypeEnum.Information, LoggerType = LoggerTypeEnum.XmlWriterLog)]
    class XmlWriterLogInformationType : ILoggerOperationType, ILoggerOperationTypeXml
    {
        public void WriteMessageType(string message)
        {
            throw new NotImplementedException();
        }

        public string ReadMessageType()
        {
            throw new NotImplementedException();
        }

        public void WriteMessageType<TData>(TData data)
        {
            try
            {
                XmlWriterLog<TData>.AddXmlLogEvent(data);
            }
            catch (Exception)
            {
                // Emtpy
            }
        }
    }
}