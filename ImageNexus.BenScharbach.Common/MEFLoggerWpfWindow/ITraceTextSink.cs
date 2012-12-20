using System.Diagnostics;

namespace ImageNexus.BenScharbach.Common.MEFLoggerWpfWindow
{
    public interface ITraceTextSink
	{
		void Fail(string msg);
		void Event(string msg, TraceEventType eventType);
        // 7/17/2011 - Handles the TraceEvent.
        void TraceEvent(string source, TraceEventType eventType, int id, string message);
	}
}
