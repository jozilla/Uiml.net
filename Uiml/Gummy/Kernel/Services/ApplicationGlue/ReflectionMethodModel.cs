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

            foreach (ParameterInfo pi in m.GetParameters())
            {
                inputs.Add(new ReflectionMethodParameterModel(pi, false, this));
            }

            // ignore void parameters
            if (!m.ReturnType.Equals(typeof(void)))
                outputs.Add(new ReflectionMethodParameterModel(m.ReturnParameter, true, this));
        }
    }
}
