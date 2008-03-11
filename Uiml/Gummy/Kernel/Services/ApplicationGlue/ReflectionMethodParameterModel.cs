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

        public ReflectionMethodParameterModel (ParameterInfo pi, bool isOut, MethodModel parentMethod)
        {
            type = pi.ParameterType;
            name = pi.Name;
            isOutput = isOut;
            parameterInfo = pi;
            parent = parentMethod;
        }
    }
}
