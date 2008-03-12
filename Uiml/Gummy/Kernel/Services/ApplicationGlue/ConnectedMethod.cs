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
        }

        public event EventHandler Updated;

        public List<MethodParameterModel> Inputs
        {
            get { return m_method.Inputs; }
        }

        public DomainObject Invoke
        {
            get { return m_method.Invoke.Link; }
        }

        public DomainObject Output
        {
            get { return m_method.Outputs[0].Link; }
        }

        public ConnectedMethod(MethodModel method)
        {
            m_method = method;
            m_method.Updated += new EventHandler(ModelUpdated);
        }

        void ModelUpdated(object sender, EventArgs e)
        {
            OnUpdate(e);
        }

        /*public void AddInput(MethodParameterModel param, DomainObject dom)
        {
            Inputs.Add(param, dom);
            OnUpdate(null);
        }*/

        public void OnUpdate(EventArgs e)
        {
            if (Updated != null)
            {
                Updated(this, e);
            }
        }

        public bool IsLinked
        {
            get 
            { 
                return m_method.Invoke.Linked || 
                    m_method.Outputs.Exists(delegate(MethodParameterModel m) { return m.Linked; }) || 
                    m_method.Inputs.Exists(delegate(MethodParameterModel m) { return m.Linked; }); 
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
            foreach (MethodParameterModel input in Inputs)
            {
                if (!input.Linked)
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
