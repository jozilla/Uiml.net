using System;
using System.Collections.Generic;
using System.Text;

using System.Windows.Forms;
using System.Drawing;

using Uiml;
using Uiml.Gummy.Kernel.Selected;

namespace Uiml.Gummy.Visual
{
    public class VisualProperty : Panel
    {
        TextBox m_propertyValue = new TextBox();
        Label m_propertyTitle = new Label();
        Property m_prop = null;

        public VisualProperty( Property prop ) : base()
        {
            Size = new Size(200,25);
            m_prop = prop;

            Controls.Add(m_propertyValue);
            Controls.Add(m_propertyTitle);

            m_propertyTitle.Dock = DockStyle.Left;
            m_propertyValue.Dock = DockStyle.Fill;

            m_propertyValue.LostFocus += new EventHandler(onLostFocus);

            RefreshMe();
        }

        void onLostFocus(object sender, EventArgs e)
        {
            m_prop.Value = m_propertyValue.Text;
            if (SelectedDomainObject.Instance.Selected != null)
                SelectedDomainObject.Instance.Selected.Updated();
        }

        //Need to become an observer function....
        public void RefreshMe()
        {
            m_propertyTitle.Text = m_prop.Name;
            m_propertyValue.Text = m_prop.Value.ToString();
        }

        public Property Property
        {
            get
            {
                return m_prop;
            }
            set
            {
                m_prop = value;
            }
        }
    }
}
