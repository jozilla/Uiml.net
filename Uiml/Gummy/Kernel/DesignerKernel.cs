using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using Uiml.Gummy.Serialize;
using Uiml.Gummy.Kernel.Services;

using System.Windows.Forms;
using System.Drawing;

namespace Uiml.Gummy.Kernel
{
    public class DesignerKernel : Form, IService, IServiceContainer
    {
        List<IService> m_services = new List<IService>();
        DesignerLoader m_loader = new DesignerLoader();
        string m_platform = "swf-1.1";

        static DesignerKernel m_kernel = null;

        private DesignerKernel()
        {
            ActiveSerializer.Instance.Serializer = m_loader.CreateSerializer(m_platform);
        }

        private DesignerKernel(string vocabulary): base()
        {
            //TODO: needs to be loaded from a config file or dialog window
            //ActiveSerializer.Instance.Serializer = m_loader.CreateSerializer("idtv-1.0");
            Platform = vocabulary;
        }

        public static DesignerKernel Instance
        {
            get
            {
                if (m_kernel == null)
                    m_kernel = new DesignerKernel();
                return m_kernel;
            }
            set
            {
                m_kernel = value;
            }
        }

        public string Platform
        {
            get
            {
                return m_platform;
            }
            set
            {
                m_platform = value;
                ActiveSerializer.Instance.Serializer = m_loader.CreateSerializer(m_platform);
            }
        }

        public void Init()
        {
            LoadServices(null);
            InitializeComponents();
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
            file.MenuItems.Add("&New", this.FileNew_Clicked);
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

                if (child.ServiceControl is Form)
                {
                    Form childForm = (Form)child.ServiceControl;
                    childForm.MdiParent = this;
                    childForm.ShowIcon = false;
                    childForm.ControlBox = false;
                }
            }
        }

        private void DockMdiChild(IService child)
        {
            if (child.ServiceControl is Form)
            {
                Form childForm = (Form)child.ServiceControl;

                switch (child.ServiceName)
                {
                    case "gummy-toolbox":
                        childForm.Dock = DockStyle.Left;
                        break;
                    case "gummy-canvas":
                        Form toolbox = (Form)GetService("gummy-toolbox").ServiceControl;
                        Form properties = (Form)GetService("gummy-propertypanel").ServiceControl;
                        childForm.StartPosition = FormStartPosition.Manual;
                        childForm.Location = new System.Drawing.Point(toolbox.Width + 1, toolbox.Location.Y);
                        childForm.Size = new System.Drawing.Size(properties.Location.X - toolbox.Width - 1, toolbox.Height);                        
                        break;                    
                    case "gummy-propertypanel":
                        childForm.Dock = DockStyle.Right;
                        childForm.Width = 300;
                        break;
                }
            }
        }

        private void DockMdiChildren()
        {
            DockMdiChild(GetService("gummy-toolbox"));
            DockMdiChild(GetService("gummy-propertypanel"));
            DockMdiChild(GetService("gummy-canvas"));
        }

        private void UnDockMdiChild(IService child)
        {
            Form childForm = (Form)child.ServiceControl;
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
            DockMdiChildren();
            Application.Run();            
            return true;           
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

        public System.Windows.Forms.Control ServiceControl
        {
            get
            {
                return this;
            }
        }

        public List<IService> Services
        {
            get
            {
                return m_services;
            }
        }

        public IService GetService(string name)
        {
            for (int i = 0; i < m_services.Count; i++)
            {
                if (m_services[i].ServiceName == name)
                    return m_services[i];
            }
            return null;
        }

        public void AttachService(IService service)
        {
            m_services.Add(service);
        }

        //Load the services from an Xml Document
        public void LoadServices(XmlDocument doc)
        {
            AttachService(new ToolboxService());
            AttachService(new CanvasService());
            AttachService(new SpaceService());            
            AttachService(new PropertiesService());            
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

        public void FileNew_Clicked(object sender, EventArgs args) 
        {
            NewWizard wizard = new NewWizard();

            foreach (IService s in Services)
            {
                if (s.ServiceConfiguration != null)
                {
                    wizard.AddConfiguration(s.ServiceConfiguration);
                }
            }

            wizard.Start();
            wizard.ShowDialog();
            // TODO: notify services that their settings are changed
        }

        public void DesignerKernel_FormClosing(object sender, EventArgs args)
        {
            Close();
        }

        public IServiceConfiguration ServiceConfiguration
        {
            get 
            {
                return null; // no configuration
            }
        }

        public void NotifyConfigurationChanged()
        {
            return;
        }
    }
}
