using System;
using System.Collections.Generic;
using System.Text;

using Uiml.Gummy.Domain;
using System.Xml;
using System.Reflection;

namespace Uiml.Gummy.Kernel.Services.ApplicationGlue
{
    public class ConnectedMethodsModel
    {
        protected Dictionary<MethodModel, ConnectedMethod> m_methods = new Dictionary<MethodModel, ConnectedMethod>();
        protected ReflectionBehaviorGenerator m_behaviorGen;

        public Dictionary<MethodModel, ConnectedMethod>.ValueCollection Methods
        {
            get { return m_methods.Values; }
        }

        public event EventHandler Updated;

        public ConnectedMethodsModel()
        {
            m_behaviorGen = new ReflectionBehaviorGenerator(this);
        }

        public ConnectedMethodsModel(ConnectedMethod[] methods)
        {
            foreach (ConnectedMethod m in methods)
            {
                AddIfNotExists(m.Method);
            }
        }

        public void OnUpdate(EventArgs e)
        {
            if (Updated != null)
            {
                Updated(this, e);
            }
        }

        void ChildUpdated(object sender, EventArgs e)
        {
            OnUpdate(e);
        }

        public void AddMethod(MethodModel m)
        {
            AddIfNotExists(m);
        }

        public ConnectedMethod GetMethod(MethodModel m)
        {
            if (m_methods.ContainsKey(m))
                return m_methods[m];
            else
                return null;
        }

        /*public void RegisterInput(MethodParameterModel param, DomainObject dom)
        {
            AddIfNotExists(param.Parent);
            m_methods[param.Parent].AddInput(param, dom);
        }

        public void RegisterInvoke(MethodParameterModel param, DomainObject dom)
        {
            AddIfNotExists(param.Parent);
            m_methods[param.Parent].Invoke = dom;
        }

        public void RegisterOutput(MethodParameterModel param, DomainObject dom)
        {
            AddIfNotExists(param.Parent);
            m_methods[param.Parent].Output = dom;
        }*/

        private void AddIfNotExists(MethodModel m)
        {
            if (!m_methods.ContainsKey(m))
            {
                ConnectedMethod conn = new ConnectedMethod(m);
                conn.Updated += new EventHandler(ChildUpdated);
                m_methods.Add(m, conn);

                m_behaviorGen.Update(m);
            }
        }

        public XmlNode GenerateBehavior(XmlDocument doc)
        {
            return m_behaviorGen.GenerateBehavior(doc);
        }

        public XmlNode GenerateLogic(XmlDocument doc)
        {
            return m_behaviorGen.GenerateLogic(doc);
        }
    }
}
