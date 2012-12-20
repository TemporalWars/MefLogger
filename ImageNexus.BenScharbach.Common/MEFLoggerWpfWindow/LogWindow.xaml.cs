using System;
using System.Threading;
using System.Windows;

namespace ImageNexus.BenScharbach.Common.MEFLoggerWpfWindow
{
	/// <summary>
	/// Interaction logic for LogWindow.xaml
	/// </summary>
	public partial class LogWindow : Window
	{
        private static Timer _timerPollService;
	    private static int _periodBetweenCalls = 3000;

	    #region Properties

        public static Action WcfTimerCallback { get; set; }

        // 7/18/2011
        /// <summary>
        /// Main application sets to 'True' when shutting down.  This allows
        /// this window to live throughout the application cycle, regardless if it
        /// hidden from view.
        /// </summary>
        public static bool KillWindow { get; set; }

        /// <summary>
        /// Sets the time between Timer poll calls, set in Milliseconds.
        /// </summary>
        /// <remarks>3000 is the default</remarks>
	    public static int PeriodBetweenCalls
	    {
	        get { return _periodBetweenCalls; }
	        set { _periodBetweenCalls = value; }
	    }

	    #endregion

        /// <summary>
        /// Constructor
        /// </summary>
		public LogWindow()
		{
			InitializeComponent();

            // Create timer for polling WCF Listener messages.
            _timerPollService = new Timer(OnTimerCallback, this, 500, PeriodBetweenCalls);
		}
		
         // 7/10/2011 - // NOTE: Avoid Anonymous callbacks with timer.
        private static void OnTimerCallback(object state)
        {
            if (WcfTimerCallback != null)
                WcfTimerCallback();
        }

        // 7/18/2011 - Captures the Closing event.
        private void WpfLogger_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Check if to kill window or hide it instead.
            if (KillWindow) return;
            e.Cancel = true;
            Hide();
        }
	}
}
