using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Uiml.Gummy.Domain;
using Uiml.Gummy.Kernel.Services.Commands;

namespace Uiml.Gummy.Kernel.Services.Controls
{
    public delegate void SizeChangeHandler(object sender, Size size);    

    public partial class CartesianGraph : UserControl
    {        
        private List<Rectangle> m_examples = new List<Rectangle>();
        private List<Rectangle> m_highlightedExamples = new List<Rectangle>();
        DesignSpaceData m_designSpaceData = null;
        private int m_selectedExample = -1;
        private Rectangle m_cursor = new Rectangle(12, 12, 16, 16);

        public event SizeChangeHandler DesignSpaceCursorChanged;        
        
        private Shape.ShapUpdateHandler m_shapeUpdateHandler = null;
        private DomainObject m_selected = null;

        private CartesianGraphState m_graphState = null;
        public PaintEventHandler CartesianGraphPaint;

        public CartesianGraph(DesignSpaceData spaceData)
        {
            m_designSpaceData = spaceData;
            InitializeComponent();            
            this.DoubleBuffered = true;
            m_shapeUpdateHandler = new Shape.ShapUpdateHandler(onShapeUpdated);
            Paint += new PaintEventHandler(onPaintGraph);
            MouseDown += new MouseEventHandler(onMouseDownGraph);
            
            ExampleRepository.Instance.ExampleDesignAdded += new ExampleRepository.ExampleDesignAddedHandler(onExampleDesignAdded);
            m_graphState = new NavigateCartesianGraphState(this);
        }

        private void onExampleDesignAdded(object sender, Size s)
        {
            Point p = SizeToPoint(s);
            m_examples.Add(new Rectangle(p.X - 3, p.Y - 3, 6, 6));
            snapCursorToTheSelectedExample();
            updateHighlightedExamples();
            Refresh();
        }

        /*
         * This method needs to be called before the graph can work properly
         * -> The internal datastructures will be preprocessed
         */
        public void InitGraph()
        {            
            m_designSpaceData.MaximumPoint = new Point(Width - 90, Height - 20);
            m_designSpaceData.InitDesignSpace(Width, Height);            
            Selected.SelectedDomainObject.Instance.DomainObjectSelected += new Uiml.Gummy.Kernel.Selected.SelectedDomainObject.DomainObjectSelectedHandler(domainObjectSelected);
        }

        void domainObjectSelected(DomainObject dom, EventArgs e)
        {
            if (m_selected != null)
            {
                m_selected.Polygon.ShapeUpdated -= m_shapeUpdateHandler;
            }
            dom.Polygon.ShapeUpdated += m_shapeUpdateHandler;
            m_selected = dom;
            updateHighlightedExamples();
            Refresh();
        }

        void updateHighlightedExamples()
        {
            //Build up the highlighted examples
            if (m_selected == null)
                return;
            m_highlightedExamples.Clear();
            Dictionary<Size, DomainObject> dict = ExampleRepository.Instance.GetDomainObjectExamples(m_selected.Identifier);
            if (dict != null)
            {
                Dictionary<Size, DomainObject>.Enumerator dictEnumerator = dict.GetEnumerator();
                while (dictEnumerator.MoveNext())
                {
                    Point rectLocation = SizeToPoint(dictEnumerator.Current.Key);
                    Rectangle rect = new Rectangle(rectLocation.X - 15, rectLocation.Y - 15, 30, 30);
                    m_highlightedExamples.Add(rect);
                }
            }
        }

        void onShapeUpdated(Shape.IShape shape)
        {
            Refresh();
        }

        private void fireDesignSpaceCursorChanged()
        {            
            if (this.DesignSpaceCursorChanged != null)
            {
                DesignSpaceCursorChanged(this, PointToSize(CursorPosition));
            }            
        }

        public Size PointToSize(Point pnt)
        {
            return m_designSpaceData.PointToSize(pnt);
        }

        public Point SizeToPoint(Size size)
        {
            return m_designSpaceData.SizeToPoint(size);
        }

        void onMouseDownGraph(object sender, MouseEventArgs e)
        {
            //Show the context menu?
            if (e.Button == MouseButtons.Right)
            {
                Rectangle rect = detectSelectedExample(e.Location);
                if (rect != Rectangle.Empty)
                {
                    if (m_selectedExample == -1 || m_examples[m_selectedExample] != rect)
                    {
                        ContextMenu cMenu = new ContextMenu();
                        List<ICommand> commands = new List<ICommand>();                        
                        commands.Add(new ShowWireFrameExample(PointToSize(new Point(rect.Location.X + rect.Width / 2, rect.Location.Y + rect.Height / 2))));
                        Menu menu = (Menu)cMenu;
                        MenuFactory.CreateMenu(commands, ref menu);
                        cMenu.Show(this, e.Location);
                    }
                }
            }
        }
        
        public Size FocussedSize
        {
            get
            {
                return PointToSize(CursorPosition);
            }
            set
            {                
                CursorPosition = SizeToPoint(value);
                snapCursorToTheSelectedExample();
                fireDesignSpaceCursorChanged();
                Refresh();
            }
        }

        public int XStep
        {
            get
            {
                return m_designSpaceData.XIncrement;
            }
        }

        public int YStep
        {
            get
            {
                return m_designSpaceData.YIncrement;
            }
        }

        Rectangle detectSelectedExample(Point pnt)
        {
            for (int i = 0; i < m_examples.Count; i++)
            {
                Rectangle rect = m_examples[i];
                if (rect.Contains(pnt))
                {
                    return rect;
                }
            }
            return Rectangle.Empty;
        }

        void snapCursorToTheSelectedExample()
        {
            Rectangle rect = detectSelectedExample(CursorPosition);
            if (rect != Rectangle.Empty)
            {
                int centerX = rect.X + rect.Width / 2;
                int centerY = rect.Y + rect.Height / 2;                
                m_selectedExample = m_examples.IndexOf(rect);
                m_cursor.Location = new Point(centerX - m_cursor.Width / 2, centerY - m_cursor.Height / 2);
                Refresh();
                return;
            }
            m_selectedExample = -1;
         }
        
        public Point CursorPosition
        {
            get
            {
                Point position = new Point(m_cursor.X + m_cursor.Width / 2, m_cursor.Y + m_cursor.Height / 2);              
                return position;
            }
            set
            {
                Point position = new Point(value.X - m_cursor.Width / 2, value.Y - m_cursor.Height / 2);
                //Check if this position is not out of the boundaries                                
                if (position.X > m_designSpaceData.MaximumPoint.X - m_cursor.Width / 2)
                    position.X = m_designSpaceData.MaximumPoint.X - m_cursor.Width / 2;
                else if (position.X < m_designSpaceData.OriginPoint.X - m_cursor.Width / 2)
                    position.X = m_designSpaceData.OriginPoint.X - m_cursor.Width / 2;
                if (position.Y > m_designSpaceData.MaximumPoint.Y - m_cursor.Height / 2)
                    position.Y = m_designSpaceData.MaximumPoint.Y - m_cursor.Height / 2;
                else if (position.Y < m_designSpaceData.OriginPoint.Y - m_cursor.Height / 2)
                    position.Y = m_designSpaceData.OriginPoint.Y - m_cursor.Height / 2;               
                m_cursor.Location = position;
                //snap the cursor to the undelying example
                snapCursorToTheSelectedExample();                
                fireDesignSpaceCursorChanged();
            }
        }

        void onPaintGraph(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            
            g.FillRectangle(new SolidBrush(BackColor), new Rectangle(0, 0, this.Bounds.Width, Bounds.Height));

            g.FillRectangle(Brushes.White, m_designSpaceData.OriginPoint.X, m_designSpaceData.OriginPoint.Y, 
                m_designSpaceData.MaximumPoint.X - m_designSpaceData.MaximumPoint.X,
                m_designSpaceData.MaximumPoint.Y - m_designSpaceData.OriginPoint.Y);  
            drawXAxis(g);
            drawYAxis(g);
            g.DrawRectangle(Pens.Black, m_designSpaceData.OriginPoint.X, m_designSpaceData.OriginPoint.Y, m_designSpaceData.MaximumPoint.X - m_designSpaceData.OriginPoint.X, m_designSpaceData.MaximumPoint.Y - m_designSpaceData.OriginPoint.Y);

            if (Selected.SelectedDomainObject.Instance.Selected != null)
            {
                Selected.SelectedDomainObject.Instance.Selected.Polygon.Paint(g, m_designSpaceData.OriginPoint);
            }

            for (int i = 0; i < m_examples.Count; i++)
            {
                if (i != m_selectedExample)
                {
                    g.FillRectangle(Brushes.DarkBlue, m_examples[i]);
                }
                else
                {
                    g.FillRectangle(Brushes.YellowGreen, m_examples[i]);
                    g.DrawRectangle(Pens.Chocolate, m_examples[i]);                    
                }
            }
            SolidBrush semiTransHighlightedBrush = new SolidBrush(Color.FromArgb(30, Color.Yellow));
            for (int i = 0; i < m_highlightedExamples.Count; i++)
            {
                Rectangle current = m_highlightedExamples[i];
                g.FillEllipse(semiTransHighlightedBrush, current);
                g.DrawEllipse(Pens.Peru, current);                
            }

            if (CartesianGraphPaint != null)
                CartesianGraphPaint(this, e);

            if (Selected.SelectedDomainObject.Instance.Selected != null)
            {
                Selected.SelectedDomainObject.Instance.Selected.Polygon.Paint(g, m_designSpaceData.OriginPoint);
            }

            //SolidBrush semiTransUIBrush = new SolidBrush(Color.FromArgb(50,Color.Gray));
            SolidBrush semiTransBrush = new SolidBrush(Color.FromArgb(50, 255, 0, 0));

            g.FillRectangle(semiTransBrush, m_cursor);

            //g.FillRectangle(semiTransUIBrush, m_designSpaceData.OriginPoint.X, m_designSpaceData.OriginPoint.Y, CursorPosition.X - m_designSpaceData.OriginPoint.X, CursorPosition.Y - m_designSpaceData.OriginPoint.Y);
            g.DrawRectangle(Pens.Black, m_designSpaceData.OriginPoint.X, m_designSpaceData.OriginPoint.Y, CursorPosition.X - m_designSpaceData.OriginPoint.X, CursorPosition.Y - m_designSpaceData.OriginPoint.Y);
            g.DrawRectangle(Pens.Black, m_cursor);
            g.DrawLine(Pens.Black, m_cursor.X + m_cursor.Width / 2, m_cursor.Y, m_cursor.X + m_cursor.Width / 2, m_cursor.Y + m_cursor.Height);
            g.DrawLine(Pens.Black, m_cursor.X, m_cursor.Y + m_cursor.Height / 2, m_cursor.X + m_cursor.Width, m_cursor.Y + m_cursor.Height / 2);
                       
        }

        private void drawYAxis(Graphics g)
        {
            float x1 = (float)m_designSpaceData.OriginPoint.X * ((float)3 / (float)5);
            float x2 = (float)m_designSpaceData.OriginPoint.X * ((float)4 / (float)5);
            float x3 = (float)m_designSpaceData.OriginPoint.X * ((float)2 / (float)4);

            Font fnt = new Font("Arial", 7);
            int counter = 0;
            for (int y = m_designSpaceData.OriginPoint.Y; y <= m_designSpaceData.MaximumPoint.Y; y++)
            {
                Point pnt = new Point(m_designSpaceData.OriginPoint.X,y);
                Size size = PointToSize(pnt);
                if (counter % 30 == 0)
                {
                    g.DrawLine(Pens.Black, x1, y, m_designSpaceData.OriginPoint.X, y);
                    g.DrawLine(Pens.LightGray, m_designSpaceData.OriginPoint.X, y, m_designSpaceData.MaximumPoint.X, y);
                    Rectangle rect = new Rectangle(0,y - 5, 25, 10);
                    g.DrawString(size.Height.ToString(), fnt, Brushes.Black, rect);
                }
                else if (counter % 2 == 0)
                    g.DrawLine(Pens.Black, x2, y, m_designSpaceData.OriginPoint.X, y);                    
                counter++;
            }
        }

        private void drawXAxis(Graphics g)
        {
            float y1 = (float)m_designSpaceData.OriginPoint.Y * ((float)3 /(float)5);
            float y2 = (float)m_designSpaceData.OriginPoint.Y * ((float)4 / (float)5);
            float y3 = (float)m_designSpaceData.OriginPoint.Y * ((float)2 / (float)4);
            Font fnt = new Font("Arial", 7);
            int counter = 0;
            for (int x = m_designSpaceData.OriginPoint.X; x <= m_designSpaceData.MaximumPoint.X; x+=1)
            {
                Point pnt = new Point(x, m_designSpaceData.OriginPoint.Y);
                Size size = PointToSize(pnt);
                //g.DrawString(size.Width.ToString(),new Font(
                if (counter % 30 == 0)
                {
                    g.DrawLine(Pens.Black, x, y1, x, m_designSpaceData.OriginPoint.Y);
                    g.DrawLine(Pens.LightGray, x, m_designSpaceData.OriginPoint.Y, x, m_designSpaceData.MaximumPoint.Y);
                    int l = 20;
                    if (size.Width.ToString().Length == 1)
                        l = 10;
                    else if (size.Width.ToString().Length == 2)
                        l = 16;
                    else if (size.Width.ToString().Length == 3)
                        l = 20;
                    else
                        l = 25;
                    Rectangle rect = new Rectangle(x - l/2, 0, l, (int)y3);
                    g.DrawString(size.Width.ToString(), fnt, Brushes.Black, rect);
                }
                else if (counter % 2 == 0)
                    g.DrawLine(Pens.Black, x, y2, x, m_designSpaceData.OriginPoint.Y);
                counter++;
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // CartesianGraph
            // 
            this.Name = "CartesianGraph";
            this.Load += new System.EventHandler(this.Graph_Load_1);
            this.ResumeLayout(false);

        }

        public void SpaceModeChanged(object sender, Mode mode)
        {
            if (m_graphState != null)
                m_graphState.DestroyEvents();
            switch (mode)
            {
                case Mode.Draw:
                    m_graphState = new DrawCartesianGraphState(this);
                    break;
                case Mode.Navigate:
                    m_graphState = new NavigateCartesianGraphState(this);                    
                    break;
                case Mode.Erase:
                    m_graphState = new EraseCartesianGraphState(this);
                    break;
            }
            Refresh();
        }

        private void Graph_Load_1(object sender, EventArgs e)
        {

        }

        public Point Origin
        {
            get
            {
                return m_designSpaceData.OriginPoint;
            }
        }

    }
}
