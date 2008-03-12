using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Uiml.Gummy.Domain;
using Uiml.Gummy.Kernel.Services.Controls;

namespace Uiml.Gummy.Kernel.Services
{
    public partial class SpaceService : Panel
    {

        public SpaceService()
        {            
        }

        public void Init()
        {                
            DesignerKernel.Instance.CurrentDocumentChanged += new EventHandler(currentDocumentChanged);
            this.Size = new System.Drawing.Size(460, 340);
            this.Text = "2D Example Space";
        }

        void currentDocumentChanged(object sender, EventArgs e)
        {
            InitializeComponent();
            m_cartesianGraphControl.InitGraph();
            m_cartesianGraphControl.DesignSpaceCursorChanged += new SizeChangeHandler(designSpaceCursorChanged); 
            DesignerKernel.Instance.CurrentDocument.ScreenSizeUpdated += new Document.ScreenSizeUpdateHandler(screenSizeUpdated);
            DesignerKernel.Instance.CurrentDocument.SpaceModeChanged += new Document.SpaceModeChangeHandler(m_cartesianGraphControl.SpaceModeChanged);            
        }

        void screenSizeUpdated(object sender, Size newSize)
        {
            if (newSize != m_cartesianGraphControl.FocussedSize)
            {
                m_cartesianGraphControl.FocussedSize = newSize;
                Refresh();
            }
        }

        void designSpaceCursorChanged(object sender, Size size)
        {
            if (DesignerKernel.Instance.CurrentDocument != null)
                DesignerKernel.Instance.CurrentDocument.CurrentSize = size;
        }

        public bool Open()
        {
            this.Visible = true;                       
            return true;
        }

        public bool Close()
        {
            this.Visible = false;
            return true;
        }

        public string ServiceName
        {
            get
            {
                return "gummy-designspace";
            }
        }

        public System.Windows.Forms.Control ServiceControl
        {
            get
            {
                return this;
            }
        }

        public IServiceConfiguration ServiceConfiguration
        {
            get 
            {
                return null;
            }
        }

        public void NotifyConfigurationChanged()
        {
            return;
        }
        
        public void DocumentUpdated(object sender, EventArgs e)
        {
        }
    }
}