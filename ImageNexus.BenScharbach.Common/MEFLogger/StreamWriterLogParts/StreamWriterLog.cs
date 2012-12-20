using System;
using System.IO;
using ImageNexus.BenScharbach.Common.AttributesCodeInjector.AttsException;

namespace ImageNexus.BenScharbach.Common.MEFLogger.StreamWriterLogParts
{
    static class StreamWriterLog
    {
        private static StreamWriter _fileToSaveStream;
        private static bool _loggingInProcess;
        private readonly static string LogOutputLocation;
        private static DateTime _startLoggingDateTime;

        private static int _filesProcessSuccessfullyCounter;

        // 6/25/2011
        private static string _loggerOutputName = "StreamWriterLog.txt";

        #region Properties

        // 6/26/2011
        /// <summary>
        /// Set to the <see cref="StreamWriter"/>'s output name; default to 'SteamWriterLog.txt'.
        /// </summary>
        public static string LoggerOutputName
        {
            get { return _loggerOutputName; }
            set { _loggerOutputName = value; }
        }

        /// <summary>
        /// Set to the number of files to process.
        /// </summary>
        public static int NumberOfFilesToProcess { get; set; }

        // 6/26/2011
        /// <summary>
        /// Returns the count of the number of successfully processed files.
        /// </summary>
        public static int FilesProcessSuccessfullyCounter
        {
            get { return _filesProcessSuccessfullyCounter; }
        }

        #endregion

        /// <summary>
        /// Static constructor
        /// </summary>
        static StreamWriterLog()
        {
            // Retrieve ProgramData folder
            // Note: Old Path -> @"C:\Program Files (x86)\Image-Nexus\ICL Web Service\Logs\"
            var myProgramDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            LogOutputLocation  = string.Concat(myProgramDataPath, @"\\Image-Nexus\ICL_Service_Logs\");

            // Create folder, if doesn't exist)
            if (!Directory.Exists(LogOutputLocation))
            {
                Directory.CreateDirectory(LogOutputLocation);
            }
            
        }

        // 6/26/2011
        /// <summary>
        /// Updates the internal results counter.
        /// </summary>
        /// <param name="result">Operation result</param>
        public static void UpdateResultCounter(bool result)
        {
            _filesProcessSuccessfullyCounter = FilesProcessSuccessfullyCounter + ((result) ? 1 : 0);
        }

        // 6/26/2011
        [CatchAllExceptionHandling(false, null, false)]
        internal static void StartLogfile(string logName, string headerName = "Log Activity")
        {
            if (_loggingInProcess)
            {
                throw new InvalidOperationException("In order to start a new log file, you MUST call 'EndLogFile' to end the current logging process!");
            }

            var filePath = Path.Combine(LogOutputLocation, logName ?? _loggerOutputName);

            _fileToSaveStream = new StreamWriter(filePath);

            // Add Header to file
            _fileToSaveStream.WriteLine(string.Format("-------------------- {0} ---------------------", headerName));

            _loggingInProcess = true;
            _startLoggingDateTime = DateTime.Now;
            _filesProcessSuccessfullyCounter = 0; // 7/23/2011

        }

        // 4/9/2011
        [CatchAllExceptionHandling(false, null, false)]
        public static void AddLogEvent(string logEvent)
        {
            if (_fileToSaveStream == null)
            {
                throw new InvalidOperationException("You MUST call 'StartLogFile' before adding a log events!");
            }

            _fileToSaveStream.WriteLine(logEvent);
        }

        // 4/9/2011
        [CatchAllExceptionHandling(false, null, false)]
        public static void EndLogFile()
        {
            if (_fileToSaveStream == null) return;

            // Add Footer to file
            _fileToSaveStream.WriteLine("-------------------- End of Log Activity ---------------------");
            _fileToSaveStream.WriteLine(string.Format("Start Time [ {0} ]", _startLoggingDateTime));
            _fileToSaveStream.WriteLine(string.Format("End Time [ {0} ]", DateTime.Now));

            _fileToSaveStream.WriteLine(string.Format("Files {0} of {1}  completed operation.", _filesProcessSuccessfullyCounter, NumberOfFilesToProcess));

            _fileToSaveStream.Close();
            _fileToSaveStream.Dispose();
            _fileToSaveStream = null;
            _loggingInProcess = false;
        }

    }
}
