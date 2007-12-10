using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Reflection;
using Uiml.Gummy.Kernel.Services.ApplicationGlue;

namespace Uiml.Gummy.Kernel.Services {
    public partial class ApplicationGlueService : Form, IService {
        public ApplicationGlueService() {
            InitializeComponent();
        }

        public void Init()
        {
            DrawService(typeof(Uiml.Gummy.Kernel.DesignerKernel));
        }

        public void DrawService (Type t)
        {
            List<MethodModel> methods = new List<MethodModel>();

            foreach (MethodInfo m in t.GetMethods())
            {
                methods.Add(new ReflectionMethodModel(m));
            }
            layout.Controls.Add(new MethodsView(new MethodsModel(methods.ToArray())));
        }
    }
}