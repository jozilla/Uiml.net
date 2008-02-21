using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

using Uiml.Gummy.Kernel.Selected;
using Uiml.Gummy.Visual;
using Uiml.Gummy.Domain;

using UMD.HCIL.Piccolo;
using UMD.HCIL.Piccolo.Nodes;
using UMD.HCIL.PiccoloX;
using UMD.HCIL.Piccolo.Util;

namespace Uiml.Gummy.Kernel.Services
{
    public class CanvasService : Form, IService
    {
        DomainObjectCollection m_domainObjects = new DomainObjectCollection();
        Size m_canvasSize = new Size(20,20);
        Rectangle m_uiRectangle = new Rectangle(0,0,20,20);
        Point m_origin = new Point(0, 0);        
        List<Rectangle> m_modifiers = new List<Rectangle>();
        int m_boxSize = 8;
        BoxID m_clickedBox = BoxID.None;

        public event EventHandler CanvasResized;        
        private CanvasServiceConfiguration m_config;

        public CanvasService() : base()
        {
            m_config = new CanvasServiceConfiguration(this);
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
            MouseDown += new MouseEventHandler(onMouseDown);
            MouseMove += new MouseEventHandler(onMouseMove);
            MouseUp += new MouseEventHandler(onMouseUp);
            //IsMdiContainer = true;
        }

        void onMouseUp(object sender, MouseEventArgs e)
        {
            m_clickedBox = BoxID.None;
        }

        void onMouseMove(object sender, MouseEventArgs e)
        {

            if (m_clickedBox != BoxID.None)
            {
                switch (m_clickedBox)
                {
                    case BoxID.MiddleRight:
                        CanvasSize = new Size(e.X - m_origin.X, CanvasSize.Height);
                        break;
                    case BoxID.BottomMiddle:
                        CanvasSize = new Size(CanvasSize.Width,e.Y - m_origin.Y);
                        break;
                    case BoxID.BottomRight:
                        CanvasSize = new Size(e.X - m_origin.X, e.Y - m_origin.Y);
                        break;
                }
            }
        }

        void onMouseDown(object sender, MouseEventArgs e)
        {
            m_clickedBox = clickedBox(e.Location);
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
            if(m_uiRectangle.Contains(new Point(e.X,e.Y)))
            {
                DomainObject tmp = new DomainObject();            
                DomainObject dom = (DomainObject)e.Data.GetData(tmp.GetType());
                DomainObject domCloned = (DomainObject)dom.Clone();
                domCloned.Location = this.PointToClient(new Point(e.X, e.Y));
                domCloned.Identifier = DomainObjectFactory.Instance.AutoID();
                m_domainObjects.Add(domCloned);
                ExampleRepository.Instance.AddExampleDomainObject(CanvasSize, (DomainObject)domCloned.Clone());
            }
          
        }

       

        void onPaint(object sender, PaintEventArgs e)
        {            
            Graphics g = e.Graphics;
            g.FillRectangle(Brushes.WhiteSmoke,new Rectangle(0,0,Width,Height));
            Rectangle doubleBorder0 = new Rectangle(m_uiRectangle.X, m_uiRectangle.Y, m_uiRectangle.Width + 2, m_uiRectangle.Height + 2);
            Rectangle doubleBorder1 = new Rectangle(m_uiRectangle.X, m_uiRectangle.Y, m_uiRectangle.Width - 2, m_uiRectangle.Height - 2);
            g.FillRectangle(Brushes.DarkGray,m_uiRectangle);
            g.DrawRectangle(Pens.Black, m_uiRectangle);
            g.DrawRectangle(Pens.Black, doubleBorder0);
            g.DrawRectangle(Pens.Black, doubleBorder1);

            recalculateModifiers();
            foreach (Rectangle r in m_modifiers)
            {
                g.FillRectangle(Brushes.DarkGray, r);
                g.DrawRectangle(Pens.Black, r);
            }
        }

        BoxID clickedBox(Point p)
        {
            for (int i = 0; i < m_modifiers.Count; i++)
            {
                if(m_modifiers[i].Contains(p))
                    switch (i)
                    {
                        case 0:
                            return BoxID.BottomMiddle;
                        case 1:
                            return BoxID.BottomRight;
                        case 2:
                            return BoxID.MiddleRight;
                    }
            }
            return BoxID.None;
        }

        void recalculateModifiers()
        {
            m_modifiers.Clear();
            m_modifiers.Add(new Rectangle(CanvasSize.Width / 2 - m_boxSize / 2, CanvasSize.Height - m_boxSize/2, m_boxSize, m_boxSize));
            m_modifiers.Add(new Rectangle(CanvasSize.Width - m_boxSize/2, CanvasSize.Height - m_boxSize/2, m_boxSize, m_boxSize));
            m_modifiers.Add(new Rectangle(CanvasSize.Width - m_boxSize/2, CanvasSize.Height / 2 - m_boxSize / 2, m_boxSize, m_boxSize));            
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
                if (CanvasResized != null)
                    CanvasResized(this, new EventArgs());
                Refresh();
            }
        }


        public IServiceConfiguration ServiceConfiguration
        {
            get 
            {
                return m_config;
            }
        }

        public void NotifyConfigurationChanged()
        {
            this.Size = m_config.ScreenSize;
        }
    }
}
