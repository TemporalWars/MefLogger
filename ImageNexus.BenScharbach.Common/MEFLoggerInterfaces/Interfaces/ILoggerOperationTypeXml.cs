namespace ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Interfaces
{
    // 3/23/2012
    /// <summary>
    /// ILoggerOperationTypeXml interface
    /// </summary>
    public interface ILoggerOperationTypeXml
    {
        /// <summary>
        /// Serializes the given data structure and adds to the internal log file.
        /// </summary>
        /// <param name="data">Data to serialize.</param>
        void WriteMessageType<T>(T data);
    }
}