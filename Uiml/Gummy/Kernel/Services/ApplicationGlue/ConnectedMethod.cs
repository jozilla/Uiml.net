using System;
using System.Collections.Generic;
using System.Text;

using Uiml.Gummy.Domain;

namespace Uiml.Gummy.Kernel.Services.ApplicationGlue
{
    public class ConnectedMethod
    {
        private MethodModel m_method;

        public MethodModel Method
        {
            get { return m_method; }
            set { m_method = value; } 
        }

        private Dictionary<MethodParameterModel, DomainObject> m_inputs = new Dictionary<MethodParameterModel, DomainObject>();

        public Dictionary<MethodParameterModel, DomainObject> Inputs
        {
            get { return m_inputs; }
        }

        private DomainObject m_invoke;

        public DomainObject Invoke
        {
            get { return m_invoke; }
            set { m_invoke = value; }
        }

        private DomainObject m_output;

        public DomainObject Output
        {
            get { return m_output; }
            set { m_output = value; }
        }

        public ConnectedMethod(MethodModel method)
        {
            Method = method;
        }

        public void AddInput(MethodParameterModel param, DomainObject dom)
        {
            Inputs.Add(param, dom);
        }

        public bool IsComplete()
        {
            bool invoke;
            bool output;
            bool inputs;

            invoke = Invoke != null;

            inputs = true;
            foreach (MethodParameterModel input in Inputs.Keys)
            {
                if (!Method.Inputs.Contains(input))
                    inputs = false;
            }

            output = (Method.Outputs.Count > 0) ? Output != null: Output == null;

            return output && invoke && inputs;
        }
    }
}
