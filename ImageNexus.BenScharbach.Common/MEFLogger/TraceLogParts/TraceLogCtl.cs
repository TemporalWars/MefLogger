using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using ImageNexus.BenScharbach.Common.MEFLogger.TaskActions;
using ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Enums;

namespace ImageNexus.BenScharbach.Common.MEFLogger.TraceLogParts
{
    public static class TraceLogCtl
    {
        // 7/9/2011 - Exceptions List used to track when a file name was append a GUID string.
        private static Dictionary<string, string> _loggerOutputPathExceptionMappings = new Dictionary<string, string>();

        // 7/24/2011
        private static string _traceLogsRunTimeStamp;
        private static string _traceLogsRunTimeFolderName;
        private static readonly string LogPathAtProgramData; // 7/25/2011
        private static string _textFilePathName;
        private static readonly bool IsService; // 7/25/2011

        // 10/2/2011 - Create Concurrent WriteMessage task
        private static readonly DoWriteMessagesTask _writeMessagesTask;

        // 7/25/2011
        /// <summary>
        /// Constructor
        /// </summary>
        static TraceLogCtl()
        {
            // Retrieve ProgramData folder
            // Note: Old Path -> @"C:\Program Files (x86)\Image-Nexus\ICL Web Service\Logs\LoggerCallsOutput.txt"
            var myProgramDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            LogPathAtProgramData = String.Concat(myProgramDataPath, @"\\Image-Nexus\");

            // 7/25/2011 - Checks if the current process is running within the context of a windows service.
            IsService = Logger.IsService((uint)Process.GetCurrentProcess().Id);

            // 10/2/2011 - Create Task which checks if ConcurrentQueue has items to process.
           _writeMessagesTask = new DoWriteMessagesTask();
        }

        /// <summary>
        /// Creates an instance of a <see cref="TextWriterTraceListener"/>.
        /// </summary>
        /// <param name="loggerOutputPath">Relative path to use within the 'ProgramData\Image-Nexus' folder.</param>
        /// <returns>The ID for the created <see cref="TraceListener"/></returns>
        public static int CreateTextTraceListener(string loggerOutputPath = "ICL_Trace_Logs\\TraceLogCallsOutput.txt")
        {
            // Split
            string fileName;
            var pathOnly = GetPathOnly(loggerOutputPath, out fileName);

            // 7/25/2011
            if (IsService)
                fileName = string.Concat("Srv_", fileName);

            // 7/24/2011 - create folder name
           CreateTimeStampTraceListenerFolderName();

            // 7/24/2011 - concatenate timeStamp folder
            var pathOnlyWithTimeStamp = string.Concat(pathOnly, _traceLogsRunTimeFolderName);
           
            // Create FullPath in the 'ProgramData' folder.
            var fullPath = CreateLoggerFullPath(fileName, pathOnlyWithTimeStamp);

            // 7/9/2011 - Do update check for exception path.
            CheckForPathException(ref fullPath);

            int traceListenerInstanceId;

            // 6/28/2011 - Check if already exist
            var traceInstance = Trace.Listeners[fullPath];
            if (traceInstance != null)
            {
                // Then return current traceListenerInstanceId
                traceListenerInstanceId = Trace.Listeners.IndexOf(traceInstance);
            }
            else
            {
                // Create a file for output named TestFile.txt.
                Stream loggerFile = CreateLoggerFile(ref fullPath, loggerOutputPath);
                traceListenerInstanceId = Trace.Listeners.Add(new TextWriterTraceListener(loggerFile)
                {
                    TraceOutputOptions =
                        TraceOptions.DateTime | TraceOptions.ProcessId |
                        TraceOptions.ThreadId,
                    Name = fullPath // 6/28/2011
                });
            }

            Trace.AutoFlush = true;
            return traceListenerInstanceId;
        }

        // 7/24/2011
        /// <summary>
        /// Helper method which creates the <see cref="TraceListener"/> output folder name.
        /// </summary>
        private static void CreateTimeStampTraceListenerFolderName()
        {
            // check if timeStamp file exist
            _textFilePathName = string.Concat(LogPathAtProgramData, @"\traceLogRunTimeStamp.txt");
            if (File.Exists(_textFilePathName))
            {
                _traceLogsRunTimeFolderName = ReadTimeStampFile(_textFilePathName);

                // check if valid data within file.
                if (_traceLogsRunTimeFolderName.StartsWith("RunAt_")) return;
            }

            // Create DateTime string for Logger
            _traceLogsRunTimeStamp = string.Concat(DateTime.Now.Month, "_", DateTime.Now.Day, "_", DateTime.Now.Year,
                                                   "_", DateTime.Now.Hour, "_", DateTime.Now.Minute);

            // Create 'RunDate' folder name
            _traceLogsRunTimeFolderName = String.Concat("RunAt_", _traceLogsRunTimeStamp);

            // Save to text file.
            CreateTimeStampFile(_textFilePathName);
        }

        // 7/25/2011
        /// <summary>
        /// Helper method which saves the <see cref="_traceLogsRunTimeFolderName"/>.
        /// </summary>
        private static void CreateTimeStampFile(string textFilePath)
        {
            if (string.IsNullOrEmpty(textFilePath))
                throw new ArgumentNullException("textFilePath");
            try
            {
                // Delete old file, if still exist
                if (File.Exists(textFilePath))
                    File.Delete(textFilePath);

                // Save name to textfile at base
                using (var outfile = new StreamWriter(textFilePath))
                {
                    outfile.Write(_traceLogsRunTimeFolderName);
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("The file could not be created.");
                Console.WriteLine(e.Message);
            }
        }

        // 7/26/2011
        /// <summary>
        /// Deletes the 'traceLogRunTimeStamp.txt', if it exist.
        /// </summary>
        internal static void DoStartCheckIfNeedsDeletion()
        {
            try
            {
                var textFilePathName = string.Concat(LogPathAtProgramData, @"\traceLogRunTimeStamp.txt");

                // Delete old file, if still exist
                if ((File.Exists(textFilePathName)))
                    File.Delete(textFilePathName);
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("The file could not be deleted.");
                Console.WriteLine(e.Message);
            }
            
        }

        // 7/25/2011
        /// <summary>
        /// Helper method which reads in the <see cref="_traceLogsRunTimeFolderName"/>.
        /// </summary>
        private static string ReadTimeStampFile(string fullFilePathName)
        {
            if (string.IsNullOrEmpty(fullFilePathName))
                throw new ArgumentNullException("fullFilePathName");

            try
            {
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                using (var sr = new StreamReader(fullFilePathName))
                {
                    // Read and display lines from the file until the end of
                    // the file is reached.
                    return sr.ReadLine();
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

            return null;
        }
       

        // 7/9/2011
        /// <summary>
        /// When the calling program closes, this method should be called to flush and close all open
        /// TraceListeners.  The service will then recreate new listeners to replace the old ones.
        /// </summary>
        internal static void FlushAndCloseAllTraceListeners()
        {
            // retrieve listeners
            var listeners = new TraceListener[Trace.Listeners.Count];
            Trace.Listeners.CopyTo(listeners, 0);

            // Iterate collection
            var count = listeners.Length;
            for (var index = 0; index < count; index++)
            {
                var listener = listeners[index];

                // Skip the Default listener
                if (listener is DefaultTraceListener)
                    continue;

                // Flush/Close current listener
                listener.Flush();
                listener.Close();
                listener.Dispose();
                listeners[index] = null;
               
            }

            // Clear old TraceListeners 
            Trace.Listeners.Clear();

            // 7/24/2011 - Reset values for service
            _traceLogsRunTimeStamp = null;
            _traceLogsRunTimeFolderName = null;
          
        }

        /// <summary>
        /// Helper method which takes a relative <paramref name="pathOnly"/> and <paramref name="fileName"/>, and
        /// create a fullPath within the 'ProgramData\Image-Nexus' folder.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="pathOnly">(Optional) relative path to concatenate to the full 'ProgramData\Image-Nexus' folder.</param>
        /// <returns>FullPath at the 'ProgramData\Image-Nexus' folder.</returns>
        private static string CreateLoggerFullPath(string fileName, string pathOnly = "")
        {
            // Concatenate pathOnly
            var logPath = string.Concat(LogPathAtProgramData, pathOnly);

            // Create Image-Nexus folder, if doesn't exist)
            if (!Directory.Exists(logPath))
                Directory.CreateDirectory(logPath);

            return Path.Combine(logPath, fileName);
        }

        // 7/9/2011
        /// <summary>
        /// Helper method which creates a text file for logging.
        /// </summary>
        /// <param name="filePath">Full path to logger text file.</param>
        /// <param name="loggerOutputPath">Relative path to use within the 'ProgramData\Image-Nexus' folder.</param>
        /// <returns>Instance of <see cref="Stream"/>.</returns>
        private static Stream CreateLoggerFile(ref string filePath, string loggerOutputPath)
        {
            try
            {
                return File.Create(filePath);
            }
            catch (IOException exp)
            {
                // Split
                string fileName;
                var pathOnly = GetPathOnly(loggerOutputPath, out fileName);

                var extension = new FileInfo(filePath).Extension;
                var fileNameNoExt = fileName.Substring(0, fileName.Length - extension.Length);
                var newFileName = String.Concat(fileNameNoExt, Guid.NewGuid().ToString(), extension);

                // Create FullPath in the 'ProgramData' folder.
                var newFilePath = CreateLoggerFullPath(newFileName, pathOnly);

                // Add to Exceptions Dictionary
                AddLoggerPathException(filePath, newFilePath);

                // Update calling FilePath with new path.
                filePath = newFilePath;

                return File.Create(newFilePath);
            }
            // 7/19/2011
            catch (UnauthorizedAccessException exp)
            {
                MessageBox.Show("The application MUST be started in Administrator Mode!", "File Access Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            return null;
        }

        /// <summary>
        /// Helper method which adds a new loggerOutput exception entry into the internal
        /// <see cref="_loggerOutputPathExceptionMappings"/>.
        /// </summary>
        /// <param name="fullPath">Original file path.</param>
        /// <param name="newFilePath">New file path assocation.</param>
        private static void AddLoggerPathException(string fullPath, string newFilePath)
        {
            if (_loggerOutputPathExceptionMappings == null)
                _loggerOutputPathExceptionMappings = new Dictionary<string, string>();

            if (String.IsNullOrEmpty(fullPath))
                throw new ArgumentNullException("fullPath");

            if (String.IsNullOrEmpty(newFilePath))
                throw new ArgumentNullException("newFilePath");

            if (_loggerOutputPathExceptionMappings.ContainsKey(fullPath))
            {
                _loggerOutputPathExceptionMappings[fullPath] = newFilePath;
            }
            else
            {
                _loggerOutputPathExceptionMappings.Add(fullPath, newFilePath);
            }
        }

        /// <summary>
        /// Helper method which checks if a associated exception path is required.
        /// </summary>
        /// <param name="fullPath">FullPath to check and update if required.</param>
        private static void CheckForPathException(ref string fullPath)
        {
            if (_loggerOutputPathExceptionMappings == null)
                _loggerOutputPathExceptionMappings = new Dictionary<string, string>();

            if (_loggerOutputPathExceptionMappings.ContainsKey(fullPath))
                fullPath = _loggerOutputPathExceptionMappings[fullPath];
        }

        /// <summary>
        /// Helper method to seperate the full path from the file name.
        /// </summary>
        /// <param name="loggerOutputPath"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static string GetPathOnly(string loggerOutputPath, out string fileName)
        {
            var delimeter = new string[2];
            delimeter[0] = @"\";
            delimeter[1] = @"\\";

            var subStrings = loggerOutputPath.Split(delimeter, StringSplitOptions.None);

            fileName = subStrings[subStrings.Length - 1];
            Array.Resize(ref subStrings, subStrings.Length - 1);

            // NOTE - Hack: To fix error in Path.Combine, MUST add "\" to the first substring!
            subStrings[0] += @"\";

            return Path.Combine(subStrings);
        }

        // 7/17/2011; 10/2/2011 - Updated to add to concurrent queue
        /// <summary>
        /// Helper method which writes a message to the proper <see cref="TraceListener"/>.
        /// </summary>
        /// <param name="messageType">The <see cref="MessageTypeEnum"/> in use.</param>
        /// <param name="traceListenerInstanceId"><see cref="TraceListener"/> array index to use.</param>
        /// <param name="message">Message to log</param>
        internal static void WriteMessage(MessageTypeEnum messageType, int traceListenerInstanceId, string message)
        {

            try
            {
                var appname = (Logger.IsRunningInWcfService) ? "Srv" : "App"; // 7/27/2011

                // add to concurrent task to process
                _writeMessagesTask.DoWriteMessage(traceListenerInstanceId, message, messageType, appname);
            }
            catch (ArgumentOutOfRangeException)
            {
                // Note: Expected error which can occur during shutdown of app.
                Console.WriteLine("WriteMessage in TraceLogParts is throwing the ArgOutOfRangeException error.");
            }

        }

        /// <summary>
        /// Dispose called from the <see cref="LoggerCompositionContainer"/> dispose method.
        /// </summary>
        public static void Dispose()
        {
            // 10/2/2011 - Dispose of Task
            _writeMessagesTask.Dispose();

            // retrieve listeners
            var listeners = new TraceListener[Trace.Listeners.Count];
            Trace.Listeners.CopyTo(listeners, 0);

            var count = listeners.Length;
            for (var index = 0; index < count; index++)
            {
                var listener = listeners[index];
                if (listener == null) continue;

                listener.Flush();
                listener.Close();
                listener.Dispose();
            }

            Trace.Listeners.Clear();
        }
    }
}
