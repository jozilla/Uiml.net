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
        private ApplicationGlueServiceConfiguration m_config;

        public ApplicationGlueService() {
            InitializeComponent();

            m_config = new ApplicationGlueServiceConfiguration(this);
        }

        public void Init()
        {
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
            get { return "gummy-application-glue"; }
        }

        public bool IsEssential
        {
            get { return false; }
        }

        public void DrawService (Type t)
        {
            if (layout.Controls.Count > 0)
            {
                layout.Controls.Clear();
            }

            foreach (MethodInfo m in t.GetMethods(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Instance))
            {
                // ignore asynchronous methods
                bool asyncOutput = false;
                bool asyncParams = false;
                bool async = false;
                
                asyncOutput = m.ReturnType.Equals(typeof(IAsyncResult));

                foreach (ParameterInfo param in m.GetParameters())
                {
                    if (param.ParameterType.Equals(typeof(IAsyncResult)))
                        asyncParams = true;
                }

                async = asyncOutput || asyncParams;

                if (!async) 
                    DesignerKernel.Instance.CurrentDocument.Methods.AddMethod(new ReflectionMethodModel(m));
            }
            layout.Controls.Add(new ConnectedMethodsView(DesignerKernel.Instance.CurrentDocument.Methods));
        }

        public Control ServiceControl
        {
            get { return this; }
        }

        public IServiceConfiguration ServiceConfiguration
        {
            get { return m_config; }
        }

        public void NotifyConfigurationChanged()
        {
            try
            {
                DrawService(m_config.Assembly.GetTypes()[0]); // todo: check correct type
                // add to document
                DesignerKernel.Instance.CurrentDocument.Libraries.Add(m_config.Assembly);
            }
            catch
            {
                // fail silenty
            }
        }

        public void DocumentUpdated(object sender, EventArgs e)
        {
            ServiceControl.Refresh();
        }
    }
}