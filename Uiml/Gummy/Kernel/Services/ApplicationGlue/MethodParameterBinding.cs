using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Uiml.Gummy.Kernel.Services.ApplicationGlue
{
    public abstract class MethodParameterBinding : IMethodParameterBinding
    {
        protected MethodParameterModel m_param;

        public MethodParameterModel Parameter
        {
            get { return m_param; }
        }

        public MethodParameterBinding(MethodParameterModel param)
        {
            m_param = param;
        }

        public abstract XmlNode GetUiml(XmlDocument doc);
        public virtual void Break()
        {
            Parameter.Binding = null;
        }
    }
}
