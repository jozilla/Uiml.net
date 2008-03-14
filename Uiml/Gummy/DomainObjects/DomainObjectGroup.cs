using System;
using System.Collections.Generic;
using System.Text;
using Shape;
using System.Drawing;

using Uiml.Gummy.Kernel;
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

        Point m_location = new Point(0,0);
        Size m_size = new Size(0,0);

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
            m_location = checkLocation(m_location);
            m_size = checkSize(m_location,m_size);
            Updated();
            DesignerKernel.Instance.CurrentDocument.DomainObjects.BringForward(dom);
            DesignerKernel.Instance.CurrentDocument.DomainObjects.SendBackward(this);
        }

        void domUpdated(object sender, EventArgs e)
        {
            DomainObject dom = (DomainObject)sender;
            updateBoundaries();
            m_location = checkLocation(m_location);
            m_size = checkSize(m_location,m_size);
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

        Point checkLocation(Point location)
        {            
            if (location.X > m_minPoint.X)
            {
                location.X = m_minPoint.X;                
            }
            if (location.Y > m_minPoint.Y)
            {
                location.Y = m_minPoint.Y;                
            }
            return location;
        }

        Size checkSize(Point location, Size size)
        {
            if (location.X + size.Width < m_maxPoint.X)
                size.Width = m_maxPoint.X - location.X;
            if (location.Y + size.Height < m_maxPoint.Y)
                size.Height = m_maxPoint.Y - location.Y;
            return size;
        }
        
        void checkBounds()
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
                return m_size;
            }
            set
            {
                m_size = value;                
                m_size = checkSize(m_location,m_size);
                Updated();
            }
        }        

        public override Point Location
        {
            get
            {
                return m_location;
            }
            set
            {
                //Calculate deltas
                int deltaX = m_location.X - value.X;
                int deltaY = m_location.Y - value.Y;
                //Set and update the location
                m_location = value;
                m_location = checkLocation(m_location);
                Size tmpSize = m_size;
                Updated();
                foreach (DomainObject dom in DomainObjects)
                {
                    Point pnt = dom.Location;
                    dom.Location = new Point(pnt.X - deltaX, pnt.Y - deltaY);
                }
                
            }
        }

        public override Rectangle Bounds
        {
            get
            {
                return new Rectangle(Location, Size);
            }
            set
            {
                if (value.Location.X > m_location.X)
                {
                    int delta = 0;
                    if (value.Location.X > m_minPoint.X)
                    {
                        delta = value.Location.X - m_minPoint.X;
                        value.Size = new Size(value.Size.Width + delta, value.Size.Height);
                        value.Location = new Point(m_minPoint.X, value.Location.Y);
                    }
                }
                if (value.Location.Y > m_location.Y)
                {
                    int delta = 0;
                    if (value.Location.Y > m_minPoint.Y)
                    {
                        delta = value.Location.Y - m_minPoint.Y;
                        value.Size = new Size(value.Size.Width, value.Size.Height + delta);
                        value.Location = new Point(value.Location.X, m_minPoint.Y);
                    }
                }
                Point location = checkLocation(value.Location);
                Size size = checkSize(value.Location, value.Size);
                m_location = location;
                m_size = size;
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
