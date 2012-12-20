using System;
using System.Diagnostics;
using System.IO;

namespace ImageNexus.BenScharbach.Common.MEFLogger.ConsoleParts
{
    // 7/1/2011
    static class ConsoleLog
    {

        //private ConcurrentDictionary<int,List>

        /// <summary>
        /// Creates an instance of a <see cref="TextWriterTraceListener"/>.
        /// </summary>
        /// <returns>The ID for the created <see cref="TraceListener"/></returns>
        public static int CreateTextTraceListener(string loggerOutputPath = "ICL_Trace_Logs\\TraceLogCallsOutput.txt")
        {
            return 0;
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
            return Path.Combine(subStrings);
        }


        public static void WriteMessage(string message, int traceListenerInstanceId)
        {
            
        }

        public static void Dispose()
        {
           
        }
    }
}
