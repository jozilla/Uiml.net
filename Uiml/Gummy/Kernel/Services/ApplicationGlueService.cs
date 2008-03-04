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
    public partial class ApplicationGlueService : Form, IService, IUimlProvider {
        private List<ConnectedMethod> m_methods;
        private IBehaviorGenerator m_gen;
        private Logic m_logic;
        private string m_logicXmlString;
        private Behavior m_behavior;
        private string m_behaviorXmlString;

        private ApplicationGlueServiceConfiguration m_config;

        public ApplicationGlueService() {
            InitializeComponent();
            m_gen = new ReflectionBehaviorGenerator();

            m_config = new ApplicationGlueServiceConfiguration(this);
        }

        public void Init()
        {
            Menu = new MainMenu();
            MenuItem service = Menu.MenuItems.Add("Logic");
            service.MenuItems.Add("Select", this.LogicSelect_Clicked);
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

            List<MethodModel> methods = new List<MethodModel>();

            foreach (MethodInfo m in t.GetMethods(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Instance))
            {
                methods.Add(new ReflectionMethodModel(m));
            }
            layout.Controls.Add(new MethodsView(new MethodsModel(methods.ToArray())));
        }

        public void LogicSelect_Clicked(object sender, EventArgs args)
        {
        }

        public List<IUimlElement> GetUimlElements()
        {
            m_logic = m_gen.GenerateLogic(out m_logicXmlString);
            m_behavior = m_gen.GenerateBehavior(out m_behaviorXmlString);

            List<IUimlElement> elements = new List<IUimlElement>();
            elements.Add(m_logic);
            elements.Add(m_behavior);
            return elements;
        }

        public List<string> GetUimlElementsXml()
        {
            List<string> xmlStrings = new List<string>();

            xmlStrings.Add(m_logicXmlString);
            xmlStrings.Add(m_behaviorXmlString);

            return xmlStrings;
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
            }
            catch
            {
                // fail silenty
            }
        }
    }
}