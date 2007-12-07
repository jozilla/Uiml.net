using System;
using System.Collections.Generic;
using System.Text;

using Uiml.Gummy.Domain;

namespace Uiml.Gummy.Kernel.Selected
{
    public class SelectedDomainObject
    {

        private static SelectedDomainObject m_selectedDomObject = null;
        private DomainObject m_domObject = null;

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

        public DomainObject Selected
        {
            get
            {
                return m_domObject;
            }
            set
            {
                m_domObject = value;
                if (DomainObjectSelected != null)
                    DomainObjectSelected(m_domObject, new EventArgs());
            }
        }
    }
}
