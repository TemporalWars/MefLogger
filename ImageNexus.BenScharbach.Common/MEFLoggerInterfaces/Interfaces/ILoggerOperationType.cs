namespace ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Interfaces
{
    /// <summary>
    /// ILoggerOperationType interface
    /// </summary>
    public interface ILoggerOperationType
    {
        /// <summary>
        /// Message to write to the internal logger.
        /// </summary>
        /// <param name="message">Message to store.</param>
        void WriteMessageType(string message);

        /// <summary>
        /// Reads back a message from the internal logger.
        /// </summary>
        /// <returns>Message</returns>
        string ReadMessageType();

    }
}