using System;
using System.IO;
using System.Xml.Serialization;
using ImageNexus.BenScharbach.Common.AttributesCodeInjector.AttsException;
using ImageNexus.BenScharbach.Common.MEFLogger.XmlWriterLogParts.Xml;

namespace ImageNexus.BenScharbach.Common.MEFLogger.XmlWriterLogParts
{
    // 3/23/2012
    /// <summary>
    /// Xml Log writer.
    /// </summary>
    static class XmlWriterLog<TData>
    {
        private static FileStream _fileToSaveStream;
        private static bool _loggingInProcess;
        private readonly static string LogOutputLocation;
        private static DateTime _startLoggingDateTime;
        private static int _filesProcessSuccessfullyCounter;
        private static string _loggerOutputName = "XmlWriterLog.txt";
        private static XmlSerializer _serializer;
        private static XmlWrapper<TData> _xmlWrapper;

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
        static XmlWriterLog()
        {
            // Retrieve ProgramData folder
            // Note: Old Path -> @"C:\Program Files (x86)\Image-Nexus\ICL Web Service\Logs\"
            var myProgramDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            LogOutputLocation  = string.Concat(myProgramDataPath, @"\\Image-Nexus\ICL_Service_Logs\XmlLogs\");

            // Create folder, if doesn't exist)
            if (!Directory.Exists(LogOutputLocation))
            {
                Directory.CreateDirectory(LogOutputLocation);
            }
        }

        
        /// <summary>
        /// Updates the internal results counter.
        /// </summary>
        /// <param name="result">Operation result</param>
        public static void UpdateResultCounter(bool result)
        {
            _filesProcessSuccessfullyCounter = FilesProcessSuccessfullyCounter + ((result) ? 1 : 0);
        }

        /// <summary>
        /// Starts the creation of an XML log file.
        /// </summary>
        /// <param name="logName"></param>
        [OnCatchAllExceptionHandling(false, null, false)]
        internal static void StartXmlLogfile(string logName)
        {
            if (_loggingInProcess)
            {
                throw new InvalidOperationException("In order to start a new log file, you MUST call 'EndLogFile' to end the current logging process!");
            }

            var filePath = Path.Combine(LogOutputLocation, logName ?? _loggerOutputName);

            // Create XML Wrapper
            _xmlWrapper = new XmlWrapper<TData>();

            // Create Xml Serializer instance from provided data generic type.
            _serializer = new XmlSerializer(typeof(XmlWrapper<TData>));

            // Open file stream.
            _fileToSaveStream = File.Open(filePath, FileMode.Create);

            _loggingInProcess = true;
            _startLoggingDateTime = DateTime.Now;
            _filesProcessSuccessfullyCounter = 0;
        }

        
        [OnCatchAllExceptionHandling(false, null, false)]
        public static void AddXmlLogEvent(TData data)
        {
            if (_fileToSaveStream == null)
            {
                throw new InvalidOperationException("You MUST call 'StartLogFile' before adding a log events!");
            }

            // Add new item to the xmlWrapper collection
            _xmlWrapper.CollectionToSerialize.Add(data);
            
        }

       
        [OnCatchAllExceptionHandling(false, null, false)]
        public static void EndXmlLogFile()
        {
            if (_fileToSaveStream == null) return;

            // Write out serialized xmlWrapper data.
            _serializer.Serialize(_fileToSaveStream, _xmlWrapper);

            // Add Footer to file
            //_fileToSaveStream.WriteLine("-------------------- End of Log Activity ---------------------");
            //_fileToSaveStream.WriteLine(string.Format("Start Time [ {0} ]", _startLoggingDateTime));
            //_fileToSaveStream.WriteLine(string.Format("End Time [ {0} ]", DateTime.Now));
            //_fileToSaveStream.WriteLine(string.Format("Files {0} of {1}  completed operation.", _filesProcessSuccessfullyCounter, NumberOfFilesToProcess));

            _fileToSaveStream.Close();
            _fileToSaveStream.Dispose();
            _fileToSaveStream = null;
            _loggingInProcess = false;
        }
    }
}
