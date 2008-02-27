using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Uiml.Gummy.Kernel.Services
{
    public partial class ApplicationGlueServiceConfiguration : UserControl, IServiceConfiguration
    {
        private IService m_service;

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
    }
}
