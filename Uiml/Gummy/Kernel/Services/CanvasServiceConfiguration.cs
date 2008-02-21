using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Uiml.Gummy.Kernel.Services {
    public partial class CanvasServiceConfiguration : UserControl, IServiceConfiguration {
        private uint m_width;
        private uint m_height;
        private IService m_service;

        private bool m_ready = true; // set ready to true

        public CanvasServiceConfiguration(IService parent) 
        {
            InitializeComponent();
            m_service = parent;
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
            get { return true; } // this is optional, otherwise we use a default screen size
        }

        public bool Ready
        {
            get { return m_ready; } 
        }

        public IService Service
        {
            get { return m_service; }
        }

        public void CheckIfReady()
        {
            try
            {
                m_width = uint.Parse(width.Text);
                m_height = uint.Parse(height.Text);
                m_ready = true;
            }
            catch
            {
                m_ready = false;
            }

            if (Ready)
                OnReadyStateChanged(new ReadyStateChangedEventArgs(true));
            else
                OnReadyStateChanged(new ReadyStateChangedEventArgs(false));
        }

        private void width_TextChanged(object sender, EventArgs e)
        {
            CheckIfReady();
        }

        private void height_TextChanged(object sender, EventArgs e)
        {
            CheckIfReady();
        }

        public Size ScreenSize
        {
            get { return new Size((int) m_width, (int) m_height); }
        }
    }
}
