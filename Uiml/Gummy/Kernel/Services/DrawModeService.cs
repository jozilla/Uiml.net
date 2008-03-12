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
            Selected.SelectedDomainObject.Instance.DomainObjectSelected += new Uiml.Gummy.Kernel.Selected.SelectedDomainObject.DomainObjectSelectedHandler(domainObjectSelected);
        }

        void domainObjectSelected(Uiml.Gummy.Domain.DomainObject dom, EventArgs e)
        {
            if (m_drawModes != null)
            {
                if (dom != null)
                {
                    m_drawModes.DrawModeEnabled = true;
                }
                else
                {
                    m_drawModes.DrawModeEnabled = false;
                }

                if (Selected.SelectedDomainObject.Instance.MultipleSelected)
                {
                    m_drawModes.GroupingEnabled = true;
                }
                else
                {
                    m_drawModes.GroupingEnabled = false;
                }
            }
        }

        void documentChanged(object sender, EventArgs e)
        {
            DesignerKernel.Instance.CurrentDocument.SpaceModeChanged += new Document.SpaceModeChangeHandler(m_drawModes.SpaceModeChanged);
            m_drawModes.SpaceModeChanged(this, DesignerKernel.Instance.CurrentDocument.Mode);
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
