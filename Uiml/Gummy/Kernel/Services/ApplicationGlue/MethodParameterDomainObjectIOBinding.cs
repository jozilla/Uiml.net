using System;
using System.Collections.Generic;
using System.Text;

using Uiml.Gummy.Domain;

namespace Uiml.Gummy.Kernel.Services.ApplicationGlue
{
    public abstract class MethodParameterDomainObjectIOBinding : MethodParameterDomainObjectBinding
    {
        protected Property m_domainProperty;

        public Property Property
        {
            get { return m_domainProperty; }
        }

        public MethodParameterDomainObjectIOBinding(MethodParameterModel param, DomainObject dom, Property prop)
            : base(param, dom)
        {
            m_domainProperty = prop;
        }

    }
}
