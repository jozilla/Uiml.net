using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Uiml.Gummy.Domain
{
    public class DomainObjectCollection : List<DomainObject>
    {
        public delegate void DomainObjectCollectionHandler(object sender, List<DomainObject> dom);

        public event DomainObjectCollectionHandler DomainObjectAdded;
        public event DomainObjectCollectionHandler DomainObjectRemoved;
        public event DomainObjectCollectionHandler DomainObjectToFront;
        public event DomainObjectCollectionHandler DomainObjectToBack;

        public DomainObjectCollection()
            : base()
        {            
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

        public new void Remove(DomainObject dom)
        {
            base.Remove(dom);            
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
