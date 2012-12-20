namespace ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Interfaces
{
    // 7/2/2011
    /// <summary>
    /// The <see cref="ILoggerLogWindowType"/> is used to add additional interface channels, beyond the
    /// standard <see cref="ILoggerOperationType"/>.
    /// </summary>
    public interface ILoggerLogWindowType 
    {
        /// <summary>
        /// Unique AttributeKeyName.
        /// </summary>
        string AttributeKeyName { get; set; }

        // 7/10/2011
        /// <summary>
        /// Set when attribute is applied to some assembly which is running
        /// in the context of a WCF service.
        /// </summary>
        bool IsRunningInWcfService { get; set; }
    }
}