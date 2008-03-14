using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Uiml.Gummy.Kernel
{
    public class DesignSpaceData
    {
        //The minimum size of the design space
        private Size m_minSize = new Size(0, 0);
        //The maximum size of the design space
        private Size m_maxSize = new Size(1200, 1200);
        //The necessary hashtables to transfer between sizes and points
        private Dictionary<Size, Point> m_sizeToPoint = new Dictionary<Size, Point>();
        private Dictionary<Point, Size> m_pointToSize = new Dictionary<Point, Size>();
        //The origin of the designspace
        private Point m_origin = new Point(30, 36);
        //The maximal point of the designspace
        private Point m_max = new Point(280, 300);
        //The x and y steps the user interface may resize
        private int m_xIncrement = 1;
        private int m_yIncrement = 1;

        public DesignSpaceData()
        {
            //Fixme : Get size and width from config file
            InitDesignSpace(260, 280);
        }

        public void InitDesignSpace(int _width, int _height)
        {
            //Clean the old values in the hashtables
            m_pointToSize.Clear();
            m_sizeToPoint.Clear();
            //Get the right size steps such that there is a good point-size relationship possible (minsize = 0,0)
            m_xIncrement = (int)Math.Ceiling((float)(m_maxSize.Width) / (float)(_width - m_origin.X - (_width - m_max.X)));
            m_yIncrement = (int)Math.Ceiling((float)(m_maxSize.Height) / (float)(_height - m_origin.Y - (_height - m_max.Y)));
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
                    int width = m_minSize.Width + ((x - m_origin.X) * m_xIncrement);
                    int height = m_minSize.Height + ((y - m_origin.Y) * m_yIncrement);
                    Size size = new Size(width, height);
                    m_pointToSize.Add(pnt, size);
                    m_sizeToPoint.Add(size, pnt);
                }
            //Due to some rounding errors, the max-size needs to be corrected
            m_maxSize = m_pointToSize[m_max];
        }

        public Size MinimumSize
        {
            get
            {
                return m_minSize;
            }
            set
            {
                m_minSize = value;
            }
        }

        public Size MaximumSize
        {
            get
            {
                return m_maxSize;
            }
            set
            {
                m_maxSize = value;
            }
        }

        public Point OriginPoint
        {
            get
            {
                return m_origin;
            }
            set
            {
                m_origin = value;
            }
        }

        public Point MaximumPoint
        {
            get
            {
                return m_max;
            }
            set
            {
                m_max = value;
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
            Console.WriteLine("START sizeToPoint ({0}) ", size);
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

        public int XIncrement
        {
            get
            {
                return m_xIncrement;
            }
        }

        public int YIncrement
        {
            get
            {
                return m_yIncrement;
            }
        }
    }
}
