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
                if(DomainObject != null)
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

        public bool ThumbnailCallback()
        {
            return false;
        }

        bool cycle = false;

        protected virtual void domainObjectUpdated(object sender, EventArgs e)
        {
            if (!cycle)
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
                    Size oldSize = DomainObject.Size;
                    cycle = true;
                    DomainObject.Size = new Size(30, 30);
                    Image img = (Bitmap)ActiveSerializer.Instance.Serializer.Serialize(DomainObject);
                    Image.GetThumbnailImageAbort myCallback = new Image.GetThumbnailImageAbort(ThumbnailCallback);
                    if (img.Size.Width > 30 && img.Size.Height > 30)
                    {
                        img = img.GetThumbnailImage(30, 30, myCallback, IntPtr.Zero);
                    }
                    Image = img;
                    Size = new Size(30, 30);
                    DomainObject.Size = oldSize;

                    //double ratio = (double)img.Width / (double)img.Height;
                    //Do something to keep the ratio!!!  
                    /*Image thumb = null;     
                
                    if (img.Width > img.Height)
                    {
                        int width = 30;
                        int height = Convert.ToInt32(((double)img.Height / (double)img.Width) * 30);
                        Width = 30;
                        thumb = img.GetThumbnailImage(width, height, myCallback, IntPtr.Zero);                    
                    }
                    else
                    {
                        int height = 30;
                        int width = Convert.ToInt32(((double)img.Width / (double)img.Height) * 30);
                        Width = 30;
                        thumb = img.GetThumbnailImage(width, height, myCallback, IntPtr.Zero);                    
                    }
                    Image = thumb;*/
                    //Size = new Size(30, 30);
                    //SizeMode = PictureBoxSizeMode.Zoom;
                    cycle = false;
                }
            }
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
