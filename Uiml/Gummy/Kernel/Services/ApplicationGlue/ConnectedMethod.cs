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

        public event EventHandler Updated;

        private Dictionary<MethodParameterModel, DomainObject> m_inputs = new Dictionary<MethodParameterModel, DomainObject>();

        public Dictionary<MethodParameterModel, DomainObject> Inputs
        {
            get { return m_inputs; }
        }

        private DomainObject m_invoke;

        public DomainObject Invoke
        {
            get { return m_invoke; }
            set 
            { 
                m_invoke = value;
                OnUpdate(null);
            }
        }

        private DomainObject m_output;

        public DomainObject Output
        {
            get { return m_output; }
            set 
            { 
                m_output = value;
                OnUpdate(null);
            }
        }

        public ConnectedMethod(MethodModel method)
        {
            Method = method;
        }

        public void AddInput(MethodParameterModel param, DomainObject dom)
        {
            Inputs.Add(param, dom);
            OnUpdate(null);
        }

        public void OnUpdate(EventArgs e)
        {
            if (Updated != null)
            {
                Updated(this, e);
            }
        }

        public bool IsComplete()
        {
            List<MethodParameterModel> missingInputParams = null;
            bool missingOutput = false;
            bool missingInvoke = false;
            return IsComplete(out missingInputParams, out missingOutput, out missingInvoke);
        }

        public bool IsComplete(out List<MethodParameterModel> missingInputParams, out bool missingOutput, out bool missingInvoke)
        {
            // initialization
            missingInputParams = new List<MethodParameterModel>();
            missingOutput = false;
            missingInvoke = false;

            bool invoke;
            bool output;
            bool inputs;

            invoke = Invoke != null;

            if (!invoke)
                missingInvoke = true;

            inputs = true;
            foreach (MethodParameterModel input in Inputs.Keys)
            {
                if (!Method.Inputs.Contains(input))
                {
                    inputs = false;
                    missingInputParams.Add(input);
                }
            }

            output = (Method.Outputs.Count > 0) ? Output != null: Output == null;

            if (!output)
                missingOutput = true;

            return output && invoke && inputs;
        }
    }
}
