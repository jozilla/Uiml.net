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

        public override DomainObject MoveNext()
        {
            return base.MoveNext();
        }

        private void TraversePostOrder()
        {
            for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i] is DomainObjectContainer)
                {
                    TraversePostOrder();
                }
            }
        }
    }
}
