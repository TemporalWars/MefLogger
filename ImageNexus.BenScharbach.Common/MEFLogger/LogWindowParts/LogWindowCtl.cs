using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using ImageNexus.BenScharbach.Common.MEFLogger.TraceLogParts;
using ImageNexus.BenScharbach.Common.MEFLogger.WcfCustomListener;
using ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Enums;
using ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Interfaces;

namespace ImageNexus.BenScharbach.Common.MEFLogger.LogWindowParts
{
    // 7/2/2011
    public class LogWindowCtl : ILogWindowCtl
    {
        // singleton instance
        private static readonly LogWindowCtl LogWindowControlInstance = new LogWindowCtl();

        // Note: Key is the 'AttributeName' given by the developer in the 'Property' of the MEFAttribute.
        // Note: Value is the 'Document' WPF instance this is associated with.
        private static readonly Dictionary<string, int> AttributeKeyNameToDocId = new Dictionary<string, int>();

        // Note: Key is the 'DocumentId' Wpf window to associate.
        // Note: Value is the 'ListenerId' instance this is associated with.
        private static readonly Dictionary<int, int> DocIdToListenerId = new Dictionary<int, int>();

        #region Properties

        /// <summary>
        /// Returns an instance of this singleton <see cref="LogWindowCtl"/> class.
        /// </summary>
        ILogWindowCtl ILogWindowCtl.GetInstance
        {
            get { return GetInstance; }
        }

        /// <summary>
        /// Returns an instance of this singleton <see cref="LogWindowCtl"/> class.
        /// </summary>
        public static LogWindowCtl GetInstance
        {
            get
            {
                return LogWindowControlInstance;
            }
        }

        #endregion

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static LogWindowCtl()
        {
        }

        // 7/2/2011
        /// <summary>
        /// Adds the given <paramref name="attributeKeyName"/> and <paramref name="documentId"/> association to 
        /// the internal dictionary.
        /// </summary>
        /// <param name="attributeKeyName">Unique AttributeKeyName</param>
        /// <param name="documentId">Unique DocumentId to associate the proper TraceDocument WPF output console.</param>
        void ILogWindowCtl.SetTraceDocumentKey(string attributeKeyName, int documentId)
        {
            SetTraceDocumentKey(attributeKeyName, documentId);
        }

        /// <summary>
        /// Adds the given <param name="attributeKeyName"></param> and <param name="documentId"></param> association to 
        /// the internal dictionary.
        /// </summary>
        /// <param name="attributeKeyName">Unique AttributeKeyName</param>
        /// <param name="documentId">Unique DocumentId to associate the proper TraceDocument WPF output console.</param>
        private static void SetTraceDocumentKey(string attributeKeyName, int documentId)
        {
            if (string.IsNullOrEmpty(attributeKeyName))
                throw new ArgumentNullException("attributeKeyName");

            if (documentId < 0)
                throw new ArgumentOutOfRangeException("documentId");

            if (AttributeKeyNameToDocId.ContainsKey(attributeKeyName))
                AttributeKeyNameToDocId[attributeKeyName] = documentId;
            else
            {
                AttributeKeyNameToDocId.Add(attributeKeyName, documentId);
            }

        }

        /// <summary>
        /// Associates the given <paramref name="listenerId"/> with a 'TraceDocument' and returns a unique
        /// DocumentId.
        /// </summary>
        /// <param name="documentId">Unique DocumentId Key to associate the proper TraceDocument WPF output console.</param>
        /// <param name="listenerId">Unique Listener ID</param>
        void ILogWindowCtl.CreateTraceDocumentListener(int documentId, int listenerId)
        {
            CreateTraceDocumentListener(documentId, listenerId);
        }

        /// <summary>
        /// Creates a TraceTextSource listener and returns a unique
        /// DocumentId.
        /// </summary>
        /// <param name="documentId">Unique DocumentId Key to associate the proper TraceDocument WPF output console.</param>
        /// <param name="listenerId">Unique Listener ID</param>
        private static void CreateTraceDocumentListener(int documentId, int listenerId)
        {
            // Add to Dictionary
            if (DocIdToListenerId.ContainsKey(documentId))
                DocIdToListenerId[documentId] = listenerId;
            else
            {
                DocIdToListenerId.Add(documentId, listenerId);
            }
        }

        // 7/10/2011
        /// <summary>
        /// Creates a WcfTraceListener.
        /// </summary>
        /// <param name="attributeKeyName">Unique AttributeKeyName</param>
        /// <param name="documentId">Unique DocumentId Key to associate the proper TraceDocument WPF output console.</param>
        public static void CreateWcfTraceDocumentListener(string attributeKeyName, int documentId)
        {
            if (string.IsNullOrEmpty(attributeKeyName))
                throw new ArgumentNullException("attributeKeyName");

            if (AttributeKeyNameToDocId.ContainsKey(attributeKeyName))
                return;

            // Add AttributeKeyName
            AttributeKeyNameToDocId.Add(attributeKeyName, documentId);

            // Create WcfTraceListener
            var wcfTraceListener = new WcfTraceListener { AttributeKeyName = attributeKeyName, DocumentId = documentId };
            var listenerId = Trace.Listeners.Add(wcfTraceListener);

            // Add to Dictionary
            if (DocIdToListenerId.ContainsKey(documentId))
                DocIdToListenerId[documentId] = listenerId;
            else
            {
                DocIdToListenerId.Add(documentId, listenerId);
            }
        }

        // 7/17/2011 - Updated with new param 'MessageTypeEnum'
        /// <summary>
        /// Writes some message into a WPF TraceDocument window.
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="message">Message to write.</param>
        /// <param name="attributeKeyName">Unique AttributeKeyName</param>
        void ILogWindowCtl.WriteMessage(MessageTypeEnum messageType, string message, string attributeKeyName)
        {
            WriteMessage(messageType, message, attributeKeyName);
        }

        /// <summary>
        /// Writes some message into a WPF TraceDocument window.
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="message">Message to write.</param>
        /// <param name="attributeKeyName">Unique AttributeKeyName</param>
        private static void WriteMessage(MessageTypeEnum messageType, string message, string attributeKeyName)
        {
            if (string.IsNullOrEmpty(attributeKeyName))
                throw new ArgumentNullException("attributeKeyName");

            if (!AttributeKeyNameToDocId.ContainsKey(attributeKeyName))
                return;

            // Retrieve the DocumentId association
            var documentId = AttributeKeyNameToDocId[attributeKeyName];
            
            // 7/4/2011 - Verify Listener instance exist
            if (!DocIdToListenerId.ContainsKey(documentId)) return;
            
            // Retrieve the ListenerId association
            var traceListenerInstanceId = DocIdToListenerId[documentId];

            // 7/27/2011 - Writes the message
            // Note: When running in the service domain, OnGui calls is not necessary; hence the check.
            if (Logger.IsRunningInWcfService)
                TraceLogCtl.WriteMessage(messageType, traceListenerInstanceId, message);
            else
            {
                // 7/17/2011 - Writes the message using the OnGui attribute.
                WriteMessage(messageType, traceListenerInstanceId, message);
            }
                
        }

        // 7/10/2011
        // Note: This method is called from the MainWindow's polling thread.
        /// <summary>
        /// Writes some message into a WPF TraceDocument window.
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="message">Message to write.</param>
        /// <param name="documentId">Unique DocumentId Key to associate the proper TraceDocument WPF output console.</param>
        public static void WriteMessageFromWcf(MessageTypeEnum messageType, string message, int documentId)
        {
            // 7/4/2011 - Verify Listener instance exist
            if (!DocIdToListenerId.ContainsKey(documentId)) return;

            // Retrieve the ListenerId association
            var traceListenerInstanceId = DocIdToListenerId[documentId];

            // 7/17/2011 - Writes the message using the OnGui attribute.
            WriteMessage(messageType, traceListenerInstanceId, message);
        }

        // Note: Since messages can propegate from some other calling THREAD, other than the UI Thread,
        // Note: going to use the 'Dispatcher' UI lob to guaranttee is works, regardless of performance hits.
        // 7/17/2011
        /// <summary>
        /// Helper method which writes a message to the proper <see cref="TraceListener"/>.
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="traceListenerInstanceId"><see cref="TraceListener"/> array index to use.</param>
        /// <param name="message">Message to log</param>
        internal static void WriteMessage(MessageTypeEnum messageType, int traceListenerInstanceId, string message)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Send,
                                                      new ThreadStart(delegate
                                                                          {
                                                                              TraceLogCtl.WriteMessage(messageType, traceListenerInstanceId,
                                                                                  message);
                                                                          }));

        }

        // 7/10/2011
        // NOTE: Called ONLY in WCF Service.
        /// <summary>
        /// Returns a batch of <see cref="WcfTraceListenerMessage"/> ready for WCF transport.
        /// </summary>
        /// <param name="wcfMessages">Array of <see cref="WcfTraceListenerMessage"/>.</param>
        public static void GetBatchOfWcfMessages(out WcfTraceListenerMessage[] wcfMessages)
        {
            var messages = new List<WcfTraceListenerMessage>();
            var listeners = Trace.Listeners;

            // iterate listeners to pump for messages
            var count = listeners.Count;
            for (var i = 0; i < count; i++)
            {
                var listener = listeners[i];
                var wcfListener = listener as WcfTraceListener;
                if (wcfListener == null) continue;

                // Pump for messages
                wcfListener.GetBatchOfMessages(ref messages);
            }

            // Pass back results as simple array for WCF communication.
            wcfMessages = new WcfTraceListenerMessage[messages.Count];
            messages.CopyTo(wcfMessages, 0);
        }

        public static void Dispose()
        {
            var count = Trace.Listeners.Count;
            for (var index = 0; index < count; index++)
            {
                var listener = Trace.Listeners[index];
               
                listener.Flush();
                listener.Close();
                listener.Dispose();
            }

            //Trace.Listeners.Clear();
        }
    }
}
