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

        public MethodParameterDomainObjectBinding(MethodParameterModel param, DomainObject dom)
            : base(param)
        {
            m_domainObject = dom;
        }

        public override void Break()
        {
            base.Break();
            DomainObject.BreakBinding(this);
        }
    }
}
