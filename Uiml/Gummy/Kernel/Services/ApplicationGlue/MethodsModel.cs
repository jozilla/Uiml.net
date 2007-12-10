using System;
using System.Collections.Generic;
using System.Text;

using System.Reflection;

namespace Uiml.Gummy.Kernel.Services.ApplicationGlue
{
    public class MethodsModel
    {
        protected List<MethodModel> methods = new List<MethodModel>();

        public List<MethodModel> Methods
        {
            get { return methods; }
            set { methods = value; }
        }

        public MethodsModel (MethodModel[] methods)
        {
            foreach (MethodModel m in methods)
            {
                Methods.Add(m);
            }
        }
    }
}
