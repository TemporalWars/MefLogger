using System;
using ImageNexus.BenScharbach.Common.MEFLogger.CustomAtts;
using ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Enums;
using ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Interfaces;

namespace ImageNexus.BenScharbach.Common.MEFLogger.XmlWriterLogParts
{
    /// <summary>
    /// The <see cref="XmlWriterLogErrorType"/> MEF Part is used to log warning message types to windows Trace.
    /// </summary>
    [ExportLoggerOperationType(MessageType = MessageTypeEnum.Error, LoggerType = LoggerTypeEnum.XmlWriterLog)]
    class XmlWriterLogErrorType : ILoggerOperationType, ILoggerOperationTypeXml
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