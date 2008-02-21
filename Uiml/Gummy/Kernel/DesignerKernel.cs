using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using Uiml.Gummy.Serialize;
using Uiml.Gummy.Kernel.Services;

using System.Windows.Forms;

namespace Uiml.Gummy.Kernel
{
    public class DesignerKernel : Form, IService
    {
        List<IService> m_services = new List<IService>();
        DesignerLoader m_loader = new DesignerLoader();

        public DesignerKernel(string vocabulary): base()
        {
            //TODO: needs to be loaded from a config file or dialog window
            //ActiveSerializer.Instance.Serializer = m_loader.CreateSerializer("idtv-1.0");
            ActiveSerializer.Instance.Serializer = m_loader.CreateSerializer("swf-1.1");
        }

        public void Init()
        {
            InitializeComponents();
            LoadServices(null);
            InitializeMdiChildren();
        }

        private void InitializeComponents()
        {
            Text = "CROSLOCiS Service Creation Environment";
            IsMdiContainer = true;
            WindowState = FormWindowState.Maximized;
            FormClosing += new FormClosingEventHandler(DesignerKernel_FormClosing);

            Menu = new MainMenu();
            MenuItem file = Menu.MenuItems.Add("&File");
            file.MenuItems.Add("&Quit", this.FileQuit_Clicked);
            file.MenuItems.Add("&Export", this.FileExport_Clicked);
            MenuItem windows = Menu.MenuItems.Add("&Window");
            windows.MenuItems.Add("&Docked", this.WindowDocked_Clicked);
            windows.MenuItems.Add("&Cascade", this.WindowCascade_Clicked);
            windows.MenuItems.Add("Tile &Horizontal", this.WindowTileH_Clicked);
            windows.MenuItems.Add("Tile &Vertical", this.WindowTileV_Clicked);
            windows.MdiList = true;
        }

        private void InitializeMdiChildren()
        {
            foreach (IService child in m_services)
            {
                child.Init();

                Form childForm = (Form)child;
                childForm.MdiParent = this;
                childForm.ShowIcon = false;

                if (child.IsEssential)
                    childForm.ControlBox = false;

                DockMdiChild(child);
            }
        }

        private void DockMdiChild(IService child)
        {
            Form childForm = (Form)child;

            switch (child.ServiceName)
            {
                case "gummy-toolbox":
                    childForm.Dock = DockStyle.Left;
                    break;
                case "gummy-canvas":
                    childForm.Dock = DockStyle.None;
                    childForm.Left = 0;
                    childForm.Top = 0;
                    break;
                case "gummy-propertypanel":
                    childForm.Dock = DockStyle.Right;
                    break;
                case "application-glue":
                    childForm.Dock = DockStyle.Right;
                    break;
            }
        }

        private void DockMdiChildren()
        {
            foreach (IService child in m_services)
            {
                DockMdiChild(child);
            }
        }

        private void UnDockMdiChild(IService child)
        {
            Form childForm = (Form)child;
            childForm.Dock = DockStyle.None;
        }

        private void UnDockMdiChildren()
        {
            foreach (IService child in m_services)
            {
                UnDockMdiChild(child);
            }
        }

        public bool Open()
        {
            this.Show();

            for (int i = 0; i < m_services.Count; i++)
            {
                Console.WriteLine("Loading " + m_services[i].ServiceName);
                if (!m_services[i].Open())
                {                    
                    return false;
                }
            }
            try
            {
                Application.Run();
                return true;
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                return false;
            }
        }

        public bool Close()
        {
            for (int i = 0; i < m_services.Count; i++)
            {
                Console.WriteLine("Closing " + m_services[i].ServiceName);
                if (!m_services[i].Close())
                {
                    return false;
                }
            }

            try
            {
                Application.Exit();
                return true;
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                return false;
            }
        }

        public string ServiceName
        {
            get
            {
                return "gummy-kernel";
            }
        }

        public bool IsEssential
        {
            get { return true; }
        }

        //Load the services from an Xml Document
        public void LoadServices(XmlDocument doc)
        {
            m_services.Add(new ToolboxService());
            m_services.Add(new CanvasService());
            m_services.Add(new PropertiesService());
            m_services.Add(new ApplicationGlueService());
        }

        public void WindowCascade_Clicked(object sender, EventArgs args)
        {
            UnDockMdiChildren();
            LayoutMdi(MdiLayout.Cascade);
        }

        public void WindowTileH_Clicked(object sender, EventArgs args)
        {
            UnDockMdiChildren();
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        public void WindowTileV_Clicked(object sender, EventArgs args)
        {
            UnDockMdiChildren();
            LayoutMdi(MdiLayout.TileVertical);
        }

        public void WindowDocked_Clicked(object sender, EventArgs args)
        {
            DockMdiChildren();
        }

        public void FileQuit_Clicked(object sender, EventArgs args)
        {
            Close();
        }

        public void FileExport_Clicked(object sender, EventArgs args)
        {
            List<Part> parts = new List<Part>();
            List<Property> properties = new List<Property>();
            Logic logic;
            Behavior behavior;
            string logicXml = "";
            string behaviorXml = "";

            // collect all the UIML pieces
            foreach (IService service in m_services)
            {
                if (service is IUimlProvider)
                {
                    foreach (IUimlElement item in ((IUimlProvider)service).GetUimlElements())
                    {
                        if (item is Part)
                            parts.Add((Part)item);
                        else if (item is Property)
                            properties.Add((Property)item);
                        else if (item is Behavior)
                            behavior = (Behavior)item; // FIXME: should only happen once
                        else if (item is Logic)
                            logic = (Logic)item; // FIXME: should only happen once
                    }

                    List<string> xmlStrings = ((IUimlProvider)service).GetUimlElementsXml();

                    if (xmlStrings.Count > 0)
                    {
                        // FIXME: hard-coded now
                        logicXml = xmlStrings[0];
                        behaviorXml = xmlStrings[1];
                    }
                }
            }

            // create standard UIML document
            StringWriter writer = new StringWriter();
            XmlTextWriter xmlw = new XmlTextWriter(writer);

            xmlw.WriteStartDocument();
            xmlw.WriteStartElement("uiml");
            xmlw.WriteStartElement("interface");
            xmlw.WriteEndElement(); // </interface>
            xmlw.WriteStartElement("structure");

            // parts
            foreach (Part p in parts)
            {
                XmlNode partXml = p.Serialize(new XmlDocument());
                xmlw.WriteRaw(partXml.OuterXml);
            }

            xmlw.WriteEndElement(); // </structure>
            xmlw.WriteStartElement("style");

            // properties

            foreach (Property p in properties)
            {
                XmlNode propXml = p.Serialize(new XmlDocument());
                xmlw.WriteRaw(propXml.OuterXml);
            }

            xmlw.WriteEndElement(); // </style>

            // behavior
            xmlw.WriteRaw(behaviorXml);

            xmlw.WriteEndElement(); // </interface>

            xmlw.WriteStartElement("peers");
            xmlw.WriteStartElement("presentation");
            xmlw.WriteAttributeString("base", "swf-1.1.uiml");
            xmlw.WriteEndElement(); // </presentation>

            // logic
            xmlw.WriteRaw(logicXml);

            xmlw.WriteEndElement(); // </peers>

            xmlw.WriteEndElement(); // </uiml>

            xmlw.WriteEndDocument();
            xmlw.Close();

            string xml = writer.ToString();
            string s = xml;
        }

        public void DesignerKernel_FormClosing(object sender, EventArgs args)
        {
            Close();
        }
    }
}
