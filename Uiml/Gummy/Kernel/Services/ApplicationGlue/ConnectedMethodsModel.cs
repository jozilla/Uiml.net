using System;
using System.Collections.Generic;
using System.Text;

using System.Reflection;

namespace Uiml.Gummy.Kernel.Services.ApplicationGlue
{
    public class ConnectedMethodsModel
    {
        protected List<ConnectedMethod> methods = new List<ConnectedMethod>();

        public List<ConnectedMethod> Methods
        {
            get { return methods; }
            set { methods = value; }
        }

        public ConnectedMethodsModel(ConnectedMethod[] methods)
        {
            foreach (ConnectedMethod m in methods)
            {
                Methods.Add(m);
            }
        }
    }
}
