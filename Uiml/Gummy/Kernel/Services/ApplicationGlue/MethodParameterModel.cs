using System;
using System.Collections.Generic;
using System.Text;

using System.Reflection;
using Uiml.Gummy.Domain;

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

        protected MethodParameterType paramType;

        public MethodParameterType ParameterType
        {
            get { return paramType; }
        }

        protected MethodModel parent;

        public MethodModel Parent
        {
            get { return parent; }
        }

        protected DomainObject link;

        public DomainObject Link
        {
            get { return link; }
            set 
            { 
                link = value;
                Updated(this, null);
            }
        }

        public bool Linked
        {
            get { return Link != null; }
        }

        protected IMethodParameterBinding binding;

        public IMethodParameterBinding Binding
        {
            get { return binding; }
            set
            {
                binding = value;
                Updated(this, null);
            }
        }

        public bool Bound
        {
            get { return Binding != null; }
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

    public enum MethodParameterType
    {
        Input,
        Invoke,
        Output,
    }
}
