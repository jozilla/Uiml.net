using System;
using System.Collections.Generic;

using System.Reflection;

namespace Uiml.Gummy.Kernel.Services.ApplicationGlue
{
    public class ReflectionMethodModel : MethodModel
    {
        private MethodInfo methodInfo;

        public MethodInfo MethodInfo
        {
            get { return methodInfo; }
        }

        public ReflectionMethodModel (MethodInfo m)
        {
            name = m.Name;
            methodInfo = m;

            // inputs
            foreach (ParameterInfo pi in m.GetParameters())
            {
                ReflectionMethodParameterModel input = new ReflectionMethodParameterModel(pi, false, this);
                input.Updated += new EventHandler(ParameterUpdated);
                inputs.Add(input);
            }

            // invoke
            invoke = new ReflectionMethodParameterModel(this);
            invoke.Updated += new EventHandler(ParameterUpdated);

            // output (ignore void parameters)
            if (!m.ReturnType.Equals(typeof(void)))
            {
                ReflectionMethodParameterModel output = new ReflectionMethodParameterModel(m.ReturnParameter, true, this);
                output.Updated += new EventHandler(ParameterUpdated);
                outputs.Add(output);
            }
        }

        void ParameterUpdated(object sender, EventArgs e)
        {
            OnUpdate(e);
        }
    }
}
