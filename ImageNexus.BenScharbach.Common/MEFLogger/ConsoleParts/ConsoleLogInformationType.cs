using System;
using ImageNexus.BenScharbach.Common.MEFLogger.CustomAtts;
using ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Enums;
using ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Interfaces;

namespace ImageNexus.BenScharbach.Common.MEFLogger.ConsoleParts
{
    /// <summary>
    /// The <see cref="ConsoleLogInformationType"/> MEF Part is used to log information message types to a WPF Console.
    /// </summary>
    [ExportLoggerOperationType(MessageType = MessageTypeEnum.Information, LoggerType = LoggerTypeEnum.ConsoleLog)]
    class ConsoleLogInformationType : ILoggerOperationType
    {
       

        public void WriteMessageType(string message)
        {
            try
            {
                
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