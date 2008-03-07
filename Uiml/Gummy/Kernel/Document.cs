using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Drawing;
using System.Diagnostics;

using System.Collections;
using Uiml.Gummy.Domain;
using Uiml.Gummy.Serialize;

namespace Uiml.Gummy.Kernel
{
    /// <summary>
    /// This class represents the current document in Gummy.
    /// </summary>

    public enum Mode
    {
        Draw,
        Delete,
        Paint,
        Navigate
    };

    public class Document
    {
        private DomainObjectCollection m_domObjects = new DomainObjectCollection();
        private DomainObject m_formContainer = null;
        private Mode m_spaceMode = Mode.Navigate;
        //TODO: Add the wireframe data to the document
        private Size m_wireFrameSize = Size.Empty;

        public delegate void ScreenSizeUpdateHandler(object sender, Size newSize);
        public event ScreenSizeUpdateHandler ScreenSizeUpdated;
        public delegate void SpaceModeChangeHandler(object sender, Mode mode);
        public event SpaceModeChangeHandler SpaceModeChanged;
        
        public DomainObjectCollection DomainObjects
        {
            get { return m_domObjects; }
        }

        public Document()
        {
            m_formContainer = ActiveSerializer.Instance.Serializer.CreateUIContainer();
        }

        public Document(Stream s) : this()
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(s);
            UimlDocument uiml = new UimlDocument(xml.DocumentElement);

            ArrayList parts = ((Structure)uiml.UInterface.UStructure[0]).Children;

            if (parts != null && parts.Count > 0)
            {
                int i = 0;
                foreach (Part p in ((Structure)uiml.UInterface.UStructure[0]).Children)
                {
                    if (i == 0)
                    {
                    }

                    i++;
                }
            }
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

        public Mode Mode
        {
            get
            {
                return m_spaceMode;
            }
            set
            {
                m_spaceMode = value;
                if (SpaceModeChanged != null)
                    SpaceModeChanged(this, value);
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
            return new Document(s);
        }

        public void Save(Stream s)
        {
            XmlDocument doc = Serialize();
            doc.Save(s);
        }

        public void Run()
        {
            // create temporary file
            string fileName = Path.GetTempFileName();
            FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);
            Save(stream);
            stream.Close();

            // run renderer on this file
            string uimlArgs = string.Format("-uiml \"{0}\"", fileName);
            /*string libArgs = string.Empty;

            try
            {
                string libFile = ((ApplicationGlueServiceConfiguration)GetService("application-glue").ServiceConfiguration).Assembly.Location;
                libArgs = string.Format("-libs {0}", Path.ChangeExtension(libFile, null));
            }
            catch
            {
            }*/

            string uimldotnetArgs = uimlArgs/* + " " + libArgs*/;
            ProcessStartInfo psi = new ProcessStartInfo(@"..\..\Uiml.net\Debug\uiml.net.exe", uimldotnetArgs);
            psi.ErrorDialog = true;
            Process.Start(psi);
        }
    }
}
