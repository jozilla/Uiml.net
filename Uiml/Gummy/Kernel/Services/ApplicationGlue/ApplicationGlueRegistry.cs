using System;
using System.Collections.Generic;
using System.Text;

using Uiml.Gummy.Domain;

namespace Uiml.Gummy.Kernel.Services.ApplicationGlue
{
    public sealed class ApplicationGlueRegistry
    {
        public static readonly ApplicationGlueRegistry Instance = new ApplicationGlueRegistry();

        private ApplicationGlueRegistry()
        {
        }

        private Dictionary<MethodModel, ConnectedMethod> m_methods = new Dictionary<MethodModel, ConnectedMethod>();

        public Dictionary<MethodModel, ConnectedMethod> Methods
        {
            get { return m_methods; }
        }

        public void RegisterInput(MethodParameterModel param, DomainObject dom)
        {
            AddIfNotExists(param.Parent);
            Methods[param.Parent].AddInput(param, dom);
        }

        public void RegisterInvoke(MethodModel method, DomainObject dom)
        {
            AddIfNotExists(method);
            Methods[method].Invoke = dom;
        }

        public void RegisterOutput(MethodParameterModel param, DomainObject dom)
        {
            AddIfNotExists(param.Parent);
            Methods[param.Parent].Output = dom;
        }

        private void AddIfNotExists(MethodModel m)
        {
            if (!Methods.ContainsKey(m))
                Methods.Add(m, new ConnectedMethod(m));
        }
    }
}
