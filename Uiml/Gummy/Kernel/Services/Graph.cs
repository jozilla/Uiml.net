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
        //private Rectangle m_bounds = null;
        private List<Rectangle> m_examples = new List<Rectangle>();
        private int m_selectedExample = -1;
        private Rectangle m_cursor = new Rectangle(5, 5, 10, 10);       
        private bool m_init = false;
        private bool m_cursorClicked = false;

        private Size m_minSize = new Size(100, 100);
        private Size m_maxSize = new Size(800, 800);

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

        private void Graph_Load(object sender, EventArgs e)
        {
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
                    DesignSpaceCursorChanged(this, calculateNewSize(x, y));
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
                    return calculateNewSize(selectedRec.X, selectedRec.Y);
                }
                else
                {
                    return calculateNewSize(m_cursor.X + m_cursor.Width / 2, m_cursor.Y + m_cursor.Height / 2);
                }
            }
            set
            {
                m_cursor.Location = calculateNewPosition(value);
                Refresh();
            }
        }

        Point calculateNewPosition(Size size)
        {
            double x = (((double)size.Width - (double)m_minSize.Width) / (double)m_maxSize.Width) *(double)Size.Width;
            double y = (((double)size.Height - (double)m_minSize.Height) / (double)m_maxSize.Height) * (double)Size.Height;
            //double x = ((double)(m_maxSize.Width - m_minSize.Width)) / ( (double)size.Width - (double)m_minSize.Width);
            //double y = ((double)(m_maxSize.Height - m_minSize.Height)) / ((double)size.Height - (double)m_minSize.Height);

            return new Point((int)x + m_cursor.Width/2,(int)y + m_cursor.Height/2);
        }

        Size calculateNewSize(int x, int y)
        {
            double width = (double)m_minSize.Width + (((double)(m_maxSize.Width - m_minSize.Width)) * ((double)((double)x / (double)this.Width)));
            double height = (double)m_minSize.Height + ((double)(m_maxSize.Height - m_minSize.Height) * (double)((double)y / (double)this.Height));

            return new Size((int)width, (int)height);
        }

        void onMouseDownGraph(object sender, MouseEventArgs e)
        {
            if (m_cursor.Contains(e.Location))
            {
                //Console.WriteLine("ok");
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
                            DesignSpaceExampleSelected(this, calculateNewSize(rect.X, rect.Y));
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
                //Console.Out.WriteLine(m_examples[i]);
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
            //m_bounds = new Rectangle(5, 5, Size.Width, Size.Height);
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
