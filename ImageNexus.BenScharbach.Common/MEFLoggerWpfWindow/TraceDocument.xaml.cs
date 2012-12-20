using System.Windows;
using System.Windows.Documents;
using System.Diagnostics;
using System.Windows.Threading;
using ImageNexus.BenScharbach.Common.AttributesCodeInjector.AttsThreading;
using ImageNexus.BenScharbach.Common.MEFLogger.LogWindowParts;
using ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Interfaces;

namespace ImageNexus.BenScharbach.Common.MEFLoggerWpfWindow
{
    public partial class TraceDocument : FlowDocument, ITraceTextSink
    {
        private Paragraph _current;

        // 7/4/2011 - Unique DocumentId
        private int _documentId = 1;

        // 7/4/2011 - Event Lock
        private readonly object _eventLock = new object();

        #region Properties

        public bool AutoAttach { get; set; }

        /// <summary>
        /// Set to a Unique DocumentId
        /// </summary>
        public int DocumentId
        {
            get { return _documentId; }
            set { _documentId = value; }
        }

        #endregion

        /// <summary>
        /// constructor
        /// </summary>
        public TraceDocument()
        {
            AutoAttach = true;
            InitializeComponent();
        }

        [OnGuiThread(DispatcherPriority.Normal, Asynchronous = true)]
        public void Event(string msg, TraceEventType eventType)
        {
            // 7/17/2011
            WriteEventMessage(msg, eventType);
        }

        // 7/17/2011
        [OnGuiThread(DispatcherPriority.Normal, Asynchronous = true)]
        public void TraceEvent(string source, TraceEventType eventType, int id, string message)
        {
            var messageToPass = string.Concat(source, "-", id, ":", message);
            WriteEventMessage(messageToPass, eventType);
        }

        // 7/17/2011
        [OnGuiThread(DispatcherPriority.Normal, Asynchronous = false)]
        private void WriteEventMessage(string msg, TraceEventType eventType)
        {
            lock (_eventLock) // 7/4/2011
            {
                if (_current == null)
                    AddParagraph(msg, eventType);
                else
                    Append(msg);
            }
        }

        [OnGuiThread(DispatcherPriority.Normal, Asynchronous = true)]
        public void Fail(string msg)
        {
            AddParagraph(msg, TraceEventType.Error);
        }

        [OnGuiThread(DispatcherPriority.Normal, Asynchronous = false)]
        private void Append(string msg)
        {
            _current.Inlines.Add(new Run(msg.TrimEnd('\n')));
            _current = msg.EndsWith("\n") ? null : _current;
         
        }

        /// <summary>
        /// Helper method which writes a paragragh for the specific style type; if none given,
        /// the 'Information' style type is used.
        /// </summary>
        /// <param name="msg">Message to display</param>
        /// <param name="traceEventType"></param>
        [OnGuiThread(DispatcherPriority.Normal, Asynchronous = false)]
        private void AddParagraph(string msg, TraceEventType traceEventType)
        {
            var p = new Paragraph(new Run(msg.TrimEnd('\n')));

            // 7/18/2011 - Set Style
            SetStyle(traceEventType, p);

            Blocks.Add(p);
            _current = msg.EndsWith("\n") ? null : p;
        }

        // 7/18/2011
        /// <summary>
        /// Helper method which sets the desired WPF style.
        /// </summary>
        /// <param name="traceEventType"></param>
        /// <param name="p"></param>
        [OnGuiThread(DispatcherPriority.Normal, Asynchronous = false)]
        private void SetStyle(TraceEventType traceEventType, Paragraph p)
        {
            switch (traceEventType)
            {
                case TraceEventType.Critical:
                case TraceEventType.Error:
                    p.Style = (Style)(Resources["Error"]);
                    break;
                case TraceEventType.Suspend:
                case TraceEventType.Warning:
                    p.Style = (Style)(Resources["Warning"]);
                    break;
                case TraceEventType.Information:
                case TraceEventType.Verbose:
                case TraceEventType.Start:
                case TraceEventType.Stop:
                case TraceEventType.Resume:
                case TraceEventType.Transfer:
                    p.Style = (Style)(Resources["Information"]);
                    break;
                default:
                    p.Style = (Style)(Resources["Information"]);
                    break;
            }
        }

        private void Document_Loaded(object sender, RoutedEventArgs e)
        {
            if (!AutoAttach) return;

            // Create special TraceTextSource listener
            var listener = new TraceTextSource(this);
            var instanceId = Trace.Listeners.Add(listener);

            // 7/2/2011 - Get ILogWindowCtl ref
            var logWindowCtl = (ILogWindowCtl)LogWindowCtl.GetInstance;

            // 7/2/2011
            logWindowCtl.CreateTraceDocumentListener(DocumentId, instanceId);
        }

        private void Document_Unloaded(object sender, RoutedEventArgs e)
        {
           
        }
    }
}
