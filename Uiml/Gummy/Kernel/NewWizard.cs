using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Uiml.Gummy.Kernel.Services;

namespace Uiml.Gummy.Kernel {
    public partial class NewWizard : Form 
    {
        private List<IServiceConfiguration> m_configurations = new List<IServiceConfiguration>();

        private List<IServiceConfiguration> Configurations
        {
            get { return m_configurations; }
            set { m_configurations = value; }
        }

        private int m_index = -1;
        
        private int Index
        {
          get { return m_index; }
          set { m_index = value; }
        }

        public NewWizard()
        {
            InitializeComponent();
        }

        public void AddConfiguration(IServiceConfiguration config)
        {
            m_configurations.Add(config);
            config.ReadyStateChanged += new ReadyStateChangedEventHandler(config_ReadyStateChanged);
        }

        public void ShowStep()
        {
            // check if we're on the last element
            if (Index == Configurations.Count - 1)
            {
                next.Text = "Finish";
                this.AcceptButton = next;
            }
            else
            {
                this.AcceptButton = null;
                next.Text = "Next";
            }

            // set back button enabled or disabled
            if (Index == 0)
                back.Enabled = false;
            else
                back.Enabled = true;

            // set next button enabled or disabled
            if (Configurations[Index].Ready || Configurations[Index].Optional)
                next.Enabled = true;
            else
                next.Enabled = false;

            // set title
            title.Text = string.Format("Step {0} of {1}", Index + 1, Configurations.Count);
        }

        public void Start()
        {
            NextStep();
        }

        public void NextStep()
        {
            Index++;

            // if we clicked "Finish"
            if (Index == Configurations.Count)
            {
                this.Close();
                
                foreach (IServiceConfiguration config in Configurations)
                {
                    config.Service.NotifyConfigurationChanged();
                }

                return;
            }

            ShowStep();

            // remove previous
            if (Index > 0)
                mainTable.Controls.Remove(Configurations[Index - 1].ServiceConfigurationControl);

            // add current
            mainTable.Controls.Add(Configurations[Index].ServiceConfigurationControl, 1, 1);
        }

        public void PreviousStep()
        {
            Index--;
            ShowStep();

            // remove previous
            if (Index < Configurations.Count - 1)
                mainTable.Controls.Remove(Configurations[Index + 1].ServiceConfigurationControl);

            // add current
            mainTable.Controls.Add(Configurations[Index].ServiceConfigurationControl, 1, 1);
        }

        private void config_ReadyStateChanged(IServiceConfiguration sender, ReadyStateChangedEventArgs e)
        {
            if (e.Ready || sender.Optional) 
                next.Enabled = true;
            else
                next.Enabled = false;
        }

        private void next_Click(object sender, EventArgs e)
        {
            NextStep();
        }

        private void back_Click(object sender, EventArgs e)
        {
            PreviousStep();
        }
    }
}
