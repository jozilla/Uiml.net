using System;
using System.Collections.Generic;
using System.Text;

using Uiml.Gummy.Domain;

namespace Uiml.Gummy.Kernel.Selected
{
    public class SelectedDomainObjectEventArgs : EventArgs
    {
        public bool MultipleSelected = false;

        public SelectedDomainObjectEventArgs()
        {
        }

        public SelectedDomainObjectEventArgs(bool multiple)
        {
            MultipleSelected = multiple;
        }
    }

    public class SelectedDomainObject
    {

        private static SelectedDomainObject m_selectedDomObject = null;
        private List<DomainObject> m_domObjects = new List<DomainObject>();
        private DomainObject m_clipBoardDomainObject = null;

        public delegate void DomainObjectSelectedHandler(DomainObject dom, EventArgs e);        

        public event DomainObjectSelectedHandler DomainObjectSelected;        

        private SelectedDomainObject()
        {
        }

        public static SelectedDomainObject Instance
        {
            get
            {
                if (m_selectedDomObject == null)
                    m_selectedDomObject = new SelectedDomainObject();
                return m_selectedDomObject;
            }
        }

        public DomainObject ClipBoardDomainObject
        {
            get
            {
                return m_clipBoardDomainObject;
            }
            set
            {
                m_clipBoardDomainObject = value;
            }
        }

        private void fireSelectEvents()
        {
            foreach (DomainObject dom in m_domObjects)
            {
                if (DomainObjectSelected != null)
                    DomainObjectSelected(dom, new EventArgs());
            }
            if (m_domObjects.Count == 0)
            {
                if (DomainObjectSelected != null)
                    DomainObjectSelected(null, new EventArgs());
            }
        }

        public void AddSelectedDomainObject(DomainObject dom)
        {
            if( !m_domObjects.Contains(dom))
                m_domObjects.Add(dom);
            fireSelectEvents();
        }

        public void RemoveSelectedDomainObjects(DomainObject dom)
        {
            m_domObjects.Remove(dom);
            fireSelectEvents();
        }

        public void ClearSelection()
        {
            m_domObjects.Clear();
            fireSelectEvents();
        }

        public List<DomainObject> SelectedDomainObjects
        {
            get
            {
                return m_domObjects;
            }
        }

        public DomainObject Selected
        {
            get
            {
                if (m_domObjects.Count > 0)
                    return m_domObjects[0];
                else
                    return null;
            }
            set
            {
                m_domObjects.Clear();
                m_domObjects.Add(value);
                fireSelectEvents();
            }
        }

        public bool IsSelected
        {
            get
            {
                return m_domObjects.Count > 0;
            }
        }

        public bool MultipleSelected
        {
            get
            {
                return m_domObjects.Count > 1;
            }
        }
    }
}
