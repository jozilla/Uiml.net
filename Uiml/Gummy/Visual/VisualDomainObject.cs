using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Uiml.Gummy.Domain;
using Uiml.Gummy.Serialize;

namespace Uiml.Gummy.Visual
{
    public class VisualDomainObject : PictureBox
    {
        DomainObject m_domObject = null;
        VisualDomainObjectState m_state = null;
        DomainObject.DomainObjectUpdateHandler m_domUpdated = null;
        BorderDrawer m_borderDrawer = new BorderDrawer();

        public VisualDomainObject() : base()
        {
            //this.BorderStyle = BorderStyle.FixedSingle;
            State = new SelectVisualDomainObjectState();
            m_domUpdated = new DomainObject.DomainObjectUpdateHandler(domainObjectUpdated);
        }

        public VisualDomainObject(DomainObject domObject)
            : this()
        {
            DomainObject = domObject;
        }

        ~VisualDomainObject()
        {
            if (m_domObject != null)
                m_domObject.DomainObjectUpdated -= m_domUpdated;
        }
        /*
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            m_borderDrawer.DrawBorder(ref m, this.Width, this.Height);
        }*/

        public DomainObject DomainObject
        {
            get
            {
                return m_domObject;
            }
            set
            {
                if (m_domObject != null)
                    m_domObject.DomainObjectUpdated -= m_domUpdated;
                m_domObject = value;
                m_domObject.DomainObjectUpdated += m_domUpdated;
                m_domObject.Updated();
            }
        }

        public VisualDomainObjectState State
        {
            get
            {
                return m_state;
            }
            set
            {
                if (m_state != null)
                    m_state.Detach();
                m_state = value;
                m_state.Attach(this);
            }
        }

        protected virtual void domainObjectUpdated(object sender, EventArgs e)
        {
            Image = ActiveSerializer.Instance.Serializer.Serialize(DomainObject);
            this.Size = DomainObject.Size;
            this.Location = DomainObject.Location;
            Refresh();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            Graphics g = pe.Graphics;
            g.DrawRectangle(new Pen(DomainObject.Color,2.0f), 0, 0, Bounds.Width - 2.0f, Bounds.Height - 2.0f);
        }

        void VisualDomainObject_MouseUp(object sender, MouseEventArgs e)
        {            
        }

        void VisualDomainObject_MouseClick(object sender, MouseEventArgs e)
        {           
            
        }

        void VisualDomainObject_MouseMove(object sender, MouseEventArgs e)
        {
        }

    }
}
