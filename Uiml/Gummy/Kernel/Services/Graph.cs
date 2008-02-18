using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Uiml.Gummy.Domain;

namespace Uiml.Gummy.Kernel.Services
{
    public delegate void DesignSpaceSizeChangeHandler(object sender, Size size);

    public partial class Graph : UserControl
    {        
        private List<Rectangle> m_examples = new List<Rectangle>();
        private int m_selectedExample = -1;
        private Rectangle m_cursor = new Rectangle(12, 12, 16, 16);
        private bool m_init = false;
        private bool m_cursorClicked = false;

        private Size m_minSize = new Size(130, 40);
        private Size m_maxSize = new Size(950, 950);

        private int m_xIncrement = 1;
        private int m_yIncrement = 1;
        private Dictionary<Size,Point> m_sizeToPoint = new Dictionary<Size,Point>();
        private Dictionary<Point, Size> m_pointToSize = new Dictionary<Point, Size>();

        public event DesignSpaceSizeChangeHandler DesignSpaceCursorChanged;
        public event DesignSpaceSizeChangeHandler DesignSpaceExampleSelected;

        public Graph()
        {
            InitializeComponent();
            this.DoubleBuffered = true;

            Paint += new PaintEventHandler(onPaintGraph);
            MouseDown += new MouseEventHandler(onMouseDownGraph);
            MouseMove += new MouseEventHandler(onMouseMoveGraph);
            MouseUp += new MouseEventHandler(onMouseUpGraph);
            ExampleRepository.Instance.ExampleDesignAdded += new ExampleRepository.ExampleDesignAddedHandler(onExampleDesignAdded);            
        }

        private void onExampleDesignAdded(object sender, Size s)
        {
            Point p = sizeToPoint(s);
            m_examples.Add(new Rectangle(p.X - 3, p.Y - 3, 6, 6));
            updateSelectedExample();
            Refresh();
        }

        /*
         * This method needs to be called before the graph can work properly
         * -> The internal datastructures will be preprocessed
         */
        public void InitGraph()
        {
            //Clean the old values in the hashtables
            m_pointToSize.Clear();
            m_sizeToPoint.Clear();
            //Get the right size steps such that there is a good point-size relationship possible
            m_xIncrement = (int)Math.Ceiling( (float)(m_maxSize.Width - m_minSize.Width) / (float)Width );
            m_yIncrement = (int)Math.Ceiling((float)(m_maxSize.Height - m_minSize.Height) / (float)Height);
            //Round the minimal and maximal edges
            m_minSize.Width = m_minSize.Width - (m_minSize.Width % m_xIncrement);
            m_maxSize.Width = m_maxSize.Width - (m_maxSize.Width % m_xIncrement);
            m_minSize.Height = m_minSize.Height - (m_minSize.Height % m_yIncrement);
            m_maxSize.Height = m_maxSize.Height - (m_maxSize.Height % m_yIncrement);

            //Initialize the hashtables
            int maxWidth = 0;
            int maxHeight = 0;
            for (int y = 0; y <= Height; y++)
                for (int x = 0; x <= Width; x++)
                {
                    Point pnt = new Point(x, y);
                    int width = m_minSize.Width + (x * m_xIncrement);
                    int height = m_minSize.Height + (y * m_yIncrement);
                    Size size = new Size(width, height);
                    if (width > maxWidth)
                        maxWidth = width;
                    if (height > maxHeight)
                        maxHeight = height;

                    m_pointToSize.Add(pnt, size);                 
                    m_sizeToPoint.Add(size, pnt);
                }
            m_maxSize.Height = maxHeight - (3 * m_yIncrement);
            m_maxSize.Width = maxWidth - (3 * m_xIncrement);
        }

        private Size pointToSize(Point pnt)
        {
            /*
             * Check if it's a valid point
             */
            if (pnt.X >= Width)
            {
                pnt.X = Width;
            }
            else if (pnt.X < 0)
            {
                pnt.X = 0;
            }
            if (pnt.Y >= Height)
            {
                pnt.Y = Height - 1;
            }
            else if (pnt.Y < 0)
            {
                pnt.Y = 0;
            }
            //Get the right point           
            return m_pointToSize[pnt];
        }

        private Point sizeToPoint(Size size)
        {
            //Check if the requested size is within the boundaries --> HERE IS AN ERROR !!    
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

            return m_sizeToPoint[size];
        }

        void onMouseUpGraph(object sender, MouseEventArgs e)
        {
            m_cursorClicked = false;
        }

        void onMouseMoveGraph(object sender, MouseEventArgs e)
        {
            if (m_cursorClicked)
            {
                int x = e.Location.X - m_cursor.Width / 2;
                int y = e.Location.Y - m_cursor.Height / 2;
                if (x > Width - m_cursor.Width)
                    x = Width - m_cursor.Width;
                else if (x < 0)
                    x = 0;
                if (y > Height - m_cursor.Height)
                    y = Height - m_cursor.Height;
                else if (y < 0)
                    y = 0;
                m_cursor.Location = new Point( x, y );
                updateSelectedExample();
                if (this.DesignSpaceCursorChanged != null)
                {
                    DesignSpaceCursorChanged(this, pointToSize(CursorPosition));
                }
                Refresh();
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
                return pointToSize(CursorPosition);
            }
            set
            {
                CursorPosition = sizeToPoint(value);
                updateSelectedExample();
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

        void updateSelectedExample()
        {
            for (int i = 0; i < m_examples.Count; i++)
            {
                Rectangle rect = m_examples[i];
                if (rect.Contains(CursorPosition))
                {
                    m_selectedExample = i;
                    int centerX = rect.X + rect.Width / 2;
                    int centerY = rect.Y + rect.Height / 2;
                    CursorPosition = new Point(centerX, centerY);
                    Refresh();                    
                    return;
                }
            }
            m_selectedExample = -1;
        }
        
        void onMouseDownGraph(object sender, MouseEventArgs e)
        {            
            m_cursorClicked = true;
            CursorPosition = e.Location;
            updateSelectedExample();
            Refresh();
            if (this.DesignSpaceCursorChanged != null)
            {
                DesignSpaceCursorChanged(this, pointToSize(CursorPosition));
            }
        }

        Point CursorPosition
        {
            get
            {
                Point position = new Point(m_cursor.X + m_cursor.Width / 2, m_cursor.Y + m_cursor.Height / 2);                
                return position;
            }
            set
            {
                Point position = new Point(value.X - m_cursor.Width / 2, value.Y - m_cursor.Height / 2);               
                m_cursor.Location = position;
            }
        }


        void onPaintGraph(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            
            g.FillRectangle(Brushes.White, new Rectangle(0, 0, this.Bounds.Width, Bounds.Height));

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

            SolidBrush semiTransUIBrush = new SolidBrush(Color.FromArgb(50,Color.Gray));
            SolidBrush semiTransBrush = new SolidBrush(Color.FromArgb(50, 255, 0, 0));

            g.FillRectangle(semiTransBrush, m_cursor);

            g.FillRectangle(semiTransUIBrush, 0, 0, CursorPosition.X, CursorPosition.Y);
            g.DrawRectangle(Pens.Black, 0, 0, CursorPosition.X, CursorPosition.Y);
            g.DrawRectangle(Pens.Black, m_cursor);
            g.DrawLine(Pens.Black, m_cursor.X + m_cursor.Width / 2, m_cursor.Y, m_cursor.X + m_cursor.Width / 2, m_cursor.Y + m_cursor.Height);
            g.DrawLine(Pens.Black, m_cursor.X, m_cursor.Y + m_cursor.Height / 2, m_cursor.X + m_cursor.Width, m_cursor.Y + m_cursor.Height / 2);
            
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Graph
            // 
            this.Name = "Graph";
            this.Load += new System.EventHandler(this.Graph_Load_1);
            this.ResumeLayout(false);

        }

        private void Graph_Load_1(object sender, EventArgs e)
        {

        }

    }
}
