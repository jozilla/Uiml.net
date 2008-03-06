using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Diagnostics;

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
        Document m_document;

        static DesignerKernel m_kernel = null;

        private DesignerKernel()
        {
            ActiveSerializer.Instance.Serializer = m_loader.CreateSerializer(m_platform);
            Application.EnableVisualStyles(); // visual styles (e.g. Windows Vista, XP, Linux, etc.)
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

        public Document CurrentDocument
        {
            get { return m_document; }
            set { m_document = value; }
        }

        public void Init()
        {
            CurrentDocument = Document.New();
            InitializeComponents();
            LoadServices(null); // initialize all services
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
            file.MenuItems.Add("&Open", this.FileOpen_Clicked);
            file.MenuItems.Add("-"); // separator
            file.MenuItems.Add("&Save", this.FileSave_Clicked);
            file.MenuItems.Add("-"); // separator
            file.MenuItems.Add("&Run", this.FileRun_Clicked);
            file.MenuItems.Add("-"); // separator
            file.MenuItems.Add("&Quit", this.FileQuit_Clicked);
            MenuItem windows = Menu.MenuItems.Add("&Window"); 
            windows.MenuItems.Add("&Docked", this.WindowDocked_Clicked);
            windows.MenuItems.Add("&Cascade", this.WindowCascade_Clicked);
            windows.MenuItems.Add("Tile &Horizontal", this.WindowTileH_Clicked);
            windows.MenuItems.Add("Tile &Vertical", this.WindowTileV_Clicked);
            windows.MdiList = true;

            // set icon
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DesignerKernel));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
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
            Application.Run();            
            return true;           
        }

        protected bool OpenChildren()
        {
            for (int i = 0; i < m_services.Count; i++)
            {
                Console.WriteLine("Loading " + m_services[i].ServiceName);
                if (!m_services[i].Open())
                {
                    return false;
                }
            }

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
            AttachService(new WireFrameService());
            AttachService(new PropertiesService());

            InitializeMdiChildren(); // initialize all services
        }

        public void ShowServices()
        {
            // GUI stuff
            OpenChildren();
            DockMdiChildren();
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

        public void FileNew_Clicked(object sender, EventArgs args) 
        {
            CurrentDocument = Document.New();

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

            if (wizard.DialogResult == DialogResult.OK)
                ShowServices();
        }

        public void FileOpen_Clicked(object sender, EventArgs args)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "UIML files (*.uiml)|*.uiml";
            ofd.ShowDialog();
            Stream stream = ofd.OpenFile();
            // TODO: ask for confirmation

            CurrentDocument = Document.Open(stream);
        }

        public void FileSave_Clicked(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.AddExtension = true;
            sfd.Filter = "UIML files (*.uiml)|*.uiml";
            sfd.ShowDialog();
            Stream stream = sfd.OpenFile();

            CurrentDocument.Save(stream);
        }

        public void FileRun_Clicked(object sender, EventArgs args)
        {
            // create temporary file for current design
            string fileName = Path.GetTempFileName();
            FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);
            CurrentDocument.Save(stream);

            // run renderer on this file
            string uimlArgs = string.Format("-uiml {0}", fileName);
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

        public void FileQuit_Clicked(object sender, EventArgs args)
        {
            Close();
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

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DesignerKernel));
            this.SuspendLayout();
            // 
            // DesignerKernel
            // 
            this.ClientSize = new System.Drawing.Size(284, 264);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DesignerKernel";
            this.ResumeLayout(false);

        }
    }
}
