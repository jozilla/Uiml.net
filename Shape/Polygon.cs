using System;
using System.Collections.Generic;
using System.Text;

using System.Drawing;

namespace Shape
{
    public class Polygon : IShape, ICloneable
    {
        List<Point> m_points = new List<Point>();
        Point m_tmpPoint = Point.Empty;
        Point m_tmpPointSorted = Point.Empty;

        public event ShapUpdateHandler ShapeUpdated;

        public Polygon()
        {
        }
               
        public object Clone()
        {
            Polygon cloned = new Polygon();
            foreach (Point pnt in m_points)
            {
                cloned.m_points.Add(new Point(pnt.X, pnt.Y));
            }
            return cloned;
        }

        public List<Point> Points
        {
            get
            {
                return m_points;
            }
            set
            {
                m_points = value;
            }
        }

        //Temporary point that is added to the polygon, this needs to be confirmed later
        public Point TmpPoint
        {
            get
            {
                return m_tmpPoint;
            }
            set
            {
                m_tmpPoint = value;
                if (ShapeUpdated != null)
                    ShapeUpdated(this);
            }
        }
       

        public void AddTmpPoint()
        {
            AddPoint(m_tmpPoint);
            m_tmpPoint = Point.Empty;
        }

        public void AddPoint(Point pnt)
        {
            if (!m_points.Contains(pnt))
            {
                m_points.Add(pnt);
                if (ShapeUpdated != null)
                    ShapeUpdated(this);
            }
        }

        public int ClickedPoint(Point clickCoordinates)
        {
            foreach(Point pnt in Points)
            {
                Rectangle rect = new Rectangle(pnt.X - 2, pnt.Y - 2, 4, 4);
                if (rect.Contains(clickCoordinates))
                {
                    return Points.IndexOf(pnt);
                }
            }
            return -1;
        }

        public void ReplacePoint(int index, Point replacer)
        {
            if (index == -1)
                return;
            Points[index] = replacer;
            if (ShapeUpdated != null)
                ShapeUpdated(this);
        }

        public List<Point> PointsInRectangle(Rectangle rect)
        {
            List<Point> points = new List<Point>();
            foreach(Point pnt in Points)
            {
                if (rect.Contains(pnt))
                {
                    points.Add(pnt);
                }
            }
            return points;
        }

        private double distance(double x1, double y1, double x2, double y2)
        {
            double distance = Math.Sqrt(((x1 - x2) * (x1 - x2)) + ((y1 - y2) * (y1 - y2)));
            return distance;
        }        

        public void RemovePoint(Point pnt)
        {
            m_points.Remove(pnt);
            if (ShapeUpdated != null)
                ShapeUpdated(this);
        }

        public virtual void Paint(Graphics g, Point offset)
        {
            List<Point> drawPoints = new List<Point>();
            for (int i = 0; i < m_points.Count; i++)
            {
                drawPoints.Add(new Point(m_points[i].X + offset.X, m_points[i].Y + offset.Y));
            }
            //Add the temporary point...
            Point offsetTmpPoint = Point.Empty;
            if (m_tmpPoint != Point.Empty)
            {
                offsetTmpPoint = new Point(m_tmpPoint.X + offset.X, m_tmpPoint.Y + offset.Y);
                drawPoints.Add(offsetTmpPoint);
            }           
            if (drawPoints.Count > 2)
            {
                SolidBrush semiTransBrush = new SolidBrush(Color.FromArgb(50, 0, 255, 0));
                g.FillPolygon(semiTransBrush, drawPoints.ToArray());
                g.DrawPolygon(Pens.DarkBlue, drawPoints.ToArray());                
            }
            else if (drawPoints.Count == 2)
            {
                g.DrawLine(Pens.DarkBlue, drawPoints[0], drawPoints[1]);
            }
            foreach (Point pnt in drawPoints)
            {
                g.DrawRectangle(Pens.Coral, pnt.X - 2, pnt.Y - 2, 4, 4);
            }
            drawPoints.Remove(offsetTmpPoint);
        }

        public virtual bool PointInShape(Point pt)
        {
           /* Point p1, p2;

            bool inside = false;

            if (m_points.Count < 3)
            {
                return inside;
            }

            Point oldPoint = new Point(
                m_points[m_points.Count - 1].X, m_points[m_points.Count - 1].Y);

            for (int i = 0; i < m_points.Count; i++)
            {
                Point newPoint = new Point(m_points[i].X, m_points[i].Y);

                if (newPoint.X > oldPoint.X)
                {
                    p1 = oldPoint;
                    p2 = newPoint;
                }

                else
                {
                    p1 = newPoint;
                    p2 = oldPoint;
                }
                
                if ((newPoint.X < p.X) == (p.X <= oldPoint.X)
                    && ((long)p.Y - (long)p1.Y) * (long)(p2.X - p1.X)
                     < ((long)p2.Y - (long)p1.Y) * (long)(p.X - p1.X))
                {
                    inside = !inside;
                }

                oldPoint = newPoint;
            }
            return inside;*/
            bool isIn = false;


            int i, j = 0;

            // The following code is converted from a C version found at 
            // http://www.ecse.rpi.edu/Homepages/wrf/Research/Short_Notes/pnpoly.html
            for (i = 0, j = m_points.Count - 1; i < m_points.Count; j = i++)
            {
                if (
                    (
                     ((m_points[i].Y <= pt.Y) && (pt.Y < m_points[j].Y)) || ((m_points[j].Y <= pt.Y) && (pt.Y < m_points[i].Y))
                    ) &&
                    (pt.X < (m_points[j].X - m_points[i].X) * (pt.Y - m_points[i].Y) / (m_points[j].Y - m_points[i].Y) + m_points[i].X)
                   )
                {
                    isIn = !isIn;
                }
            }
        

            return isIn;
        }

        public void CreateUpperEdge(Point edge)
        {
            for (int i = 0; i <m_points.Count; i++)
            {
                Point pnt = m_points[i];
                if (pnt.X > edge.X)
                    pnt.X = edge.X;
                if (pnt.Y > edge.Y)
                    pnt.Y = edge.Y;
                m_points[i] = pnt;
            }
            if (ShapeUpdated != null)
                ShapeUpdated(this);
        }

        public void CreateUnderEdge(Point edge)
        {
            for (int i = 0; i < m_points.Count; i++)
            {
                Point pnt = m_points[i];
                if (pnt.X < edge.X)
                    pnt.X = edge.X;
                if (pnt.Y < edge.Y)
                    pnt.Y = edge.Y;
                m_points[i] = pnt;
            }
            if (ShapeUpdated != null)
                ShapeUpdated(this);
        }
    }
}
