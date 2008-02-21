using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Uiml.Gummy.Serialize;
using Uiml.Gummy.Kernel.Services;

using System.Windows.Forms;

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
            Text = "Gummy";
            IsMdiContainer = true;
            WindowState = FormWindowState.Maximized;
            FormClosing += new FormClosingEventHandler(DesignerKernel_FormClosing);

            Menu = new MainMenu();
            MenuItem file = Menu.MenuItems.Add("&File");
            file.MenuItems.Add("&New", this.FileNew_Clicked);
            file.MenuItems.Add("&Quit", this.FileQuit_Clicked);
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

                Form childForm = (Form)child.ServiceControl;
                childForm.MdiParent = this;
                childForm.ShowIcon = false;
                childForm.ControlBox = false;
                DockMdiChild(child);
            }
        }

        private void DockMdiChild(IService child)
        {
            Form childForm = (Form)child.ServiceControl;

            switch (child.ServiceName)
            {
                case "gummy-toolbox":
                    childForm.Dock = DockStyle.Left;
                    break;
                case "gummy-canvas":
                    childForm.Dock = DockStyle.None;
                    childForm.Left = 100;
                    childForm.Top = 0;
                    break;
                case "gummy-designspace":
                    childForm.Dock = DockStyle.None;
                    childForm.Left = 400;
                    childForm.Top = 0;
                    break;
                case "gummy-propertypanel":
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
