using ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Enums;

namespace ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Interfaces
{
    /// <summary>
    /// LoggerOperationType capabilities interface
    /// </summary>
    public interface ILoggerOperationTypeCapabilities
    {
        MessageTypeEnum MessageType { get; }
        LoggerTypeEnum LoggerType { get; }
    }
}