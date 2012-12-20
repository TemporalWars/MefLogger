using System.Diagnostics;
using System.Runtime.Serialization;

namespace ImageNexus.BenScharbach.Common.MEFLogger.WcfCustomListener
{
    // 7/10/2011
    /// <summary>
    /// Stores WCF messages from a instance of <see cref="WcfTraceListener"/>, used to
    /// communicate via WCF to front-end application.
    /// </summary>
    [DataContract]
    public class WcfTraceListenerMessage
    {
        /// <summary>
        /// Unique AttributeKeyName which associates to a single <see cref="TraceListener"/>
        /// </summary>
        [DataMember]
        public string AttributeKeyName { get; set; }

        /// <summary>
        /// Unique DocumentId to associate the proper TraceDocument WPF output console.
        /// </summary>
        [DataMember]
        public int DocumentId { get; set; }

        /// <summary>
        /// Simple array of string messages from the <see cref="WcfTraceListener"/>.
        /// </summary>
        [DataMember]
        public string[] Messages { get; set; }
    }
}
