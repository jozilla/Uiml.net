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
        Size m_canvasSize = new Size(20,20);
        Rectangle m_uiRectangle = new Rectangle(0,0,20,20);
        Point m_origin = new Point(0, 0);

        public event EventHandler CanvasResized;

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
            BackColor = Color.DarkGray;
            //Resize += new EventHandler(onResize);
            m_domainObjects.DomainObjectCollectionUpdated += new DomainObjectCollection.DomainObjectCollectionUpdatedHandler(onDomainObjectCollectionUpdated);
            Paint += new PaintEventHandler(onPaint);
            DoubleBuffered = true;
            CanvasSize = new Size(100, 100);
            //IsMdiContainer = true;
        }

        public void UpdateToNewSize()
        {
            //Update every domainobject to its new properties...
            DomainObjectCollection.Enumerator domEnum = m_domainObjects.GetEnumerator();
            while (domEnum.MoveNext())
            {
                domEnum.Current.UpdateToNewSize(CanvasSize);
            }
        }

        void onDomainObjectCollectionUpdated(object sender, DomainObjectCollectionEventArgs e)
        {
            switch (e.State)
            {
                case DomainObjectCollectionEventArgs.STATE.MOREADDED:
                case DomainObjectCollectionEventArgs.STATE.MOREREMOVED:
                    Controls.Clear();
                    for (int i = 0; i < m_domainObjects.Count; i++)
                    {
                        VisualDomainObject visDom = new VisualDomainObject(m_domainObjects[i]);
                        visDom.State = new ResizeAndMoveVisualDomainObjectState();
                        Controls.Add(visDom);
                    }
                    break;
                case DomainObjectCollectionEventArgs.STATE.ONEADDED:
                    VisualDomainObject vDom = new VisualDomainObject(e.DomainObject);
                    vDom.State = new ResizeAndMoveVisualDomainObjectState();                    
                    Controls.Add(vDom);
                    vDom.BringToFront();
                    break;
                case DomainObjectCollectionEventArgs.STATE.ONEREMOVED:
                    //FixMe: Implement removing domain objects
                    break;
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
            domCloned.Identifier = DomainObjectFactory.Instance.AutoID();
            m_domainObjects.Add(domCloned);
            ExampleRepository.Instance.AddExampleDomainObject(CanvasSize, (DomainObject)domCloned.Clone());
          
        }

        void onPaint(object sender, PaintEventArgs e)
        {            
            Graphics g = e.Graphics;
            g.FillRectangle(Brushes.DarkGray,new Rectangle(0,0,Width,Height));            
            g.FillRectangle(Brushes.WhiteSmoke,m_uiRectangle);
            g.DrawRectangle(Pens.Blue, m_uiRectangle);            
            //Font fnt = new Font("Arial", 12);
            //g.FillRectangle(Brushes.DarkBlue, rectTitle);
            //g.DrawRectangle(Pens.Black, rectTitle);
            //g.DrawString("User Interface", fnt, Brushes.White, rectTitle);
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

        //Deprecated -> may not be used anymore
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
            }
        }

        public Size CanvasSize
        {
            get
            {
                return m_canvasSize;
            }
            set
            {
                m_canvasSize = value;                
                m_uiRectangle = new Rectangle(m_origin.X, m_origin.Y, CanvasSize.Width, CanvasSize.Height);
                Refresh();
                if (CanvasResized != null)
                    CanvasResized(this, new EventArgs());                
            }
        }

    }
}
