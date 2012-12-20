using System.IO;

namespace ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Interfaces
{
    // 6/26/2011
    /// <summary>
    /// The <see cref="ILoggerStreamWriterType"/> is used to add additional interface channels, beyond the
    /// standard <see cref="ILoggerOperationType"/>.
    /// </summary>
    public interface ILoggerStreamWriterType
    {
        /// <summary>
        /// Starts the <see cref="StreamWriter"/> log file.
        /// </summary>
        /// <param name="logName">LogName to use for output file.</param>
        /// <param name="headerName">(Optional) HeaderName to use - defaults to 'Log Activity'</param>
        void StartStreamWriterLogfile(string logName, string headerName = "Log Activity");

        /// <summary>
        /// Closes an open <see cref="StreamWriter"/> log file.
        /// </summary>
        void EndStreamWriterLogFile();
    }
}