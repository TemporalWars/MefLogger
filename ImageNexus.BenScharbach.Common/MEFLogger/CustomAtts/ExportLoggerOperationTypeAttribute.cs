using System;
using System.ComponentModel.Composition;
using ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Enums;
using ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Interfaces;

namespace ImageNexus.BenScharbach.Common.MEFLogger.CustomAtts
{
    /// <summary>
    /// The <see cref="ExportLoggerOperationTypeAttribute"/> is a custom <see cref="ExportAttribute"/> used to define new parts for
    /// the ILogger.
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExportLoggerOperationTypeAttribute : ExportAttribute
    {
        public ExportLoggerOperationTypeAttribute() : base(typeof(ILoggerOperationType)) { }
        public MessageTypeEnum MessageType { get; set; }
        public LoggerTypeEnum LoggerType { get; set; }
    }
}