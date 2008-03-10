using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Uiml.Gummy.Kernel.Services
{
    public partial class ApplicationGlueServiceConfiguration : UserControl, IServiceConfiguration
    {
        private IService m_service;
        private Assembly m_assembly = null;
        private OpenFileDialog m_openfiledialog;

        public ApplicationGlueServiceConfiguration(IService parent)
        {
            m_service = parent;
            InitializeComponent();
        }

        public event ReadyStateChangedEventHandler ReadyStateChanged;

        protected virtual void OnReadyStateChanged(ReadyStateChangedEventArgs e)
        {
            if (ReadyStateChanged != null)
            {
                ReadyStateChanged(this, e);
            }
        }

        public Control ServiceConfigurationControl
        {
            get { return this; }
        }

        public bool Optional
        {
            get { return true; }
        }

        public bool Ready
        {
            get { return true; } // todo
        }

        public IService Service
        {
            get { return m_service; }
        }

        public Assembly Assembly
        {
            get { return m_assembly; }
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            m_openfiledialog = new OpenFileDialog();
            m_openfiledialog.Filter = "Services (*.wsdl,*.dll)|*.wsdl;*.dll|All|*.*";
            m_openfiledialog.Title = "Select a service";
            m_openfiledialog.ShowDialog();
            m_openfiledialog.CheckFileExists = true;
            m_openfiledialog.CheckPathExists = true;
            m_openfiledialog.Multiselect = false;

            this.service.Text = m_openfiledialog.FileName;
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            if (Path.GetExtension(m_openfiledialog.FileName) == ".dll")
            {
                m_assembly = Assembly.LoadFile(m_openfiledialog.FileName);
                loadStatus.Value = 100;
            }
            else
            {
                // do python script
            }
        }
    }
}
