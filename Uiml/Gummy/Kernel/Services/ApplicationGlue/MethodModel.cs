using System;
using System.Collections.Generic;
using System.Text;

using System.Reflection;

namespace Uiml.Gummy.Kernel.Services.ApplicationGlue
{
    public abstract class MethodModel
    {
        protected string name;

        public string Name
        {
            get { return name; }
        }

        protected List<MethodParameterModel> inputs = new List<MethodParameterModel>();

        public List<MethodParameterModel> Inputs
        {
            get { return inputs; }
        }

        protected List<MethodParameterModel> outputs = new List<MethodParameterModel>();

        public List<MethodParameterModel> Outputs
        {
            get { return outputs; }
        }

        protected MethodParameterModel invoke;

        public MethodParameterModel Invoke
        {
            get { return invoke; }
        }

        public event EventHandler Updated;

        public void OnUpdate(EventArgs e)
        {
            if (Updated != null)
            {
                Updated(this, e);
            }
        }
    }
}
