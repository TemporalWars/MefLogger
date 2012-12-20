using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace ImageNexus.BenScharbach.Common.MEFLogger.WcfCustomListener
{
    // 7/10/2011
    /// <summary>
    /// The <see cref="WcfTraceListener"/> is used to save <see cref="TraceListener"/> messages into an internal concurrent queue.  This
    /// data is then retrieved from the 'FrontEnd' application periodically and populated to the WPF output window.
    /// </summary>
    class WcfTraceListener : TraceListener
    {
        private readonly ConcurrentQueue<string> _wcfMessages = new ConcurrentQueue<string>();

        // Defaults to 10 for batch Get quantity
        private int _batchGetQuantity = 10;

        #region Properties

        /// <summary>
        /// Unique AttributeKeyName which associates to a single <see cref="TraceListener"/>
        /// </summary>
        public string AttributeKeyName { get; set; }

        /// <summary>
        /// Unique DocumentId to associate the proper TraceDocument WPF output console.
        /// </summary>
        public int DocumentId { get; set; }

        /// <summary>
        /// Set your 'Get' batch quantity.
        /// </summary>
        /// <remarks>Default is 10.</remarks>
        public int BatchGetQuantity
        {
            get { return _batchGetQuantity; }
            set { _batchGetQuantity = value; }
        }

        #endregion

        /// <summary>
        /// When overridden in a derived class, writes the specified message to the listener you create in the derived class.
        /// </summary>
        /// <param name="message">A message to write. </param><filterpriority>2</filterpriority>
        public override void Write(string message)
        {
           // Add message to Queue
            _wcfMessages.Enqueue(message);
        }

        /// <summary>
        /// When overridden in a derived class, writes a message to the listener you create in the derived class, followed by a line terminator.
        /// </summary>
        /// <param name="message">A message to write. </param><filterpriority>2</filterpriority>
        public override void WriteLine(string message)
        {
            // Add message to Queue
            _wcfMessages.Enqueue(message);
        }

        /// <summary>
        /// Retrieves a batch of messages from the current instance and populates
        /// the result into a <see cref="WcfTraceListenerMessage"/>.
        /// </summary>
        /// <param name="wcfMessages">Collection used to add a <see cref="WcfTraceListenerMessage"/>.</param>
        public void GetBatchOfMessages(ref List<WcfTraceListenerMessage> wcfMessages)
        {
            var messages = new List<string>();

            // Dequeue some messages
            for (var i = 0; i < _batchGetQuantity; i++)
            {
                string message;
                if (_wcfMessages.TryDequeue(out message))
                {
                    messages.Add(message);
                }
            }

            // Create new wcfMessage
            var wcfMessage = new WcfTraceListenerMessage()
                                 {
                                     AttributeKeyName = AttributeKeyName,
                                     DocumentId = DocumentId,
                                     Messages = messages.ToArray()
                                 };

            // Add to output
            wcfMessages.Add(wcfMessage);

        }

    }
}
