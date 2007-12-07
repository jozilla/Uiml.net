using System;
using System.Collections.Generic;
using System.Text;
using Uiml.Rendering;
using Uiml.Gummy.Domain;

namespace Uiml.Gummy.Serialize
{
    public abstract class BaseManipulator : ICloneable
    {
        protected DomainObject m_domObject = null;
        protected IRenderer m_renderer = null;

        public BaseManipulator(DomainObject dom, IRenderer renderer)
        {
            DomainObject = dom;
            m_renderer = renderer;
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

        public IRenderer Renderer
        {
            get
            {
                return m_renderer;
            }
        }

        public abstract object Clone();
    }
}
