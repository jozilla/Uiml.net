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
        //TODO: Add the wireframe data to the document
        private Size m_wireFrameSize = Size.Empty;

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

        public XmlDocument Serialize()
        {
            XmlDocument doc = new XmlDocument();
            XmlNode uiml = doc.CreateElement("uiml");
            doc.AppendChild(uiml);
            XmlNode iface = doc.CreateElement("interface");
            uiml.AppendChild(iface);
            XmlNode structure = doc.CreateElement("structure");
            iface.AppendChild(structure);
            XmlNode style = doc.CreateElement("style");
            iface.AppendChild(style);
            
            // <Fill up structure and style>
            // start with top container
            XmlNode topPart = FormContainer.Part.Serialize(doc);
            structure.AppendChild(topPart);
            foreach (Property p in FormContainer.Properties)
            {
                XmlNode prop = p.Serialize(doc);
                style.AppendChild(prop);
            }
            // now all other domain objects
            foreach (DomainObject dom in DomainObjects)
            {
                XmlNode part = dom.Part.Serialize(doc);
                topPart.AppendChild(part);

                foreach (Property p in dom.Properties)
                {
                    XmlNode prop = p.Serialize(doc);
                    style.AppendChild(prop);
                }
            }
            // </Fill up structure and style>

            XmlNode peers = doc.CreateElement("peers");
            uiml.AppendChild(peers);
            XmlNode presentation = doc.CreateElement("presentation");
            XmlAttribute baseAttr = doc.CreateAttribute("base");
            baseAttr.Value = ActiveSerializer.Instance.Serializer.Voc.Identifier + ".uiml";
            presentation.Attributes.Append(baseAttr);
            peers.AppendChild(presentation);

            return doc;
        }

        public static Document New()
        {
            return new Document();
        }

        public static Document Open(Stream s)
        {
            return null;
            //return new Document(s);
        }

        public void Save(Stream s)
        {
            XmlDocument doc = Serialize();
            doc.Save(s);
        }
    }
}
