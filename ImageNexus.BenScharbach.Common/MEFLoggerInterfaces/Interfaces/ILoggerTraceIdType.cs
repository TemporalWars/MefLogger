using System.Diagnostics;

namespace ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Interfaces
{
    // 6/26/2011
    /// <summary>
    /// The <see cref="ILoggerTraceIdType"/> is used to add additional interface channels, beyond the
    /// standard <see cref="ILoggerOperationType"/>.
    /// </summary>
    public interface ILoggerTraceIdType 
    {
        /// <summary>
        /// The ID for the created <see cref="TraceListener"/>.
        /// </summary>
        int TraceListenerInstanceId { get; set; }
      
    }
}