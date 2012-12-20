namespace ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Interfaces
{
    // 6/26/2011
    /// <summary>
    /// The <see cref="ILoggerResultType"/> is used to add additional interface channels, beyond the
    /// standard <see cref="ILoggerOperationType"/>.
    /// </summary>
    public interface ILoggerResultType 
    {
        /// <summary>
        /// Set to the number of files to process, which is displayed in footer.
        /// </summary>
        int NumberOfFilesToProcess { get; set; }

        /// <summary>
        /// Message to write to the internal logger, with the result (true/false).
        /// </summary>
        /// <param name="message">Messagge to store.</param>
        /// <param name="result">Result to store (true/false)</param>
        void WriteMessageType(string message, bool result) ;
    }
}