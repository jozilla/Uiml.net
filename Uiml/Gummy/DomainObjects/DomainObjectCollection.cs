using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Drawing;

using Uiml.Gummy.Kernel;

namespace Uiml.Gummy.Domain
{
    public class DomainObjectCollection : List<DomainObject>
    {
        public delegate void DomainObjectCollectionHandler(object sender, List<DomainObject> dom);       

        public event DomainObjectCollectionHandler DomainObjectAdded;
        public event DomainObjectCollectionHandler DomainObjectRemoved;
        public event DomainObjectCollectionHandler DomainObjectToFront;
        public event DomainObjectCollectionHandler DomainObjectToBack;

        //All the domainobjects
        private List<DomainObject> m_allDomainObjects = new List<DomainObject>();
        private Document m_document = null;

        public DomainObjectCollection(Document doc)
            : base()
        {
            doc.ScreenSizeUpdated += new Document.ScreenSizeUpdateHandler(screenSizUpdated);
            m_document = doc;
        }

        void screenSizUpdated(object sender, System.Drawing.Size newSize)
        {
            Point pnt = m_document.DesignSpaceData.SizeToPoint(newSize);
            pnt = new Point(pnt.X - m_document.DesignSpaceData.OriginPoint.X, pnt.Y - m_document.DesignSpaceData.OriginPoint.Y);
            List<DomainObject> objectsAdded = new List<DomainObject>();
            List<DomainObject> objectsRemoved = new List<DomainObject>();
            int w = this.Count;
            foreach (DomainObject dom in m_allDomainObjects)
            {
                if (dom.Polygon.PointInShape(pnt) && !Contains(dom))
                {
                    base.Add(dom);
                    objectsAdded.Add(dom);
                }
                else if (!dom.Polygon.PointInShape(pnt) && Contains(dom))
                {
                    base.Remove(dom);
                    objectsRemoved.Add(dom);
                }
            }
            if (DomainObjectRemoved != null)
                DomainObjectRemoved(this, objectsRemoved);
            if (DomainObjectAdded != null)
                DomainObjectAdded(this, objectsAdded);
        }

        ~DomainObjectCollection()
        {
            try
            {
                Clear();
            }
            catch { }
        }

        public new void Add(DomainObject d)
        {
            //Small trick to be compatible with a control collection :-)
            base.Insert(0,d);
            m_allDomainObjects.Insert(0,d);
            if (DomainObjectAdded != null)
            {
                List<DomainObject> domObjects = new List<DomainObject>();
                domObjects.Add(d);
                DomainObjectAdded(this, domObjects);
            }
        }

        public new void AddRange(IEnumerable<DomainObject> dom)
        {
            IEnumerator<DomainObject> enumerator = dom.GetEnumerator();            
            base.AddRange(dom);
            if (DomainObjectAdded != null)
            {
                List<DomainObject> domObjects = new List<DomainObject>();
                domObjects.AddRange(dom);
                DomainObjectAdded(this, domObjects);
            }
        }

        public new void Clear()
        {
            if (DomainObjectRemoved != null)
            {
                DomainObjectRemoved(this, new List<DomainObject>(this));
            }
            base.Clear();            
        }

        public DomainObject Get(string label)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].Identifier == label)
                    return this[i];
            }
            return null;
        }

        public void adaptPolygon(DomainObject dom, Size size)
        {
            //Convert to point
            Point point = m_document.DesignSpaceData.SizeToPoint(m_document.CurrentSize);
            Point pnt = new Point(point.X - m_document.DesignSpaceData.OriginPoint.X, point.Y - m_document.DesignSpaceData.OriginPoint.Y);
            //Request the examples
            Dictionary<Size,DomainObject> dict = ExampleRepository.Instance.GetDomainObjectExamples(dom.Identifier);
            Dictionary<Size, DomainObject>.Enumerator dictEnum = dict.GetEnumerator();
            //If all sizes are higher then the current one : Upper bound
            //If all sizes are lower then the current one: Under bound
            //Otherwise: we could not guess how
            int allLower = 0;
            int allHigher = 0;
            while (dictEnum.MoveNext())
            {
                Size tmp = dictEnum.Current.Key;
                if (size.Width > tmp.Width && size.Height > tmp.Height)
                {
                    allHigher++;
                }
                else if (size.Width < tmp.Width && size.Height < tmp.Height)
                {
                    allLower++;
                }
            }
            if (allHigher == dict.Count)
            {
                dom.Polygon.CreateUpperEdge(pnt);
            }
            else if (allLower == dict.Count)
            {
                dom.Polygon.CreateUnderEdge(pnt);
            }
        }

        public new void Remove(DomainObject dom)
        {
            //Remove it out of the current list, but not out of the total one
            base.Remove(dom);    
            //Adapt the polygon
            adaptPolygon(dom, m_document.CurrentSize);
            if (DomainObjectRemoved != null)
            {
                List<DomainObject> domObjects = new List<DomainObject>();
                domObjects.Add(dom);
                DomainObjectRemoved(this, domObjects);
            }
        }

        public void BringForward(DomainObject dom)
        {
            if (Contains(dom) && IndexOf(dom) > 0)
            {
                int index = IndexOf(dom);
                DomainObject tmp = base[index-1];
                base[index - 1] = dom;
                base[index] = tmp;
                if (DomainObjectToFront != null)
                {
                    List<DomainObject> domObjects = new List<DomainObject>();
                    domObjects.Add(dom);
                    DomainObjectToFront(this, domObjects);
                }
            }
        }

        public void SendBackward(DomainObject dom)
        {
            if (Contains(dom) && IndexOf(dom) < Count-1)
            {
                int index = IndexOf(dom);
                DomainObject tmp = base[index + 1];
                base[index + 1] = dom;
                base[index] = tmp;                
                if (DomainObjectToBack != null)
                {
                    List<DomainObject> domObjects = new List<DomainObject>();
                    domObjects.Add(dom);
                    DomainObjectToBack(this, domObjects);
                }
            }
        }

        public void Serialize(XmlDocument doc)
        {
            foreach (DomainObject obj in this)
            {
                obj.Part.Serialize(doc);
            }

            string xml = doc.InnerXml;
        }
    }
}
