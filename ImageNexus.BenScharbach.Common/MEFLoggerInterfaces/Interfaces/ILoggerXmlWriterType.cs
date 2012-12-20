using System.IO;

namespace ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Interfaces
{
    // 3/23/2011
    /// <summary>
    /// The <see cref="ILoggerXmlWriterType"/> is used to add additional interface channels, beyond the
    /// standard <see cref="ILoggerOperationType"/>.
    /// </summary>
    public interface ILoggerXmlWriterType
    {
        /// <summary>
        /// Starts the <see cref="StreamWriter"/> log file.
        /// </summary>
        /// <param name="logName">LogName to use for output file.</param>
        void StartXmlWriterLogfile<TData>(string logName);

        /// <summary>
        /// Closes an open <see cref="StreamWriter"/> log file.
        /// </summary>
        void EndXmlWriterLogFile<TData>();
    }
}