using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Drawing;

using Uiml.Gummy.Domain;
using Uiml.Gummy.Serialize;

namespace Uiml.Gummy.Kernel
{
    public class DocumentEventArgs : EventArgs
    {        
        public DocumentEventArgs() : base()
        {
        }
    }
    /// <summary>
    /// This class represents the current document in Gummy.
    /// </summary>
    public class Document
    {
        private DomainObjectCollection m_domObjects = new DomainObjectCollection();
        private DomainObject m_formContainer = null;

        public delegate void ScreenSizeUpdateHandler(object sender, Size newSize);
        public event ScreenSizeUpdateHandler ScreenSizeUpdated;
        
        public DomainObjectCollection DomainObjects
        {
            get { return m_domObjects; }
        }

        public Document()
        {
            m_formContainer = ActiveSerializer.Instance.Serializer.CreateUIContainer();
        }

        public DomainObject FormContainer
        {
            get
            {
                return m_formContainer;
            }
            set
            {
                m_formContainer = value;
            }
        }

        public Size CurrentSize
        {
            get
            {
                return m_formContainer.Size;
            }
            set
            {
                m_formContainer.Size = value;
                if (ScreenSizeUpdated != null)
                    ScreenSizeUpdated(this, CurrentSize);
            }
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
