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

        public VisualDomainObject() : base()
        {
            this.BorderStyle = BorderStyle.FixedSingle;
            State = new SelectVisualDomainObjectState();               
        }

        public VisualDomainObject(DomainObject domObject)
            : this()
        {
            DomainObject = domObject;
        }

        public DomainObject DomainObject
        {
            get
            {
                return m_domObject;
            }
            set
            {
                m_domObject = value;
                m_domObject.DomainObjectUpdated += new DomainObject.DomainObjectUpdateHandler(domainObjectUpdated);
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

        void domainObjectUpdated(object sender, EventArgs e)
        {
             Image = ActiveSerializer.Instance.Serializer.Serialize(DomainObject);
             this.Size = DomainObject.Size;
             this.Location = DomainObject.Location;
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
