using System.Diagnostics;

namespace ImageNexus.BenScharbach.Common.MEFLoggerWpfWindow
{
    public class TraceTextSource : TraceListener
    {
        public ITraceTextSink SinkToWpfWindow { get; private set; }
        private bool _fail;
        private TraceEventType _eventType = TraceEventType.Information;

        public TraceTextSource(ITraceTextSink sink)
        {
            Debug.Assert(sink != null);
            SinkToWpfWindow = sink;
        }

        public override void Fail(string message)
        {
            _fail = true;
            base.Fail(message);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            // 7/17/2011 - Call the Sink version of TraceEvent, which sends the data to Wpf window.
            SinkToWpfWindow.TraceEvent(source, eventType, id, message);

            _eventType = eventType;
            base.TraceEvent(eventCache, source, eventType, id, message);
        }

        public override void Write(string message)
        {
            if (IndentLevel > 0)
                message = message.PadLeft(IndentLevel + message.Length, '\t');

            if (_fail)
                SinkToWpfWindow.Fail(message);

            else
                SinkToWpfWindow.Event(message, _eventType);

            _fail = false;
            _eventType = TraceEventType.Information;
        }

        public override void WriteLine(string message)
        {
            Write(message + "\n");
        }
    }
}
