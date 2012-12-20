using System.ComponentModel.Composition;
using System.Diagnostics;

namespace ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Interfaces
{
    /// <summary>
    /// The <see cref="ILoggerCompositionContainer"/> interface exposes the <see cref="ILogger"/>.
    /// </summary>
    /// <returns>The ID for the created <see cref="TraceListener"/></returns>
    public interface ILoggerCompositionContainer
    {
        [Import(typeof (ILogger))]
        ILogger Logger { get; set; }
    }
}