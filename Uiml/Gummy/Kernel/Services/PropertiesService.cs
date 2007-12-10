using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

using Uiml.Gummy.Kernel;
using Uiml.Gummy.Kernel.Selected;
using Uiml.Gummy.Visual;

namespace Uiml.Gummy.Kernel.Services
{
    public class PropertiesService : Form, IService
    {
        Domain.DomainObject m_dom = null;
        Domain.DomainObject.DomainObjectUpdateHandler m_domUpdateHandler = null;

        public PropertiesService()
            : base()
        { 
        }

        public void Init()
        {
            Text = "Properties";
            SelectedDomainObject.Instance.DomainObjectSelected += new SelectedDomainObject.DomainObjectSelectedHandler(onDomainObjectSelected);
            Size = new Size(225, 500);
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
                this.Controls.Clear();
                int y = 0;
                int x = 5;
                for (int i = 0; i < dom.Properties.Count; i++)
                {
                    VisualProperty prop = new VisualProperty(dom.Properties[i]);
                    prop.Location = new Point(x, y);
                    y += prop.Size.Height;
                    Controls.Add(prop);
                }
                Size = new Size(200, y + 100);
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
    }
}
