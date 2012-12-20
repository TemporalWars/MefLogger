using System.Collections.Generic;

namespace ImageNexus.BenScharbach.Common.MEFLogger.XmlWriterLogParts.Xml
{
    // 3/23/2012
    /// <summary>
    /// The <see cref="XmlWrapper{TData}"/> wraps the given XML generic data into a collection.
    /// </summary>
    public class XmlWrapper<TData>
    {
        public List<TData> CollectionToSerialize = new List<TData>();
    }
}