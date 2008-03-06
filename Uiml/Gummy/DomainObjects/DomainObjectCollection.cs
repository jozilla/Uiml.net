using System;
using System.Collections.Generic;
using System.Text;

namespace Uiml.Gummy.Domain
{
    public class DomainObjectCollectionEventArgs : EventArgs
    {
        public enum STATE
        {
            ONEADDED,
            MOREADDED,
            ONEREMOVED,
            MOREREMOVED,
         //   REARRANGED,
            NONE
        };

        DomainObject m_domObject = null;
        STATE m_state = STATE.NONE;

        public DomainObjectCollectionEventArgs()
            : base()
        {
        }

        public DomainObjectCollectionEventArgs(STATE state) : this()
        {
            State = state;
        }

        public DomainObjectCollectionEventArgs(STATE state, DomainObject dom) : this(state)
        {            
            DomainObject = dom;
        }

        public STATE State
        {
            get
            {
                return m_state;
            }
            set
            {
                m_state = value;
            }
        }

        public DomainObject DomainObject
        {
            get
            {
                return m_domObject;
            }
            set
            {
                m_domObject = value;
            }
        }
    }

    //DomainObjectCollection[0] = topElement, DomainObjectCollection[Count-1] = lastElement
    public class DomainObjectCollection : List<DomainObject>
    {
        //Fixme: a
        public delegate void DomainObjectCollectionUpdatedHandler(object sender, DomainObjectCollectionEventArgs e);
        public event DomainObjectCollectionUpdatedHandler DomainObjectCollectionUpdated;
        public event DomainObject.DomainObjectUpdateHandler DomainObjectUpdated;

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
            /*if (DomainObjectCollectionUpdated != null)
            {
                DomainObjectCollectionUpdated(this, (DomainObject)sender);
            }*/
        }

        private void fireDomainObjectCollectionUpdated(DomainObjectCollectionEventArgs e)
        {
            DomainObjectCollectionUpdated(this, e);
        }

        public new void Add(DomainObject d)
        {
            d.DomainObjectUpdated += m_domainObjectUpdateHandler;
            //Small trick to be compatible with a control collection :-)
            base.Insert(0,d);
            fireDomainObjectCollectionUpdated(new DomainObjectCollectionEventArgs(DomainObjectCollectionEventArgs.STATE.ONEADDED,d));
        }

        public new void AddRange(IEnumerable<DomainObject> dom)
        {
            IEnumerator<DomainObject> enumerator = dom.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.DomainObjectUpdated += m_domainObjectUpdateHandler;
            }
            base.AddRange(dom);
            fireDomainObjectCollectionUpdated(new DomainObjectCollectionEventArgs(DomainObjectCollectionEventArgs.STATE.MOREADDED));
        }

        public new void Clear()
        {
            for (int i = 0; i < Count; i++)
            {
                base[i].DomainObjectUpdated -= m_domainObjectUpdateHandler;
            }
            base.Clear();
            fireDomainObjectCollectionUpdated(new DomainObjectCollectionEventArgs(DomainObjectCollectionEventArgs.STATE.MOREREMOVED));
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
            if (Contains(dom))
                dom.DomainObjectUpdated -= m_domainObjectUpdateHandler;
            base.Remove(dom);
            fireDomainObjectCollectionUpdated(new DomainObjectCollectionEventArgs(DomainObjectCollectionEventArgs.STATE.ONEREMOVED,dom));
        }

        public void MoveUp(DomainObject dom)
        {
            if (Contains(dom) && IndexOf(dom) > 0)
            {
                int index = IndexOf(dom);
                DomainObject tmp = base[index-1];
                base[index - 1] = dom;
                base[index] = tmp;
                fireDomainObjectCollectionUpdated(new DomainObjectCollectionEventArgs(DomainObjectCollectionEventArgs.STATE.MOREADDED));
            }
        }

        public void MoveDown(DomainObject dom)
        {
            if (Contains(dom) && IndexOf(dom) < Count-1)
            {
                int index = IndexOf(dom);
                DomainObject tmp = base[index + 1];
                base[index + 1] = dom;
                base[index] = tmp;
                fireDomainObjectCollectionUpdated(new DomainObjectCollectionEventArgs(DomainObjectCollectionEventArgs.STATE.MOREADDED));
            }
        }
    }
}
