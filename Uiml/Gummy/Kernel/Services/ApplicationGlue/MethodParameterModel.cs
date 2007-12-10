using System;
using System.Collections.Generic;
using System.Text;

using System.Reflection;

namespace Uiml.Gummy.Kernel.Services.ApplicationGlue
{
    public abstract class MethodParameterModel
    {
        protected Type type;

        public Type Type
        {
            get { return type; }
        }

        protected string name;

        public string Name
        {
            get { return name; }
        }

        protected bool isOutput;

        public bool IsOutput
        {
            get { return isOutput; }
        }

        protected MethodModel parent;

        public MethodModel Parent
        {
            get { return parent; }
        }
    }
}
