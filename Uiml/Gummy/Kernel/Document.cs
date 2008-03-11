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

using Uiml.Gummy.Kernel.Services.ApplicationGlue;
using System.Reflection;

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

        private BehaviorRegistry m_behavior = new BehaviorRegistry();
        private List<Assembly> m_libraries = new List<Assembly>();

        public delegate void ScreenSizeUpdateHandler(object sender, Size newSize);
        public event ScreenSizeUpdateHandler ScreenSizeUpdated;
        public delegate void SpaceModeChangeHandler(object sender, Mode mode);
        public event SpaceModeChangeHandler SpaceModeChanged;
        
        public DomainObjectCollection DomainObjects
        {
            get { return m_domObjects; }
        }

        public BehaviorRegistry Behavior
        {
            get { return m_behavior; }
        }

        public List<Assembly> Libraries
        {
            get { return m_libraries; }
        }

        public Document()
        {
            m_formContainer = ActiveSerializer.Instance.Serializer.CreateUIContainer();
        }

        public Document(Stream s) : this()
        {
            // FIXME: use reverse type decoders!
            XmlDocument xml = new XmlDocument();
            xml.Load(s);
            UimlDocument uiml = new UimlDocument(xml.DocumentElement);

            Structure structure = (Structure)uiml.UInterface.UStructure[0];
            Style style = (Style)uiml.UInterface.UStyle[0];
            ArrayList parts = structure.Children;

            if (structure.Top != null)
            {
                List<Property> props = new List<Property>();
                foreach (Property prop in style.GetNamedPropertiesList(structure.Top.Identifier))
                    props.Add(prop);
                foreach (Property prop in structure.Top.PropertiesList)
                    props.Add(prop);

                DomainObject dom = DomainObjectFactory.Instance.Create(structure.Top, props);
                FormContainer = dom;

                // add its children
                foreach (Part p in structure.Top.GetPartChildren())
                {
                    props.Clear();
                    foreach (Property prop in style.GetNamedPropertiesList(p.Identifier))
                        props.Add(prop);
                    foreach (Property prop in p.PropertiesList)
                        props.Add(prop);

                    dom = DomainObjectFactory.Instance.Create(p, props);
                    DomainObjects.Add(dom);
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
                // screen size is updated as well
                if (ScreenSizeUpdated != null)
                    ScreenSizeUpdated(this, CurrentSize);
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

            // <behavior>
            XmlNode behavior = Behavior.GenerateBehavior(doc);
            iface.AppendChild(behavior);

            XmlNode peers = doc.CreateElement("peers");
            uiml.AppendChild(peers);

            // presentation
            XmlNode presentation = doc.CreateElement("presentation");
            XmlAttribute baseAttr = doc.CreateAttribute("base");
            baseAttr.Value = ActiveSerializer.Instance.Serializer.Voc.Identifier + ".uiml";
            presentation.Attributes.Append(baseAttr);
            peers.AppendChild(presentation);

            // logic
            XmlNode logic = Behavior.GenerateLogic(doc);
            peers.AppendChild(logic);

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
            string libArgs = "-libs";

            foreach (Assembly a in Libraries)
            {
                string libFile = a.Location;
                libArgs += " " + Path.ChangeExtension(libFile, null);
            }

            string uimldotnetArgs = uimlArgs + " " + libArgs;
            ProcessStartInfo psi = new ProcessStartInfo(@"..\..\Uiml.net\Debug\uiml.net.exe", uimldotnetArgs);
            psi.ErrorDialog = true;
            Process.Start(psi);
        }
    }
}
