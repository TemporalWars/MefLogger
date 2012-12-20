using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using ImageNexus.BenScharbach.Common.MEFLogger.LogWindowParts;
using ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Interfaces;

namespace ImageNexus.BenScharbach.Common.MEFLoggerWpfWindow
{
	/// <summary>
	/// Interaction logic for TraceTextBox.xaml
	/// </summary>
	public partial class TraceTextBoxControl : UserControl, ITraceTextSink
	{
        // 7/4/2011 - Unique DocumentId
        private const int DocumentId = 2;

		public TraceTextBoxControl()
		{
            AutoAttach = true;
			InitializeComponent();
		}

        public bool AutoAttach { get; set; }

		public void Event(string msg, TraceEventType eventType)
		{
            lock (textBox1) // 7/4/2011
            {
                textBox1.AppendText(msg);
                textBox1.ScrollToEnd();
            }
		}

	    public void TraceEvent(string source, TraceEventType eventType, int id, string message)
	    {
            var messageToPass = string.Concat(source, "-", id, ":", message);
            lock (textBox1) // 7/4/2011
            {
                textBox1.AppendText(messageToPass);
                textBox1.ScrollToEnd();
            }

	    }

	    public void Fail(string msg)
		{
            lock (textBox1) // 7/4/2011
            {
                textBox1.AppendText(msg);
                textBox1.ScrollToEnd();
            }
		}		

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
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
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }
	}
}
