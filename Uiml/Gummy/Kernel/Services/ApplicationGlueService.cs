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
        private List<ConnectedMethod> m_methods;
        private IBehaviorGenerator m_gen;

        public ApplicationGlueService() {
            InitializeComponent();
        }

        public void Init()
        {
            DrawService(typeof(Uiml.Gummy.Kernel.DesignerKernel));
            
            Button createLogic = new Button();
            createLogic.AutoSize = true;
            createLogic.Text = "Create Logic";
            createLogic.Click += new EventHandler(CreateLogic);
            
            layout.Controls.Add(createLogic);
            Button populateMethods = new Button();
            populateMethods.Text = "Populate methods";
            populateMethods.Click += new EventHandler(PopulateMethods);
            layout.Controls.Add(populateMethods);
        }

        public bool Open()
        {
            this.Visible = true;
            return true;
        }

        public bool Close()
        {
            this.Visible = false;
            return true;
        }

        public string ServiceName
        {
            get { return "application-glue"; }
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
            m_gen = new ReflectionBehaviorGenerator();
            m_gen.GenerateLogic();
        }

        public void PopulateMethods(object sender, EventArgs args)
        {
            m_gen.GenerateBehavior();
        }
    }
}