using System;
using ImageNexus.BenScharbach.Common.MEFLogger.CustomAtts;
using ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Enums;
using ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Interfaces;

namespace ImageNexus.BenScharbach.Common.MEFLogger.ConsoleParts
{
    /// <summary>
    /// The <see cref="ConsoleLogErrorType"/> MEF Part is used to log warning message types to a WPF Console.
    /// </summary>
    [ExportLoggerOperationType(MessageType = MessageTypeEnum.Error, LoggerType = LoggerTypeEnum.ConsoleLog)]
    class ConsoleLogErrorType : ILoggerOperationType
    {
       

        public void WriteMessageType(string message)
        {
            try
            {
                
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