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
        private int m_selectedExample = -1;
        private Rectangle m_cursor = new Rectangle(12, 12, 16, 16);

        private Point m_origin = new Point(30,36);
        private Point m_max = new Point(100, 100);        

        private Size m_minSize = new Size(0, 0);
        private Size m_maxSize = new Size(1100, 820);

        private int m_xIncrement = 1;
        private int m_yIncrement = 1;
        private Dictionary<Size, Point> m_sizeToPoint = new Dictionary<Size, Point>();
        private Dictionary<Point, Size> m_pointToSize = new Dictionary<Point, Size>();

        public event SizeChangeHandler DesignSpaceCursorChanged;        
        
        private Shape.ShapUpdateHandler m_shapeUpdateHandler = null;
        private DomainObject m_selected = null;


        private CartesianGraphState m_graphState = null;

        public CartesianGraph()
        {
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
            Refresh();
        }

        /*
         * This method needs to be called before the graph can work properly
         * -> The internal datastructures will be preprocessed
         */
        public void InitGraph()
        {
            //Set up the max point
            m_max = new Point(Width - 20, Height - 20);
            //Clean the old values in the hashtables
            m_pointToSize.Clear();
            m_sizeToPoint.Clear();
            //Get the right size steps such that there is a good point-size relationship possible (minsize = 0,0)
            m_xIncrement = (int)Math.Ceiling( (float) (m_maxSize.Width)  / (float)(Width - m_origin.X - (Width - m_max.X) ) );
            m_yIncrement = (int)Math.Ceiling( (float) (m_maxSize.Height) / (float)(Height - m_origin.Y - (Height - m_max.Y) ) );
            //Round the minimal and maximal edges
            m_minSize.Width = m_minSize.Width - (m_minSize.Width % m_xIncrement);
            m_maxSize.Width = m_maxSize.Width - (m_maxSize.Width % m_xIncrement);
            m_minSize.Height = m_minSize.Height - (m_minSize.Height % m_yIncrement);
            m_maxSize.Height = m_maxSize.Height - (m_maxSize.Height % m_yIncrement);

            //Initialize the hashtables
            for (int y = m_origin.Y; y <= m_max.Y; y++)
                for (int x = m_origin.X; x <= m_max.X; x++)
                {
                    Point pnt = new Point(x, y);
                    int width = m_minSize.Width + ( (x - m_origin.X) * m_xIncrement);
                    int height = m_minSize.Height + ( (y - m_origin.Y) * m_yIncrement);
                    Size size = new Size(width, height);
                    m_pointToSize.Add(pnt, size);
                    m_sizeToPoint.Add(size, pnt);
                }
            //Due to some rounding errors, the max-size needs to be corrected
            m_maxSize = m_pointToSize[m_max];
            Selected.SelectedDomainObject.Instance.DomainObjectSelected += new Uiml.Gummy.Kernel.Selected.SelectedDomainObject.DomainObjectSelectedHandler(domainObjectSelected);
        }

        void domainObjectSelected(DomainObject dom, EventArgs e)
        {
            if (m_selected != null)
            {
                m_selected.Shape.ShapeUpdated -= m_shapeUpdateHandler;
            }
            dom.Shape.ShapeUpdated += m_shapeUpdateHandler;
            m_selected = dom;
            Refresh();
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
            /*
             * Check if it's a valid point
             */
            if (pnt.X >= m_max.X)
            {
                pnt.X = m_max.X;
            }
            else if (pnt.X < m_origin.X)
            {
                pnt.X = m_origin.X;
            }
            if (pnt.Y >= m_max.Y)
            {
                pnt.Y = m_max.Y;
            }
            else if (pnt.Y < m_origin.Y)
            {
                pnt.Y = m_origin.Y;
            }
            //Get the right point           
            return m_pointToSize[pnt];
        }

        public Point SizeToPoint(Size size)
        {
            //Check if the requested size is within the boundaries --> HERE IS AN ERROR !!    
            Console.WriteLine("START sizeToPoint ({0}) ",size);
            if (size.Width < m_minSize.Width)
            {
                size.Width = m_minSize.Width;
            }
            else if (size.Width > m_maxSize.Width)
            {
                size.Width = m_maxSize.Width;
            }            
            if (size.Height > m_maxSize.Height)
            {
                size.Height = m_maxSize.Height;
            }
            else if (size.Height < m_minSize.Height)
            {
                size.Height = m_minSize.Height;
            }
            
            size.Width = size.Width - (size.Width % m_xIncrement);
            size.Height = size.Height - (size.Height % m_yIncrement);
            Console.WriteLine("END sizeToPoint ({0}) ", size);

            return m_sizeToPoint[size];
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

        public Size MaximumCanvasSize
        {
            get
            {
                return m_maxSize;
            }
        }

        public Size MinimumCanvasSize
        {
            get
            {
                return m_minSize;
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
                return m_xIncrement;
            }
        }

        public int YStep
        {
            get
            {
                return m_yIncrement;
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
                if (position.X > m_max.X - m_cursor.Width / 2)
                    position.X = m_max.X - m_cursor.Width / 2;
                else if (position.X < m_origin.X - m_cursor.Width / 2)
                    position.X = m_origin.X - m_cursor.Width / 2;
                if (position.Y > m_max.Y - m_cursor.Height / 2)
                    position.Y = m_max.Y - m_cursor.Height / 2;
                else if (position.Y < m_origin.Y - m_cursor.Height / 2)
                    position.Y = m_origin.Y - m_cursor.Height / 2;               
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

            g.FillRectangle(Brushes.White, m_origin.X, m_origin.Y, m_max.X - m_origin.X, m_max.Y - m_origin.Y);  
            drawXAxis(g);
            drawYAxis(g);           
            g.DrawRectangle(Pens.Black, m_origin.X, m_origin.Y, m_max.X - m_origin.X, m_max.Y - m_origin.Y);

            if (Selected.SelectedDomainObject.Instance.Selected != null)
            {
                Selected.SelectedDomainObject.Instance.Selected.Shape.Paint(g, m_origin);
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

            if (Selected.SelectedDomainObject.Instance.Selected != null)
            {
                Selected.SelectedDomainObject.Instance.Selected.Shape.Paint(g, m_origin);
            }

            SolidBrush semiTransUIBrush = new SolidBrush(Color.FromArgb(50,Color.Gray));
            SolidBrush semiTransBrush = new SolidBrush(Color.FromArgb(50, 255, 0, 0));

            g.FillRectangle(semiTransBrush, m_cursor);

            g.FillRectangle(semiTransUIBrush, m_origin.X, m_origin.Y, CursorPosition.X - m_origin.X, CursorPosition.Y - m_origin.Y);
            g.DrawRectangle(Pens.Black, m_origin.X, m_origin.Y, CursorPosition.X - m_origin.X, CursorPosition.Y - m_origin.Y);
            g.DrawRectangle(Pens.Black, m_cursor);
            g.DrawLine(Pens.Black, m_cursor.X + m_cursor.Width / 2, m_cursor.Y, m_cursor.X + m_cursor.Width / 2, m_cursor.Y + m_cursor.Height);
            g.DrawLine(Pens.Black, m_cursor.X, m_cursor.Y + m_cursor.Height / 2, m_cursor.X + m_cursor.Width, m_cursor.Y + m_cursor.Height / 2);
                       
        }

        private void drawYAxis(Graphics g)
        {
            float x1 = (float)m_origin.X * ((float)3 / (float)5);
            float x2 = (float)m_origin.X * ((float)4 / (float)5);
            float x3 = (float)m_origin.X * ((float)2 / (float)4);

            Font fnt = new Font("Arial", 7);
            int counter = 0;
            for (int y = m_origin.Y; y <= m_max.Y; y++)
            {
                Point pnt = new Point(m_origin.X,y);
                Size size = PointToSize(pnt);
                if (counter % 30 == 0)
                {
                    g.DrawLine(Pens.Black, x1, y, m_origin.X, y);
                    g.DrawLine(Pens.LightGray, m_origin.X, y, m_max.X, y);
                    Rectangle rect = new Rectangle(0,y - 5, 25, 10);
                    g.DrawString(size.Height.ToString(), fnt, Brushes.Black, rect);
                }
                else if (counter % 2 == 0)
                    g.DrawLine(Pens.Black, x2, y, m_origin.X, y);                    
                counter++;
            }
        }

        private void drawXAxis(Graphics g)
        {
            float y1 = (float)m_origin.Y * ((float)3 /(float)5);
            float y2 = (float)m_origin.Y * ((float)4 / (float)5);
            float y3 = (float)m_origin.Y * ((float)2 / (float)4);
            Font fnt = new Font("Arial", 7);
            int counter = 0;
            for (int x = m_origin.X; x <= m_max.X; x+=1)
            {
                Point pnt = new Point(x, m_origin.Y);
                Size size = PointToSize(pnt);
                //g.DrawString(size.Width.ToString(),new Font(
                if (counter % 30 == 0)
                {
                    g.DrawLine(Pens.Black, x, y1, x, m_origin.Y);
                    g.DrawLine(Pens.LightGray, x, m_origin.Y, x, m_max.Y);
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
                    g.DrawLine(Pens.Black, x, y2, x, m_origin.Y);
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
            }
        }

        private void Graph_Load_1(object sender, EventArgs e)
        {

        }

        public Point Origin
        {
            get
            {
                return m_origin;
            }
        }

    }
}
