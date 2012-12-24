using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using ImageNexus.BenScharbach.Common.MEFLogger.TraceLogParts;
using ImageNexus.BenScharbach.Common.MEFLoggerInterfaces.Interfaces;

namespace ImageNexus.BenScharbach.Common.MEFLogger
{
    /// <summary>
    /// The <see cref="LoggerCompositionContainer"/> is used to aggregate multiple MEF parts into one instance.
    /// </summary>
    public class LoggerCompositionContainer : ILoggerCompositionContainer, IDisposable
    {
        private readonly CompositionContainer _container;

        [Import(typeof (ILogger))]
        public ILogger Logger { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public LoggerCompositionContainer()
        {
            //An aggregate catalog that combines multiple catalogs
            var catalog = new AggregateCatalog();
            //Adds all the parts found in the same assembly as the Program class
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(LoggerCompositionContainer).Assembly));
            //catalog.Catalogs.Add(new DirectoryCatalog("C:\\Users\\PEdgeServer2_Admin\\Documents\\Visual Studio 2010\\Projects\\SimpleCalculator\\SimpleCalculator\\CS\\SimpleCalculator3\\Extensions"));
            
            //Create the CompositionContainer with the parts in the catalog
            _container = new CompositionContainer(catalog);

            //Fill the imports of this object
            try
            {
                _container.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                Console.WriteLine(compositionException.ToString());
            }
        }

        // 6/23/2011
        public void Dispose()
        {
           TraceLogCtl.Dispose();
        }
    }

    /*[TestClass]
    public class testLogger
    {
        [TestMethod]
        public void Test()
        {
            var logger = new LoggerCompositionContainer(); //Composition is performed in the constructor

            // Test Writes
            logger.Logger.WriteItem(LoggerTypeEnum.EntLibLogger, MessageTypeEnum.Warning, "This stupid program crashed at 123xxdr12", null);
            logger.Logger.WriteItem(LoggerTypeEnum.EntLibLogger, MessageTypeEnum.Information, "Your computer is about to blow up!", null);
            logger.Logger.WriteItem(LoggerTypeEnum.EntLibLogger, MessageTypeEnum.Error, "What the F#$K are you doing!", null);

            // Test Reads
            //var result = logger.Logger.ReadItem(LoggerTypeEnum.EventLog, MessageTypeEnum.Warning);
            //result = logger.Logger.ReadItem(LoggerTypeEnum.EventLog, MessageTypeEnum.Information);
            //result = logger.Logger.ReadItem(LoggerTypeEnum.EventLog, MessageTypeEnum.Error);
        }
    }*/
   
}
