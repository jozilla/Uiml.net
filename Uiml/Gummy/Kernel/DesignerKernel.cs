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
    public partial class DesignerKernel : Form, IService, IServiceContainer
    {
        List<IService> m_services = new List<IService>();
        DesignerLoader m_loader = new DesignerLoader();
        string m_platform = "swf-1.1";
        Document m_document;

        static DesignerKernel m_kernel = null;

        public event EventHandler CurrentDocumentChanged;

        private DesignerKernel()
        {
            ActiveSerializer.Instance.Serializer = m_loader.CreateSerializer(m_platform);
            Application.EnableVisualStyles(); // visual styles (e.g. Windows Vista, XP, Linux, etc.)
            
            InitializeComponent();
            FormClosing += new FormClosingEventHandler(DesignerKernel_FormClosing);
        }

        private DesignerKernel(string vocabulary)
            : base()
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
            set
            {
                m_document = value;
                if (CurrentDocumentChanged != null)
                    CurrentDocumentChanged(this, null);
            }
        }

        public void Init()
        {
            CurrentDocument = Document.New();
            InitializeComponents();
            LoadServices(null); // initialize all services
        }

        private void InitializeComponents()
        {

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

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "UIML files (*.uiml)|*.uiml";
            ofd.ShowDialog();
            Stream stream = ofd.OpenFile();
            // TODO: ask for confirmation

            CurrentDocument = Document.Open(stream);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.AddExtension = true;
            sfd.Filter = "UIML files (*.uiml)|*.uiml";
            sfd.ShowDialog();
            Stream stream = sfd.OpenFile();

            CurrentDocument.Save(stream);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void DesignerKernel_FormClosing(object sender, EventArgs args)
        {
            Close();
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void dockedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DockMdiChildren();
        }

        private void cascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UnDockMdiChildren();
            LayoutMdi(MdiLayout.Cascade);
        }

        private void tileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UnDockMdiChildren();
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void tileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UnDockMdiChildren();
            LayoutMdi(MdiLayout.TileVertical);
        }
    }
}