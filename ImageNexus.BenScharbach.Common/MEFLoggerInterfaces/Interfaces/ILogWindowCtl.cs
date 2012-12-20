
using ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Enums;

namespace ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Interfaces
{
    // 7/2/2011
    public interface ILogWindowCtl
    {
        /// <summary>
        /// Returns an instance of this singleton <see cref="ILogWindowCtl"/> class.
        /// </summary>
        ILogWindowCtl GetInstance { get; }

        /// <summary>
        /// Adds the given <paramref name="attributeKeyName"/> and <paramref name="documentId"/> association to 
        /// the internal dictionary.
        /// </summary>
        /// <param name="attributeKeyName">Unique AttributeKeyName</param>
        /// <param name="documentId">Unique DocumentId to associate the proper TraceDocument WPF output console.</param>
        void SetTraceDocumentKey(string attributeKeyName, int documentId);

        // 7/2/2011
        /// <summary>
        /// Associates the given <paramref name="listenerId"/> with a 'TraceDocument' and returns a unique
        /// DocumentId.
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="listenerId">Unique Listener ID</param>
        void CreateTraceDocumentListener(int documentId, int listenerId);

        /// <summary>
        /// Writes some message into a WPF TraceDocument window.
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="message">Message to write.</param>
        /// <param name="attributeKeyName">Unique AttributeKeyName</param>
        void WriteMessage(MessageTypeEnum messageType, string message, string attributeKeyName);

      
    }
}