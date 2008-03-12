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
            State = new ToolboxVisualDomainObjectState();
            m_domUpdated = new DomainObject.DomainObjectUpdateHandler(domainObjectUpdated);
        }

        public VisualDomainObject(DomainObject domObject)
            : this()
        {
            DomainObject = domObject;
            //AutoSize = true;
            //this.SizeMode = PictureBoxSizeMode.AutoSize;            
        }

        ~VisualDomainObject()
        {
            if (m_domObject != null)
                m_domObject.DomainObjectUpdated -= m_domUpdated;
        }
        
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
                FixSize();
            }
        }

        private bool m_iconMode = false;

        public bool IconMode
        {
            set
            {
                m_iconMode = value;                
                if (m_iconMode)
                {
                    SizeMode = PictureBoxSizeMode.CenterImage;
                    BorderStyle = BorderStyle.FixedSingle;
                }
                else
                {
                    BorderStyle = BorderStyle.None;
                    SizeMode = PictureBoxSizeMode.Normal;
                }
                if (DomainObject != null)
                    DomainObject.Updated();
            }
            get
            {
                return m_iconMode;
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
            if (!m_iconMode)
            {
                Image = ActiveSerializer.Instance.Serializer.Serialize(DomainObject);
                this.Size = DomainObject.Size;
                this.Location = DomainObject.Location;
                Refresh();
            }
            else
            {
                Image = ActiveSerializer.Instance.Serializer.SerializeToIcon(DomainObject);
                //FIXME: Get this size from a property file or something like that
                Size = new Size(34, 34);      
               
            }
        }
        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            //Graphics g = pe.Graphics;
            //g.DrawRectangle(new Pen(DomainObject.Color,2.0f), 0, 0, Bounds.Width - 2.0f, Bounds.Height - 2.0f);
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

        /*
         * Fix the size between the image and the UIML properties
         */
        public void FixSize()
        {
            if(Image != null && !IconMode)
                DomainObject.Size = Image.Size;
        }

    }
}
