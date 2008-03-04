using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

using Uiml.Gummy.Kernel.Selected;
using Uiml.Gummy.Visual;
using Uiml.Gummy.Domain;
using Uiml.Gummy.Kernel.Services.Controls;

using Shape;

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
        int m_rasterBlockSize = 40;

        public event EventHandler CanvasResized;        
        private CanvasServiceConfiguration m_config;

        Size m_wireFrameSize = Size.Empty;
        List<Line> m_wireFrameLines = new List<Line>();

        SelectedDomainObject.DomainObjectSelectedHandler m_selectedHandler = null;

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
            m_domainObjects.DomainObjectCollectionUpdated += new DomainObjectCollection.DomainObjectCollectionUpdatedHandler(onDomainObjectCollectionUpdated);
            Paint += new PaintEventHandler(painting);
            
            DoubleBuffered = true;
            CanvasSize = new Size(100, 100);

            MouseDown += new MouseEventHandler(onMouseDown);
            MouseMove += new MouseEventHandler(onMouseMove);
            MouseUp += new MouseEventHandler(onMouseUp);

            m_selectedHandler = new SelectedDomainObject.DomainObjectSelectedHandler(onDomainObjectSelected);
            SelectedDomainObject.Instance.DomainObjectSelected += m_selectedHandler;
        }

        void onDomainObjectSelected(DomainObject dom, EventArgs e)
        {
            if (WireFramed)
            {
                for (int i = 0; i < m_wireFrameLines.Count; i++)
                    m_wireFrameLines[i].LineColor = DomainObject.UNSELECTED_COLOR;
                List<Line> lines = GetLinesWithLabel(dom.Identifier);
                for (int i = 0; i < lines.Count; i++)
                    lines[i].LineColor = DomainObject.SELECTED_COLOR;
            }
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
                        visDom.State = new CanvasVisualDomainObjectState();
                        Controls.Add(visDom);
                    }
                    bringLinesToFront();
                    break;
                case DomainObjectCollectionEventArgs.STATE.ONEADDED:
                    VisualDomainObject vDom = new VisualDomainObject(e.DomainObject);
                    vDom.State = new CanvasVisualDomainObjectState();
                    Controls.Add(vDom);
                    vDom.BringToFront();
                    bringLinesToFront();
                    break;
                case DomainObjectCollectionEventArgs.STATE.ONEREMOVED:
                    //FixMe: Implement removing domain objects
                    bringLinesToFront();
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
            if (m_uiRectangle.Contains(this.PointToClient(new Point(e.X, e.Y))))
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

       

        void painting(object sender, PaintEventArgs e)
        { 
            Graphics g = e.Graphics;
            g.FillRectangle(Brushes.WhiteSmoke,new Rectangle(0,0,Width,Height));
            Rectangle doubleBorder0 = new Rectangle(m_uiRectangle.X, m_uiRectangle.Y, m_uiRectangle.Width + 2, m_uiRectangle.Height + 2);
            Rectangle doubleBorder1 = new Rectangle(m_uiRectangle.X, m_uiRectangle.Y, m_uiRectangle.Width - 2, m_uiRectangle.Height - 2);
            g.FillRectangle(Brushes.DarkGray,m_uiRectangle);
            for (int i = 0; i < m_uiRectangle.Height; i += m_rasterBlockSize/4)
            {
                Pen color = Pens.LightGray;                
                if (i % m_rasterBlockSize / 2 == 0)
                    color = Pens.LightPink;
                else if (i % m_rasterBlockSize / 4 == 0)
                    color = Pens.Yellow;
                //else
                //    color = Color.Khaki;
                g.DrawLine(color, 0, i, m_uiRectangle.Width, i);
            }
            for (int i = 0; i < m_uiRectangle.Width; i += m_rasterBlockSize/4)
            {
                Pen color = Pens.LightGray;
                if (i % m_rasterBlockSize / 2 == 0)
                    color = Pens.LightPink;
                else if (i % m_rasterBlockSize / 4 == 0)
                    color = Pens.Yellow;
                g.DrawLine(color, i, 0, i, m_uiRectangle.Height);
            }
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
        public DomainObjectCollection DomainObjects
        {
            get
            {
                return m_domainObjects;
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

        //Is there a wire framed mode shown?
        public bool WireFramed
        {
            set
            {
                if (value == false)
                {
                    WireFrameSize = Size.Empty;                    
                }
                Refresh();
            }
            get
            {
                return m_wireFrameSize != Size.Empty;
            }
        }

        //Which size should be wireframed?
        public Size WireFrameSize
        {
            get
            {
                return m_wireFrameSize;
            }
            set
            {
                //Clean up old lines
                for (int i = 0; i < m_wireFrameLines.Count; i++)
                    Controls.Remove(m_wireFrameLines[i]);
                for (int i = 0; i < DomainObjects.Count; i++)
                    DomainObjects[i].Color = DomainObject.DEFAULT_COLOR;
                m_wireFrameLines.Clear();

                //Set up new set of lines
                m_wireFrameSize = value;                
                List<Shape.Line> lines = WireFrameFactory.GetWireFrames(value);
                m_wireFrameLines.AddRange(lines);
                Controls.AddRange(m_wireFrameLines.ToArray());
                bringLinesToFront();             
            }
        }

        private List<Line> GetLinesWithLabel(string label)
        {
            List<Line> list = new List<Line>();
            for (int i = 0; i < m_wireFrameLines.Count; i++)
            {
                if (m_wireFrameLines[i].Label == label)
                    list.Add(m_wireFrameLines[i]);
            }

            return list;
        }

        private void bringLinesToFront()
        {
            for (int i = 0; i < m_wireFrameLines.Count; i++)
            {
                m_wireFrameLines[i].BringToFront();
            }   
        }

        public void NotifyConfigurationChanged()
        {
            this.CanvasSize = m_config.ScreenSize;
        }
    }
}
