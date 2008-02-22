using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Uiml.Gummy.Kernel.Services
{
    public partial class ToolboxServiceConfiguration : UserControl, IServiceConfiguration
    {
        private Dictionary<string, string[]> platforms = new Dictionary<string, string[]>();
        private bool m_ready;
        private IService m_service;

        public ToolboxServiceConfiguration(IService parent)
        {
            InitializeComponent();
            m_service = parent;

            platforms.Add("Desktop", new string[] { "Windows Forms", "Gtk#" });
            platforms.Add("Mobile", new string[] { "Compact Windows Forms" });
            platforms.Add("iDTV", new string[] { "iDTV Swing" });
            string[] platformStrings = new string[platforms.Count];
            platforms.Keys.CopyTo(platformStrings, 0);
            platform.Items.AddRange(platformStrings);

            widgetset.Enabled = false;

            CheckIfReady();
        }

        private void platform_SelectedIndexChanged(object sender, EventArgs e)
        {
            widgetset.Enabled = true;
            widgetset.Items.Clear();
            widgetset.Items.AddRange(platforms[platform.SelectedItem.ToString()]);
            widgetset.SelectedIndex = 0;

            CheckIfReady();
        }

        private void widgetset_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckIfReady();
        }

        public event ReadyStateChangedEventHandler ReadyStateChanged;

        private void CheckIfReady()
        {
            m_ready = platform.SelectedItem != null && widgetset.SelectedItem != null;

            if (Ready)
                OnReadyStateChanged(new ReadyStateChangedEventArgs(true));
            else
                OnReadyStateChanged(new ReadyStateChangedEventArgs(false));
        }

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
            get { return false; } 
        }

        public bool Ready
        {
            get { return m_ready; }
        }

        public IService Service
        {
            get { return m_service; }
        }

        public string Platform
        {
            get { return platform.SelectedItem.ToString(); }
        }

        public string Widgetset
        {
            get { return widgetset.SelectedItem.ToString(); }
        }
    }
}
