using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Diagnostics;

using Uiml.Gummy.Serialize;
using Uiml.Gummy.Kernel.Services;
using Uiml.Gummy.Kernel.Services.Commands;

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
            UpdateStatus("Docking ToolBox", 1, 3);
            DockMdiChild(GetService("gummy-toolbox"));
            UpdateStatus("Docking Properties Panel", 2, 3);
            DockMdiChild(GetService("gummy-propertypanel"));
            UpdateStatus("Docking Canvas", 2, 3);
            DockMdiChild(GetService("gummy-canvas"));
            UpdateStatus("Ready");
        }

        private void UnDockMdiChild(IService child)
        {
            if (child.ServiceControl is Form)
            {
                Form childForm = (Form)child.ServiceControl;
                childForm.Dock = DockStyle.None;
            }
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
                UpdateStatus(string.Format("Opening {0}", m_services[i].ServiceName), i + 1, m_services.Count);
                Console.WriteLine("Loading " + m_services[i].ServiceName);
                if (!m_services[i].Open())
                {
                    return false;
                }
            }

            UpdateStatus("Ready");
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
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Stream stream = ofd.OpenFile();
                // TODO: ask for confirmation

                CurrentDocument = Document.Open(stream);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.AddExtension = true;
            sfd.Filter = "UIML files (*.uiml)|*.uiml";
            sfd.ShowDialog();
            Stream stream = sfd.OpenFile();

            CurrentDocument.Save(stream);
            stream.Close();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void DesignerKernel_FormClosing(object sender, FormClosingEventArgs e)
        {
            Close();
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // create temporary file for current design
            string fileName = Path.GetTempFileName();
            FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);
            CurrentDocument.Save(stream);
            stream.Close();

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

        private void UpdateStatus(string text)
        {
            toolStripStatusLabel.Text = text; // set text
            statusStrip.Refresh();
        }

        private void UpdateStatus(string text, int step, int max)
        {
            UpdateStatus(text);
            toolStripProgressBar.Maximum = max;
            toolStripProgressBar.Value = step;

            if (toolStripProgressBar.Value == toolStripProgressBar.Maximum)
            {
                Timer timer = new System.Windows.Forms.Timer();
                timer.Interval = 1000;
                timer.Tick += delegate(object sender, EventArgs e)
                {
                    toolStripProgressBar.Visible = false;
                };
                timer.Start();
            }
            else
                toolStripProgressBar.Visible = true;
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CutDomainObject cutDom = new CutDomainObject();
            cutDom.Execute();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyDomainObject copyDomain = new CopyDomainObject();
            copyDomain.Execute();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteDomainObject pasteDomain = new PasteDomainObject(new Point(10, 10));
            pasteDomain.Execute();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DeleteDomainObject deleteDomain = new DeleteDomainObject();
            deleteDomain.Execute();
        }
    }
}