using System;
using System.Collections.Generic;
using System.Text;

using System.Drawing;

namespace Shape
{
    public class Polygon : IShape
    {
        List<Point> m_points = new List<Point>();

        public event ShapUpdateHandler ShapeUpdated;

        public Polygon()
        {
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

        public void AddPoint(Point pnt)
        {
            m_points.Add(pnt);
            if (ShapeUpdated != null)
                ShapeUpdated(this);
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
            if (m_points.Count > 2)
            {

                SolidBrush semiTransBrush = new SolidBrush(Color.FromArgb(50, 0, 255, 0));
                g.FillPolygon(semiTransBrush, drawPoints.ToArray());
                g.DrawPolygon(Pens.DarkBlue, drawPoints.ToArray());
            }
            else if (m_points.Count == 2)
            {
                g.DrawLine(Pens.DarkBlue, drawPoints[0], drawPoints[1]);
            }
            foreach (Point pnt in drawPoints)
            {
                g.DrawRectangle(Pens.Coral, pnt.X - 2, pnt.Y - 2, 4, 4);
            }
        }

        public virtual bool PointInShape(Point p)
        {
            Point p1, p2;

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
            return inside;
        }
    }
}
