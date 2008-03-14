using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using Uiml.Gummy.Domain;

namespace Uiml.Gummy.Kernel.Services.ApplicationGlue
{
    public abstract class MethodParameterDomainObjectBinding : MethodParameterBinding
    {
        protected DomainObject m_domainObject;

        public DomainObject DomainObject
        {
            get { return m_domainObject; }
        }

        protected Property m_domainProperty;

        public Property Property
        {
            get { return m_domainProperty; }
        }

        public MethodParameterDomainObjectBinding(MethodParameterModel param, DomainObject dom, Property prop)
            : base(param)
        {
            m_domainObject = dom;
            m_domainProperty = prop;
        }
    }
}
