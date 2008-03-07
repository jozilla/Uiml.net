using System;
using System.Collections.Generic;
using System.Text;

using Uiml.Gummy.Kernel.Services.Controls;

namespace Uiml.Gummy.Kernel.Services
{
    public class DrawModeService : IService
    {
        DrawModes m_drawModes = null;

        public DrawModeService()
        {
            DesignerKernel.Instance.CurrentDocumentChanged += new EventHandler(documentChanged);
        }

        void documentChanged(object sender, EventArgs e)
        {
            DesignerKernel.Instance.CurrentDocument.SpaceModeChanged += new Document.SpaceModeChangeHandler(m_drawModes.SpaceModeChanged);
        }        

        public void Init()
        {
            m_drawModes = new DrawModes();
        }

        public bool Open()
        {
            m_drawModes.Visible = true;
            return true;
        }

        public bool Close()
        {
            return true;
        }

        public string ServiceName
        {
            get
            {
                return "gummy-drawmodes";
            }
        }

        public System.Windows.Forms.Control ServiceControl
        {
            get
            {
                return m_drawModes;
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
        }
    }
}
