using System;
using System.Collections.Generic;
using System.Text;
using Shape;
using System.Drawing;

using Uiml;

namespace Uiml.Gummy.Domain
{
    /// <summary>
    /// This class represents a set of domainobjects which are logically grouped
    /// </summary>
    public class DomainObjectGroup : DomainObject
    {
        Polygon m_groupedPolygon = new Polygon();
        List<DomainObject> m_domainObjects = new List<DomainObject>();
        Point m_minPoint = new Point(int.MaxValue, int.MaxValue);
        Point m_maxPoint = new Point(int.MinValue, int.MinValue);

        Point m_location = new Point(int.MaxValue, int.MaxValue);
        Size m_size = new Size(int.MaxValue, int.MaxValue);

        public DomainObjectGroup()
        {
        }

        public override object Clone()
        {
            DomainObjectGroup group = new DomainObjectGroup();
            group.copyInto(this);
            group.m_minPoint = m_minPoint;
            group.m_maxPoint = m_maxPoint;
            group.m_groupedPolygon = (Polygon)m_groupedPolygon.Clone();
            //group.DomainObjects = m_domainObjects;
            return group;
        }

        public void AddDomainObjectToGroup(DomainObject dom)
        {            
            m_domainObjects.Add(dom);
            dom.DomainObjectUpdated += new DomainObjectUpdateHandler(domUpdated);
            dom.DomainObjectGroup = this;
            updateBoundaries();
            //updateBoundaries(dom.Size, dom.Location);
            checkLocation();
            checkSize();
            Updated();
        }

        void domUpdated(object sender, EventArgs e)
        {
            DomainObject dom = (DomainObject)sender;
            updateBoundaries();
            //updateBoundaries(dom.Size, dom.Location);
            checkLocation();
            checkSize();
            Updated();
        }

        void updateBoundaries()
        {
            int minX = int.MaxValue;
            int minY = int.MaxValue;
            int maxX = int.MinValue;
            int maxY = int.MinValue;
            foreach (DomainObject dom in DomainObjects)
            {
                if (dom.Location.X < minX)
                    minX = dom.Location.X;
                if (dom.Location.Y < minY)
                    minY = dom.Location.Y;
                if (dom.Location.X + dom.Size.Width > maxX)
                    maxX = dom.Location.X + dom.Size.Width;
                if (dom.Location.Y + dom.Size.Height > maxY)
                    maxY = dom.Location.Y + dom.Size.Height;
            }
            m_minPoint = new Point(minX, minY);
            m_maxPoint = new Point(maxX, maxY);
        }

        void updateBoundaries(Size size, Point pnt)
        {
            Point tmpMaxPoint = new Point(pnt.X + size.Width, pnt.Y + size.Height);
            Point tmpMinPoint = pnt;
            if (tmpMinPoint.X < m_minPoint.X)
                m_minPoint.X = tmpMinPoint.X;
            if (tmpMinPoint.Y < m_minPoint.Y)
                m_minPoint.Y = tmpMinPoint.Y;
            if (tmpMaxPoint.X > m_maxPoint.X)
                m_maxPoint.X = tmpMaxPoint.X;
            if (tmpMaxPoint.Y > m_maxPoint.Y)
                m_maxPoint.Y = tmpMaxPoint.Y;
            Updated();
        }

        void checkLocation()
        {
            if (m_location.X > m_minPoint.X)
                m_location.X = m_minPoint.X;
            if (m_location.Y > m_minPoint.Y)
                m_location.Y = m_minPoint.Y;
        }

        void checkSize()
        {
            if (m_location.X + m_size.Width < m_maxPoint.X)
            {
                m_size.Width = m_maxPoint.X - m_location.X;
            }
            if (m_location.Y + m_size.Height < m_maxPoint.Y)
            {
                m_size.Height = m_maxPoint.Y - m_location.Y;
            }
        }

        public void RemoveDomainObjectFromGroup(DomainObject dom)
        {
            m_domainObjects.Remove(dom);
            updateBoundaries();
        }

        public List<DomainObject> DomainObjects
        {
            get
            {
                return m_domainObjects;
            }
        }       

        public override Size Size
        {
            get
            {
                //return new Size(m_maxPoint.X - m_minPoint.X, m_maxPoint.Y - m_minPoint.Y);
                return m_size;
            }
            set
            {
                m_size = value;
                //m_maxPoint = new Point(m_minPoint.X + value.Width, m_minPoint.Y + value.Height);
                checkSize();
                checkLocation();
                Updated();
            }
        }

        public override Point Location
        {
            get
            {
                //return m_minPoint;
                return m_location;
            }
            set
            {
                /*int deltaX = m_minPoint.X - value.X;
                int deltaY = m_minPoint.Y - value.Y;
                m_maxPoint = new Point(m_maxPoint.X - deltaX, m_maxPoint.Y - deltaY);
                m_minPoint = value;*/
                m_location = value;
                checkLocation();
                Updated();
            }
        }

        public override Polygon Polygon
        {
            get
            {
                return m_groupedPolygon;
            }
            set
            {
                m_groupedPolygon = value;
                Updated();
            }
        }

        public Image Image
        {
            get
            {                
                Bitmap bmp = new Bitmap(Size.Width, Size.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                Graphics g = Graphics.FromImage(bmp);
                //SolidBrush transBrush = new SolidBrush(Color.FromArgb(0,0,0,0));
                //g.FillRectangle(transBrush, 0, 0, bmp.Width, bmp.Height);
                g.Clear(System.Drawing.Color.Transparent);
                return bmp;
            }
        }
    }
}
