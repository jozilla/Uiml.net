using System;
using System.Collections.Generic;
using System.Text;

namespace Uiml.Gummy.Domain
{
    class DomainObjectContainer : DomainObject
    {
        List<DomainObject> m_children = new List<DomainObject>();

        public DomainObjectContainer()
            : base()
        {            
        }

        public List<DomainObject> Children
        {
            get
            {
                return m_children;
            }
        }
    }
}
