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

        private CanvasService m_cs;

        public ApplicationGlueService(CanvasService cs) {
            m_cs = cs;
            InitializeComponent();
        }

        public void Init()
        {
            DrawService(typeof(Uiml.Gummy.Kernel.DesignerKernel));
            Button go = new Button();
            go.AutoSize = true;
            go.Text = "Create Logic";
            go.Click += new EventHandler(CreateLogic);
            layout.Controls.Add(go);
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

        public void CreateLogic(object sender, EventArgs args)
        {
            ILogicGenerator gen = new ReflectionLogicGenerator(m_cs, typeof(Uiml.Gummy.Kernel.DesignerKernel));
            gen.Generate();
        }
    }
}