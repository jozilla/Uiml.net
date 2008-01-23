using System;
using System.Collections.Generic;
using System.Text;

using Uiml.Gummy.Domain;
using Uiml.Gummy.Visual;

namespace Uiml.Gummy.Kernel.Services.ApplicationGlue
{
    public abstract class LogicGenerator : ILogicGenerator
    {
        private Dictionary<string, MethodModel> m_methods = new Dictionary<string,MethodModel>();
        private Type m_type;

        public Dictionary<string, MethodModel> Methods
        {
            get { return m_methods; }
        }

        public Type Type
        {
            get { return m_type; } 
        }

        public LogicGenerator(CanvasService cs, Type t)
        {
            m_type = t;

            foreach (VisualDomainObject obj in cs.Controls)
            {
                DomainObject dom = obj.DomainObject;

                if (dom.Linked)
                {
                    if (dom.MethodLink != null)
                    {
                        m_methods.Add(dom.MethodLink.Name, dom.MethodLink);
                    }

                    foreach (MethodParameterModel mpm in dom.MethodInputParameterLinks)
                    {
                        try
                        {
                            m_methods.Add(mpm.Parent.Name, mpm.Parent);
                        }
                        catch(ArgumentException) 
                        {
                        }
                    }

                    foreach (MethodParameterModel mpm in dom.MethodOutputParameterLinks)
                    {
                        try
                        {
                            m_methods.Add(mpm.Parent.Name, mpm.Parent);
                        }
                        catch (ArgumentException) 
                        {
                        }
                    }
                }
            }
        }

        public abstract Logic Generate();
    }
}
