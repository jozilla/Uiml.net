using System;
using System.Collections.Generic;
using System.Text;

using System.Reflection;

namespace Uiml.Gummy.Kernel.Services.ApplicationGlue
{
    public class ReflectionMethodParameterModel : MethodParameterModel
    {
        private ParameterInfo parameterInfo;

        public ParameterInfo ParameterInfo
        {
            get { return parameterInfo; }
        }

        public ReflectionMethodParameterModel(MethodModel parentMethod)
        {
            // invocation

            name = parentMethod.Name;
            paramType = MethodParameterType.Invoke;
            parent = parentMethod;
        }

        public ReflectionMethodParameterModel (ParameterInfo pi, bool isOut, MethodModel parentMethod)
        {
            // inputs or outputs

            type = pi.ParameterType;
            name = pi.Name;
            
            if (isOut)
                paramType = MethodParameterType.Output;
            else
                paramType = MethodParameterType.Input;

            parameterInfo = pi;
            parent = parentMethod;
        }
    }
}
