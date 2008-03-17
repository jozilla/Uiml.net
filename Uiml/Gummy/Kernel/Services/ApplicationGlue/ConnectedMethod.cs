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

        public bool IsComplete()
        {
            bool partial = false;

            return IsComplete(out partial);
        }

        /// <summary>
        /// Checks a method to see if it is completely bound.
        /// </summary>
        /// <param name="partial">True if at least one of the method's parameters are bound</param>
        /// <returns>True if all the method is completely bound</returns>
        public bool IsComplete(out bool partial)
        {
            List<MethodParameterModel> missingInputParams = null;
            bool missingOutput = false;
            bool missingInvoke = false;

            partial = false;
            bool result = IsComplete(out missingInputParams, out missingOutput, out missingInvoke);

            if ((missingInputParams.Count == 0) || !missingOutput || !missingInvoke)
                partial = true;

            return result;
        }

        public bool IsComplete(out List<MethodParameterModel> missingInputParams, out bool missingOutput, out bool missingInvoke)
        {
            // initialization
            missingInputParams = new List<MethodParameterModel>();
            missingOutput = false;
            missingInvoke = false;

            bool invoke;
            bool outputs;
            bool inputs;

            invoke = Method.Invoke.Bound;

            if (!invoke)
                missingInvoke = true;

            inputs = true;
            foreach (MethodParameterModel input in Method.Inputs)
            {
                if (!input.Bound)
                {
                    inputs = false;
                    missingInputParams.Add(input);
                }
            }

            outputs = true;
            foreach (MethodParameterModel output in Method.Outputs)
            {
                if (!output.Bound)
                {
                    outputs = false;
                }
            }

            if (!outputs)
                missingOutput = true;

            return outputs && invoke && inputs;
        }
    }
}
