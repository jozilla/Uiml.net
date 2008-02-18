using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using Uiml.Gummy.Domain;

namespace Uiml.Gummy.Interpolation
{
    public abstract class InterpolationAlgorithm
    {
        DomainObject m_domainObject = null;

        public InterpolationAlgorithm(DomainObject dom)
        {
            DomainObject = dom;
        }

        public DomainObject DomainObject
        {
            get
            {
                return m_domainObject;
            }
            set
            {
                m_domainObject = value;
            }
        }

        //This function adapts the domain object properties to the new required values
        public abstract void Update(Size size);
    }
}
