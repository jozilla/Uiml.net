using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

using Uiml.Gummy.Kernel;
using Uiml.Gummy.Kernel.Selected;
using Uiml.Gummy.Visual;
using Uiml.Gummy.Kernel.Services.Controls;

namespace Uiml.Gummy.Kernel.Services
{
    public class PropertiesService : Form, IService
    {
        Domain.DomainObject m_dom = null;
        Domain.DomainObject.DomainObjectUpdateHandler m_domUpdateHandler = null;
        List<Control> m_propertyControls = new List<Control>();

        public PropertiesService()
            : base()
        { 
        }

        public void Init()
        {
            Text = "Properties";
            SelectedDomainObject.Instance.DomainObjectSelected += new SelectedDomainObject.DomainObjectSelectedHandler(onDomainObjectSelected);
            Size = new Size(300, 500);
            Panel bottom = new Panel();            
            //TODO: this kind of attachments should be done by service dependencys
            Control spaceService = DesignerKernel.Instance.GetService("gummy-designspace").ServiceControl;
            bottom.Controls.Add(spaceService);
            Control wfController = DesignerKernel.Instance.GetService("gummy-wireframes").ServiceControl;
            bottom.Controls.Add(wfController);
            bottom.Height = spaceService.Height + wfController.Height + 10;
            spaceService.Dock = DockStyle.Bottom;
            wfController.Dock = DockStyle.Fill;
            if (bottom != null)
            {
                bottom.Dock = DockStyle.Bottom;                
                Controls.Add(bottom);
            }
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
                return "gummy-propertypanel";
            }
        }

        public System.Windows.Forms.Control ServiceControl
        {
            get
            {
                return this;
            }
        }

        void onDomainObjectSelected(Domain.DomainObject dom, EventArgs e)
        {
            if (m_dom != dom)
            {
                if (m_domUpdateHandler != null)
                    m_dom.DomainObjectUpdated -= m_domUpdateHandler;
                m_domUpdateHandler = new Uiml.Gummy.Domain.DomainObject.DomainObjectUpdateHandler(onDomainObjectUpdate);
                m_dom = dom;
                m_dom.DomainObjectUpdated += m_domUpdateHandler;
                for (int i = 0; i < m_propertyControls.Count; i++)
                    Controls.Remove(m_propertyControls[i]);
                m_propertyControls.Clear();
                int y = 0;
                int x = 5;
                for (int i = 0; i < dom.Properties.Count; i++)
                {
                    VisualProperty prop = new VisualProperty(dom.Properties[i]);
                    prop.Location = new Point(x, y);
                    y += prop.Size.Height;
                    Controls.Add(prop);
                    m_propertyControls.Add(prop);
                }
                //Size = new Size(200, y + 100);                
            }
            else
            {
                updateAll();
            }
        }

        void onDomainObjectUpdate(object sender, EventArgs e)
        {
            if (sender == m_dom)
            {
                updateAll();
            }
            else
            {
                Console.WriteLine("Something went wrong?");
            }
        }

        void updateAll()
        {
            for (int i = 0; i < Controls.Count; i++)
            {
                if (Controls[i] is VisualProperty)
                {
                    VisualProperty visProp = (VisualProperty)Controls[i];
                    visProp.RefreshMe();
                }
            }            
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
