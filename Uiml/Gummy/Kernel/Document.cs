using System;
using System.Collections.Generic;
using System.Text;

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

        public static Document New()
        {
        }

        public static Document Open(Stream s)
        {
        }

        public void Save(Stream s)
        {
        }
    }
}
