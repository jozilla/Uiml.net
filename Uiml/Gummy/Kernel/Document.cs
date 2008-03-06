using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

using Uiml.Gummy.Domain;

namespace Uiml.Gummy.Kernel
{
    /// <summary>
    /// This class represents the current document in Gummy.
    /// </summary>
    public class Document
    {
        private DomainObjectCollection m_domObjects = new DomainObjectCollection();

        public DomainObjectCollection DomainObjects
        {
            get { return m_domObjects; }
        }

        public Document()
        {
        }

        /// <summary>
        /// Reads document from a stream.
        /// </summary>
        /// <param name="s">Stream to read from</param>
        public Document(Stream s)
        {
        }

        public static Document New()
        {
            return new Document();
        }

        public static Document Open(Stream s)
        {
            return new Document(s);
        }

        public void Save(Stream s)
        {
            XmlTextWriter xmlw = new XmlTextWriter(s, null);
            xmlw.Formatting = Formatting.Indented;
        }
    }
}
