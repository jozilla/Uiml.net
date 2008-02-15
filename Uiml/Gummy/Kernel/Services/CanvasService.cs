using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

using Uiml.Gummy.Kernel.Selected;
using Uiml.Gummy.Visual;
using Uiml.Gummy.Domain;

namespace Uiml.Gummy.Kernel.Services
{
    public class CanvasService : Form, IService
    {
        DomainObjectCollection m_domainObjects = new DomainObjectCollection();          

        public CanvasService()
            : base()
        {
        }

        public void Init()
        {
            Text = "Canvas";
            Size = new Size(400, 400);
            AllowDrop = true;           
            DragDrop += new DragEventHandler(onDragDrop);
            DragEnter += new DragEventHandler(onDragEnter);
            DragLeave += new EventHandler(onDragLeave);
            DragOver += new DragEventHandler(onDragOver);
            BackColor = Color.DarkGray;
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
                return "gummy-canvas";
            }
        }

        public System.Windows.Forms.Control ServiceControl
        {
            get
            {
                return this;
            }
        }

        void onDragLeave(object sender, EventArgs e)
        {
            Console.WriteLine("onDragLeave");
        }

        void onDragOver(object sender, DragEventArgs e)
        {
            Console.WriteLine("onDragOver");
        }

        void onDragEnter(object sender, DragEventArgs e)
        {
            Console.WriteLine("onDragEnter");
            e.Effect = DragDropEffects.Move;
        }

        void onDragDrop(object sender, DragEventArgs e)
        {
            //Fixme: isn't there a better way to visualize the drag and drop?
            DomainObject tmp = new DomainObject();            
            DomainObject dom = (DomainObject)e.Data.GetData(tmp.GetType());
            DomainObject domCloned = (DomainObject)dom.Clone();
            domCloned.Location = this.PointToClient(new Point(e.X, e.Y));           

            VisualDomainObject visDom = new VisualDomainObject(domCloned);
            visDom.State = new ResizeVisualDomainObjectState();
            Controls.Add(visDom);
            m_domainObjects.Add(domCloned);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // CanvasService
            // 
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Name = "CanvasService";
            this.Load += new System.EventHandler(this.CanvasService_Load);
            this.ResumeLayout(false);

        }

        private void CanvasService_Load(object sender, EventArgs e)
        {

        }

        public List<DomainObject> DomainObjects
        {
            get
            {
                List<DomainObject> domainObjects = new List<DomainObject>();
                for (int i = 0; i < m_domainObjects.Count; i++)
                {
                    domainObjects.Add((DomainObject)m_domainObjects[i].Clone());
                }
                return domainObjects;
            }
            set
            {
                m_domainObjects.Clear();
                m_domainObjects.AddRange(value);
                Controls.Clear();
                for (int i = 0; i < m_domainObjects.Count; i++)
                {
                    VisualDomainObject visDom = new VisualDomainObject(m_domainObjects[i]);
                    visDom.State = new ResizeVisualDomainObjectState();
                    Controls.Add(visDom);                    
                }
            }
        }

        }
}
