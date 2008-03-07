using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

using Uiml.Gummy.Kernel.Selected;
using Uiml.Gummy.Visual;
using Uiml.Gummy.Domain;
using Uiml.Gummy.Kernel.Services.Controls;
using Uiml.Gummy.Kernel.Services.Commands;

using Shape;

namespace Uiml.Gummy.Kernel.Services
{
    public class CanvasService : Form, IService
    {
        Rectangle m_uiRectangle = new Rectangle(0,0,20,20);
        Point m_origin = new Point(0, 0);        
        List<Rectangle> m_modifiers = new List<Rectangle>();
        int m_boxSize = 8;
        BoxID m_clickedBox = BoxID.None;
        int m_rasterBlockSize = 40;
   
        CanvasServiceConfiguration m_config;

        Size m_wireFrameSize = Size.Empty;
        List<Line> m_wireFrameLines = new List<Line>();

        SelectedDomainObject.DomainObjectSelectedHandler m_selectedHandler = null;

        List<ICommand> m_commands = new List<ICommand>();

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
            DesignerKernel.Instance.CurrentDocumentChanged += new EventHandler(currentDocumentChanged);
            Paint += new PaintEventHandler(painting);
            
            DoubleBuffered = true;
            //CanvasSize = new Size(100, 100);

            MouseDown += new MouseEventHandler(onMouseDown);
            MouseMove += new MouseEventHandler(onMouseMove);
            MouseUp += new MouseEventHandler(onMouseUp);

            m_selectedHandler = new SelectedDomainObject.DomainObjectSelectedHandler(domainObjectSelected);
            SelectedDomainObject.Instance.DomainObjectSelected += m_selectedHandler;
                       
            m_commands.Add(new PasteDomainObject());
        }

        void currentDocumentChanged(object sender, EventArgs e)
        {            
            DesignerKernel.Instance.CurrentDocument.DomainObjects.DomainObjectAdded += new DomainObjectCollection.DomainObjectCollectionHandler(domainObjectAdded);
            DesignerKernel.Instance.CurrentDocument.DomainObjects.DomainObjectRemoved += new DomainObjectCollection.DomainObjectCollectionHandler(domainObjectRemoved);
            DesignerKernel.Instance.CurrentDocument.DomainObjects.DomainObjectToBack += new DomainObjectCollection.DomainObjectCollectionHandler(domainObjectOrderChanged);
            DesignerKernel.Instance.CurrentDocument.DomainObjects.DomainObjectToFront += new DomainObjectCollection.DomainObjectCollectionHandler(domainObjectOrderChanged);
            DesignerKernel.Instance.CurrentDocument.ScreenSizeUpdated += new Document.ScreenSizeUpdateHandler(screenSizeUpdated);
        }

        void screenSizeUpdated(object sender, Size newSize)
        {
            m_uiRectangle = new Rectangle(m_origin.X, m_origin.Y, newSize.Width, newSize.Height);
            Refresh();
            DomainObjectCollection.Enumerator domEnum = DesignerKernel.Instance.CurrentDocument.DomainObjects.GetEnumerator();
            while (domEnum.MoveNext())
            {
                domEnum.Current.UpdateToNewSize(newSize);
            }
        }

        
        void domainObjectOrderChanged(object sender, List<DomainObject> dom)
        {            
            foreach (Control c in Controls)
            {
                if (c is VisualDomainObject)
                {
                    VisualDomainObject visDom = (VisualDomainObject)c;
                    if (visDom.DomainObject == dom[0])
                    {
                        Controls.SetChildIndex(visDom, DesignerKernel.Instance.CurrentDocument.DomainObjects.IndexOf(dom[0]));
                    }                    
                }
            }
        }

        void domainObjectRemoved(object sender, List<DomainObject> dom)
        {
            foreach (DomainObject d in dom)
            {
                foreach (Control c in Controls)
                {
                    if (c is VisualDomainObject)
                    {
                        VisualDomainObject visDom = (VisualDomainObject)c;
                        if (visDom.DomainObject == d)
                        {
                            Controls.Remove(c);
                            break;
                        }
                    }
                }
            }
        }

        void domainObjectAdded(object sender, List<DomainObject> dom)
        {
            foreach (DomainObject d in dom)
            {
                VisualDomainObject visDom = new VisualDomainObject(d);
                visDom.State = new CanvasVisualDomainObjectState();
                Controls.Add(visDom);
                visDom.BringToFront();
            }
        }

        void domainObjectSelected(DomainObject dom, EventArgs e)
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
                        DesignerKernel.Instance.CurrentDocument.CurrentSize = new Size(e.X - m_origin.X, DesignerKernel.Instance.CurrentDocument.CurrentSize.Height);
                        break;
                    case BoxID.BottomMiddle:
                        DesignerKernel.Instance.CurrentDocument.CurrentSize = new Size(DesignerKernel.Instance.CurrentDocument.CurrentSize.Width, e.Y - m_origin.Y);
                        break;
                    case BoxID.BottomRight:
                        DesignerKernel.Instance.CurrentDocument.CurrentSize = new Size(e.X - m_origin.X, e.Y - m_origin.Y);
                        break;
                }
            }            
        }

        void onMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                m_clickedBox = clickedBox(e.Location);
            else
            {
                ContextMenu contextMenu = new ContextMenu();
                Menu menu = (Menu)contextMenu;
                MenuFactory.CreateMenu(m_commands, ref menu);
                contextMenu.Show(this, e.Location);
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
                DesignerKernel.Instance.CurrentDocument.DomainObjects.Add(domCloned);
                ExampleRepository.Instance.AddExampleDomainObject(DesignerKernel.Instance.CurrentDocument.CurrentSize, (DomainObject)domCloned.Clone());
            }          
        }       

        void painting(object sender, PaintEventArgs e)
        { 
            Graphics g = e.Graphics;
            g.FillRectangle(Brushes.WhiteSmoke,new Rectangle(0,0,Width,Height));
            Rectangle doubleBorder0 = new Rectangle(m_uiRectangle.X, m_uiRectangle.Y, m_uiRectangle.Width + 2, m_uiRectangle.Height + 2);
            Rectangle doubleBorder1 = new Rectangle(m_uiRectangle.X, m_uiRectangle.Y, m_uiRectangle.Width - 2, m_uiRectangle.Height - 2);
            g.FillRectangle(Brushes.DarkGray,m_uiRectangle);
            for (int i = 0; i < m_uiRectangle.Height; i += m_rasterBlockSize/2)
            {
                Pen color = Pens.LightGray;                
                if (i % m_rasterBlockSize / 2 == 0)
                    color = Pens.LightBlue;             
                g.DrawLine(color, 0, i, m_uiRectangle.Width, i);
            }
            for (int i = 0; i < m_uiRectangle.Width; i += m_rasterBlockSize/2)
            {
                Pen color = Pens.LightGray;
                if (i % m_rasterBlockSize / 2 == 0)
                    color = Pens.LightBlue;
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
            m_modifiers.Add(new Rectangle(DesignerKernel.Instance.CurrentDocument.CurrentSize.Width / 2 - m_boxSize / 2, DesignerKernel.Instance.CurrentDocument.CurrentSize.Height - m_boxSize / 2, m_boxSize, m_boxSize));
            m_modifiers.Add(new Rectangle(DesignerKernel.Instance.CurrentDocument.CurrentSize.Width - m_boxSize / 2, DesignerKernel.Instance.CurrentDocument.CurrentSize.Height - m_boxSize / 2, m_boxSize, m_boxSize));
            m_modifiers.Add(new Rectangle(DesignerKernel.Instance.CurrentDocument.CurrentSize.Width - m_boxSize / 2, DesignerKernel.Instance.CurrentDocument.CurrentSize.Height / 2 - m_boxSize / 2, m_boxSize, m_boxSize));            
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
        
        /*private Size CanvasSize
        {
            get
            {
                return DesignerKernel.Instance.CurrentDocument.CurrentSize;
            }
            set
            {
                DesignerKernel.Instance.CurrentDocument.CurrentSize =               
                m_uiRectangle = new Rectangle(m_origin.X, m_origin.Y, CanvasSize.Width, CanvasSize.Height);                               
                Refresh();
            }
        }*/


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
                for (int i = 0; i < DesignerKernel.Instance.CurrentDocument.DomainObjects.Count; i++)
                    DesignerKernel.Instance.CurrentDocument.DomainObjects[i].Color = DomainObject.DEFAULT_COLOR;
                m_wireFrameLines.Clear();

                //Set up new set of lines
                m_wireFrameSize = value;                
                List<Shape.Line> lines = WireFrameFactory.GetWireFrames(value);
                m_wireFrameLines.AddRange(lines);
                Controls.AddRange(m_wireFrameLines.ToArray());
                bringLinesToFront();             
            }
        }
        
        public Point MouseLocation
        {
            get
            {
                return PointToClient(MousePosition);
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
            DesignerKernel.Instance.CurrentDocument.CurrentSize = m_config.ScreenSize;
        }
    }
}
