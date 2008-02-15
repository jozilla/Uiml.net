using System;
using System.Collections.Generic;
using System.Text;

namespace Uiml.Gummy.Domain
{
    public class DomainObjectCollection : List<DomainObject>
    {
        public delegate void DomainObjectCollectionUpdatedHandler(object sender, DomainObject obj);
        public event DomainObjectCollectionUpdatedHandler DomainObjectCollectionUpdated;

        private DomainObject.DomainObjectUpdateHandler m_domainObjectUpdateHandler = null;

        public DomainObjectCollection()
            : base()
        {
            m_domainObjectUpdateHandler = new DomainObject.DomainObjectUpdateHandler(domObjUpdated);
        }

        ~DomainObjectCollection()
        {
            try
            {
                Clear();
            }
            catch { }
        }

        private void domObjUpdated(object sender, EventArgs e) 
        {
            if (DomainObjectCollectionUpdated != null)
            {
                DomainObjectCollectionUpdated(this, (DomainObject)sender);
            }
        }

        public new void Add(DomainObject d)
        {
            d.DomainObjectUpdated += m_domainObjectUpdateHandler;
            base.Add(d);
        }

        public new void AddRange(IEnumerable<DomainObject> dom)
        {
            IEnumerator<DomainObject> enumerator = dom.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.DomainObjectUpdated += m_domainObjectUpdateHandler;
            }
            base.AddRange(dom);
        }

        public new void Clear()
        {
            for (int i = 0; i < Count; i++)
            {
                base[i].DomainObjectUpdated -= m_domainObjectUpdateHandler;
            }
            base.Clear();
        }

        public new void Remove(DomainObject dom)
        {
            if (Contains(dom))
                dom.DomainObjectUpdated -= m_domainObjectUpdateHandler;
            base.Remove(dom);
        }
    }
}
