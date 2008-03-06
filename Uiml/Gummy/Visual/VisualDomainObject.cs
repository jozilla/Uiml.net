using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Uiml.Gummy.Domain;
using Uiml.Gummy.Serialize;

using System.Reflection;
using Uiml.Gummy.Kernel.Services.ApplicationGlue;

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
            this.AllowDrop = true;
            DragDrop += new DragEventHandler(OnDragDrop);
            DragEnter += new DragEventHandler(OnDragEnter);
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
            DrawAnnotation(pe.Graphics);
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

        void OnDragEnter(object sender, DragEventArgs e) 
        {
            Console.WriteLine("onDragEnter");

            if (e.Data.GetDataPresent(typeof(ReflectionMethodParameterModel)) || e.Data.GetDataPresent(typeof(ReflectionMethodModel)))
                e.Effect = DragDropEffects.Link;
            else
                e.Effect = DragDropEffects.None;
        }

        void OnDragDrop(object sender, DragEventArgs e) 
        {
            if (e.Data.GetDataPresent(typeof(ReflectionMethodParameterModel)))
            {
                // link a parameter
                m_domObject.LinkMethodParameter((MethodParameterModel)e.Data.GetData(typeof(ReflectionMethodParameterModel)));
            }
            else if (e.Data.GetDataPresent(typeof(ReflectionMethodModel)))
            {
                // link a method
                m_domObject.LinkMethod((MethodModel)e.Data.GetData(typeof(ReflectionMethodModel)));
            }

            // otherwise, do nothing
        }

        protected void DrawAnnotation(Graphics g)
        {
            /*
            // draw link icon
            string connIcon = "";

            if (m_domObject.MethodLink != null)
                connIcon = "method";
            else if (m_domObject.MethodParameterLink != null)
            {
                if (m_domObject.MethodParameterLink.IsOutput)
                    connIcon = "output";
                else
                    connIcon = "input";
            }

            Bitmap icon = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream(string.Format("gummy.Uiml.Gummy.Kernel.Services.ApplicationGlue.link_{0}.png", connIcon)));

            //int padding = (int) ( ((icon.Width + icon.Height) / 2.0) / 4.0);
            int padding = 15;
            // todo: center the bastard
            Point location = new Point(this.Width - icon.Width - padding, this.Height - icon.Height - padding);
            pe.Graphics.DrawImage(icon, location);*/

            if (m_domObject.Linked) 
            {
                Size s = new Size(10, this.Height - 20);
                Point p;
                int i = 1;

                foreach (MethodParameterModel mpm in m_domObject.MethodOutputParameterLinks)
                {
                    p = new Point(this.Width - i * 10, 10);
                    g.DrawRectangle(new Pen(Color.Gray), new Rectangle(p, s));
                    g.FillRectangle(new Pen(Color.LightCoral).Brush, new Rectangle(p, s));
                    i++;
                }

                if (m_domObject.MethodLink != null)
                {
                    p = new Point(this.Width - i * 10, 10);
                    g.DrawRectangle(new Pen(Color.Gray), new Rectangle(p, s));
                    g.FillRectangle(new Pen(Color.DarkOrange).Brush, new Rectangle(p, s));
                    i++;
                }

                foreach (MethodParameterModel mpm in m_domObject.MethodInputParameterLinks)
                {
                    p = new Point(this.Width - i * 10, 10);
                    g.DrawRectangle(new Pen(Color.Gray), new Rectangle(p, s));
                    g.FillRectangle(new Pen(Color.LightGreen).Brush, new Rectangle(p, s));
                    i++;
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
