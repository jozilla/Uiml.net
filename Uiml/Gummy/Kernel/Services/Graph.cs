using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Uiml.Gummy.Kernel.Services
{
    public delegate void DesignSpaceSizeChangeHandler(object sender, Size size);

    public partial class Graph : UserControl
    {
        private List<Rectangle> m_examples = new List<Rectangle>();
        private int m_selectedExample = -1;
        private Rectangle m_cursor = new Rectangle(5, 5, 10, 10);       
        private bool m_init = false;
        private bool m_cursorClicked = false;

        private Size m_minSize = new Size(130, 40);
        private Size m_maxSize = new Size(1500, 1500);

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

            Resize += new EventHandler(onResizeGraph);
            Paint += new PaintEventHandler(onPaintGraph);
            MouseDown += new MouseEventHandler(onMouseDownGraph);
            MouseMove += new MouseEventHandler(onMouseMoveGraph);
            MouseUp += new MouseEventHandler(onMouseUpGraph);
            
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
            for (int y = 0; y <= Height; y++)
                for (int x = 0; x <= Width; x++)
                {
                    Point pnt = new Point(x, y);
                    int width = m_minSize.Width + (x * m_xIncrement);
                    int height = m_minSize.Height + (y * m_yIncrement);
                    Size size = new Size(width, height);

                    m_pointToSize.Add(pnt, size);                 
                    m_sizeToPoint.Add(size, pnt);
                }            
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
            //Check if the requested size is within the boundaries
            if (size.Width < m_minSize.Width)
            {
                size.Width = m_minSize.Width;
            }
            else if (size.Width > m_maxSize.Width)
            {
                size.Width = m_minSize.Width;
            }
            
            if (size.Height > m_maxSize.Height)
            {
                size.Height = m_maxSize.Height;
            }
            else if (size.Height < m_minSize.Height)
            {
                size.Height = m_minSize.Height;
            }
            //Round the size to some neareby values that were anticipated during the preprocessing step
            /*size.Width = size.Width - (size.Width % m_xIncrement);
            size.Height = size.Height - (size.Height % m_yIncrement);*/
            //Get the right size
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
                m_cursor.Location = new Point( x, y );
                if (this.DesignSpaceCursorChanged != null)
                {
                    DesignSpaceCursorChanged(this, pointToSize(m_cursor.Location));
                }
                Refresh();
            }
        }

        
        public Size FocussedSize
        {
            get
            {
                if (m_selectedExample >= 0)
                {
                    Rectangle selectedRec = m_examples[m_selectedExample];
                    return pointToSize(selectedRec.Location);
                }
                else
                {
                    return pointToSize(new Point(m_cursor.X + m_cursor.Width / 2, m_cursor.Y + m_cursor.Height / 2));
                }
            }
            set
            {
                m_cursor.Location = sizeToPoint(value);
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
        
        void onMouseDownGraph(object sender, MouseEventArgs e)
        {
            if (m_cursor.Contains(e.Location))
            {                
                m_cursorClicked = true;
                m_selectedExample = -1;
            }
            else
            {
                for (int i = 0; i < m_examples.Count; i++)
                {
                    Rectangle rect = m_examples[i];
                    if (rect.Contains(e.Location))
                    {
                        m_selectedExample = i;
                        Refresh();
                        if (DesignSpaceExampleSelected != null)
                        {
                            DesignSpaceExampleSelected(this, pointToSize(rect.Location));
                        }
                        return;
                    }
                }
                m_selectedExample = -1;
                m_cursorClicked = false;                
            }
            Refresh();
        }


        void onPaintGraph(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            
            g.FillRectangle(Brushes.White, new Rectangle(0, 0, this.Bounds.Width, Bounds.Height));
            g.FillRectangle(Brushes.Red, m_cursor);

            for (int i = 0; i < m_examples.Count; i++)
            {
                if (i != m_selectedExample)
                {
                    g.FillRectangle(Brushes.Black, m_examples[i]);
                }
                else
                {
                    g.FillRectangle(Brushes.Green, m_examples[i]);
                }
            }

            g.FillRectangle(Brushes.Red, m_cursor);
        }

        void onResizeGraph(object sender, EventArgs e)
        {
            m_init = true;
        }

        public void CreateSnapshot()
        {
            Rectangle snapshot = new Rectangle(m_cursor.X + m_cursor.Width/2, m_cursor.Y + m_cursor.Height/2, 3, 3);
            m_examples.Add(snapshot);

            Refresh();
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
